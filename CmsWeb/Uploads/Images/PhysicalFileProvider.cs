using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders
{
    public class PhysicalFileProvider : IFileProvider, IDisposable
    {
        private const string PollingEnvironmentKey = "DOTNET_USE_POLLING_FILE_WATCHER";
        private static readonly char[] _pathSeparators = new char[2] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private readonly ExclusionFilters _filters;
        private readonly Func<PhysicalFilesWatcher> _fileWatcherFactory;
        private PhysicalFilesWatcher _fileWatcher;
        private bool _fileWatcherInitialized;
        private object _fileWatcherLock = new object();
        private bool? _usePollingFileWatcher;
        private bool? _useActivePolling;
        private bool _disposed;

        public bool UsePollingFileWatcher
        {
            get
            {
                if (_fileWatcher != null)
                {
                    return false;
                }
                if (!_usePollingFileWatcher.HasValue)
                {
                    ReadPollingEnvironmentVariables();
                }
                return _usePollingFileWatcher.GetValueOrDefault();
            }
            set
            {
                if (_fileWatcher != null)
                {
                    throw new InvalidOperationException("Cannot modify UsePollingFileWatcher after the file watcher has been initialized.");
                }
                _usePollingFileWatcher = value;
            }
        }

        public bool UseActivePolling
        {
            get
            {
                if (!_useActivePolling.HasValue)
                {
                    ReadPollingEnvironmentVariables();
                }
                return _useActivePolling.Value;
            }
            set
            {
                _useActivePolling = value;
            }
        }

        internal PhysicalFilesWatcher FileWatcher
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _fileWatcher, ref _fileWatcherInitialized, ref _fileWatcherLock, _fileWatcherFactory);
            }
            set
            {
                _fileWatcherInitialized = true;
                _fileWatcher = value;
            }
        }

        public string Root { get; }

        public PhysicalFileProvider(string root) : this(root, ExclusionFilters.Sensitive)
        {
        }

        public PhysicalFileProvider(string root, ExclusionFilters filters)
        {
            if (!Path.IsPathRooted(root))
            {
                throw new ArgumentException("The path must be absolute.", "root");
            }
            string fullPath = Path.GetFullPath(root);
            Root = EnsureTrailingSlash(fullPath);
            if (!Directory.Exists(Root))
            {
                Directory.CreateDirectory(Root);
            }
            _filters = filters;
            _fileWatcherFactory = CreateFileWatcher;
        }

        internal PhysicalFilesWatcher CreateFileWatcher()
        {
            string fullPath = EnsureTrailingSlash(Path.GetFullPath(Root));
            FileSystemWatcher fileSystemWatcher = null;

            if (OperatingSystem.IsBrowser() || (OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst()) || OperatingSystem.IsTvOS())
            {
                UsePollingFileWatcher = true;
                UseActivePolling = true; // Assuming this is just a setting, not used in PhysicalFilesWatcher.
            }
            else
            {
                fileSystemWatcher = (UsePollingFileWatcher) ? null : new FileSystemWatcher(fullPath);
            }
            return new PhysicalFilesWatcher(fullPath, fileSystemWatcher, UsePollingFileWatcher, _filters);
        }


        [MemberNotNull("_usePollingFileWatcher")]
        [MemberNotNull("_useActivePolling")]
        private void ReadPollingEnvironmentVariables()
        {
            string environmentVariable = Environment.GetEnvironmentVariable(PollingEnvironmentKey);
            bool value = string.Equals(environmentVariable, "1", StringComparison.Ordinal) || string.Equals(environmentVariable, "true", StringComparison.OrdinalIgnoreCase);
            _usePollingFileWatcher = value;
            _useActivePolling = value;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileWatcher?.Dispose();
                }
                _disposed = true;
            }
        }

        private string GetFullPath(string path)
        {
            if (PathNavigatesAboveRoot(path))
            {
                return null;
            }
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(Path.Combine(Root, path));
            }
            catch
            {
                return null;
            }
            if (!IsUnderneathRoot(fullPath))
            {
                return null;
            }
            return fullPath;
        }

        private bool IsUnderneathRoot(string fullPath)
        {
            return fullPath.StartsWith(Root, StringComparison.OrdinalIgnoreCase);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath) || HasInvalidPathChars(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }
            subpath = subpath.TrimStart(_pathSeparators);
            if (Path.IsPathRooted(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }
            string fullPath = GetFullPath(subpath);
            if (fullPath == null)
            {
                return new NotFoundFileInfo(subpath);
            }
            FileInfo fileInfo = new FileInfo(fullPath);
            if (IsExcluded(fileInfo, _filters))
            {
                return new NotFoundFileInfo(subpath);
            }
            return new PhysicalFileInfo(fileInfo);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            try
            {
                if (subpath == null || HasInvalidPathChars(subpath))
                {
                    return NotFoundDirectoryContents.Singleton;
                }
                subpath = subpath.TrimStart(_pathSeparators);
                if (Path.IsPathRooted(subpath))
                {
                    return NotFoundDirectoryContents.Singleton;
                }
                string fullPath = GetFullPath(subpath);
                if (fullPath == null || !Directory.Exists(fullPath))
                {
                    return NotFoundDirectoryContents.Singleton;
                }
                return new PhysicalDirectoryContents(fullPath, _filters);
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            return NotFoundDirectoryContents.Singleton;
        }

        public IChangeToken Watch(string filter)
        {
            if (filter == null || HasInvalidFilterChars(filter))
            {
                return NullChangeToken.Singleton;
            }
            filter = filter.TrimStart(_pathSeparators);
            return FileWatcher.CreateFileChangeToken(filter);
        }

        private static string EnsureTrailingSlash(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }

        private static bool PathNavigatesAboveRoot(string path)
        {
            return path.StartsWith("..", StringComparison.Ordinal) || path.Contains(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal);
        }

        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(Path.GetInvalidPathChars()) != -1;
        }

        private static bool HasInvalidFilterChars(string filter)
        {
            return filter.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;
        }

        private static bool IsExcluded(FileSystemInfo fileSystemInfo, ExclusionFilters filters)
        {
            // Implement exclusion logic here
            return false;
        }
    }
}
