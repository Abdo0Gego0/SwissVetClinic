using System;
using System.IO;
using System.Linq;
using Kendo.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Kendo.Mvc.UI;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using CmsDataAccess.Utils.FilesUtils;
using CmsDataAccess.DbModels;

namespace CmsWeb.Areas.FilesAndImages.Controllers
{
    [Area("FilesAndImages")]

    public class FileManagerDataController : Controller
    {
        protected readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment;
        private readonly FileContentBrowser directoryBrowser;
        //
        // GET: /FileManager/
        private const string contentFolderRoot = "PatientFiles";
        private const string prettyName = "Folders";
        private static readonly string[] foldersToCopy = new[] { "PatientFiles/filemanager" };


        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }

        public FileManagerDataController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            directoryBrowser = new FileContentBrowser();
        }

        /// <summary>
        /// Gets the valid file extensions by which served files will be filtered.
        /// </summary>
        public string Filter
        {
            get
            {
                return "*.*";
            }
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, prettyName);
            string path = Path.Combine(HostingEnvironment.ContentRootPath, contentFolderRoot, prettyName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(HostingEnvironment.WebRootFileProvider.GetFileInfo(sourceFolder).PhysicalPath, path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// Determines if content of a given path can be browsed.
        /// </summary>
        /// <param name="path">The path which will be browsed.</param>
        /// <returns>true if browsing is allowed, otherwise false.</returns>
        public virtual bool Authorize(string path)
        {

            return true;
            return CanAccess(path);
        }

        protected virtual bool CanAccess(string path)
        {
            return true;
            var rootPath = Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath));
            return path.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath));
            }
            else
            {
                return Path.GetFullPath(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath, path));
            }
        }

        protected virtual FileManagerEntry VirtualizePath(FileManagerEntry entry)
        {
            entry.Path = entry.Path.Replace(Path.Combine(this.HostingEnvironment.WebRootPath, ContentPath), "").Replace(@"\", "/").TrimStart('/');
            return entry;
        }

        public virtual ActionResult Create(string target,string customPath, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(target)))
            {
                throw new Exception("Forbidden");
            }


            if (String.IsNullOrEmpty(entry.Path))
            {
                newEntry = CreateNewFolder(target, customPath, entry);
            }
            else
            {
                newEntry = CopyEntry(target, entry);
            }

            return Json(VirtualizePath(newEntry));
        }

        protected virtual FileManagerEntry CopyEntry(string target, FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(NormalizePath(target), entry);

            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                CopyDirectory(new DirectoryInfo(physicalPath), Directory.CreateDirectory(physicalTarget));
                newEntry = directoryBrowser.GetDirectory(physicalTarget);
            }
            else
            {
                System.IO.File.Copy(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);
            }

            return newEntry;
        }

        protected virtual void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        protected virtual FileManagerEntry CreateNewFolder(string target, string customPath, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;
            //var path = NormalizePath(customPath);

            string path = Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath,  target.IsNullOrEmpty() ? "" : target);

            string physicalPath = EnsureUniqueName(path, entry);

            Directory.CreateDirectory(physicalPath);

            newEntry = directoryBrowser.GetDirectory(physicalPath);

            return newEntry;
        }

        protected virtual string EnsureUniqueName(string target, FileManagerEntry entry)
        {
            var tempName = entry.Name + entry.Extension;
            int sequence = 0;
            var physicalTarget = Path.Combine(NormalizePath(target), tempName);

            if (!Authorize(NormalizePath(physicalTarget)))
            {
                throw new Exception("Forbidden");
            }

            if (entry.IsDirectory)
            {
                while (Directory.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence);
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }
            else
            {
                while (System.IO.File.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence) + entry.Extension;
                    physicalTarget = Path.Combine(NormalizePath(target), tempName);
                }
            }

            return physicalTarget;
        }

        public virtual ActionResult Destroy(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);

            if (path.Contains("Videos"))
            {
                throw new Exception("Forbidden");
            }

            if (!string.IsNullOrEmpty(path))
            {
                if (entry.IsDirectory)
                {
                    DeleteDirectory(path);
                }
                else
                {
                    DeleteFile(path);
                }

                return Json(new object[0]);
            }
            throw new Exception("File Not Found");
        }

        protected virtual void DeleteFile(string path)
        {
            if (path.Contains("Videos"))
            {
                throw new Exception("Forbidden");
            }

            if (!Authorize(path))
            {
                throw new Exception("Forbidden");
            }

            var physicalPath = NormalizePath(path);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            if (!Authorize(path))
            {
                throw new Exception("Forbidden");
            }

            var physicalPath = NormalizePath(path);

            if (Directory.Exists(physicalPath))
            {
                Directory.Delete(physicalPath, true);
            }
        }

        public virtual JsonResult Read(string target,string customPath)
        {
            //var path = NormalizePath(customPath);

            string path = Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath,target.IsNullOrEmpty()?"":target);

            if (Authorize(path))
            {
                try
                {
                    var files = directoryBrowser.GetFiles(path, Filter);
                    var directories = directoryBrowser.GetDirectories(path);
                    var result = files.Concat(directories).Select(VirtualizePath);

                    return Json(result.ToArray());
                }
                catch (DirectoryNotFoundException)
                {
                    throw new Exception("File Not Found");
                }
            }

            throw new Exception("Forbidden");
        }

        /// <summary>
        /// Updates an entry with a given entry.
        /// </summary>
        /// <param name="path">The path to the parent folder in which the folder should be created.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>An empty <see cref="ContentResult"/>.</returns>
        /// <exception cref="HttpException">Forbidden</exception>
        public virtual ActionResult Update(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(entry.Path)) && !Authorize(NormalizePath(target)))
            {
                throw new Exception("Forbidden");
            }

            newEntry = RenameEntry(entry);

            return Json(VirtualizePath(newEntry));
        }

        protected virtual FileManagerEntry RenameEntry(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = path;
            var physicalTarget = EnsureUniqueName(Path.GetDirectoryName(path), entry);
            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                Directory.Move(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetDirectory(physicalTarget);
            }
            else
            {
                var file = new FileInfo(physicalPath);
                System.IO.File.Move(file.FullName, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);
            }

            return newEntry;
        }

        /// <summary>
        /// Determines if a file can be uploaded to a given path.
        /// </summary>
        /// <param name="path">The path to which the file should be uploaded.</param>
        /// <param name="file">The file which should be uploaded.</param>
        /// <returns>true if the upload is allowed, otherwise false.</returns>
        public virtual bool AuthorizeUpload(string path, IFormFile file)
        {
            if (!CanAccess(path))
            {
                throw new DirectoryNotFoundException(String.Format("The specified path cannot be found - {0}", path));
            }

            if (!IsValidFile(GetFileName(file)))
            {
                throw new InvalidDataException(String.Format("The type of file is not allowed. Only {0} extensions are allowed.", Filter));
            }

            return true;
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = Filter.Split(',');

            return allowedExtensions.Any(e => e.Equals("*.*") || e.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Uploads a file to a given path.
        /// </summary>
        /// <param name="path">The path to which the file should be uploaded.</param>
        /// <param name="file">The file which should be uploaded.</param>
        /// <returns>A <see cref="JsonResult"/> containing the uploaded file's size and name.</returns>
        /// <exception cref="HttpException">Forbidden</exception>
        [AcceptVerbs("POST")]
        public virtual ActionResult Upload(string path, IFormFile file)
        {
            FileManagerEntry newEntry;
            path = NormalizePath(path);
            var fileName = Path.GetFileName(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                SaveFile(file, path);
                newEntry = directoryBrowser.GetFile(Path.Combine(path, fileName));

                return Json(VirtualizePath(newEntry));
            }

            throw new Exception("Forbidden");
        }

        protected virtual void SaveFile(IFormFile file, string pathToSave)
        {
            try
            {
                var path = Path.Combine(pathToSave, GetFileName(file));
                using (var stream = System.IO.File.Create(path))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public virtual string GetFileName(IFormFile file)
        {
            var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            return Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
        }


        public ActionResult Video_Save(IEnumerable<IFormFile> files,string customPath,Guid visitId)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));

                    if (!Directory.Exists(Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath, "Videos")))
                    {
                        Directory.CreateDirectory(Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath, "Videos"));
                    }

                    string path = Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath,"Videos",
                         fileName);

                    FileHandler.SaveUploadedFile(file, path);


                    ApplicationDbContext cmsContext = new ApplicationDbContext();

                    PatientVisit visit = cmsContext.PatientVisit.Find(visitId);
                    visit.VisitVideo = "//"+customPath+"//" + "Videos"+"//" + fileName;

                    cmsContext.PatientVisit.Attach(visit);
                    cmsContext.Entry(visit).Property(a => a.VisitVideo).IsModified = true;
                    cmsContext.SaveChanges();



                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Video_Remove(string[] fileNames, string customPath, Guid visitId)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(HostingEnvironment.ContentRootPath, "PatientFiles", customPath, "Videos",
                         fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                         System.IO.File.Delete(physicalPath);
                    }

                    ApplicationDbContext cmsContext = new ApplicationDbContext();

                    PatientVisit visit = cmsContext.PatientVisit.Find(visitId);
                    visit.VisitVideo = "";

                    cmsContext.PatientVisit.Attach(visit);
                    cmsContext.Entry(visit).Property(a => a.VisitVideo).IsModified = true;
                    cmsContext.SaveChanges();

                }
            }

            // Return an empty string to signify success
            return Content("");
        }





    }

    public class FileContentBrowser
    {
        public virtual Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; set; }
        public IEnumerable<FileManagerEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(path);

            var extensions = (filter ?? "*").Split(new string[] { ", ", ",", "; ", ";" }, System.StringSplitOptions.RemoveEmptyEntries);

            return extensions.SelectMany(directory.GetFiles)
                .Select(file => new FileManagerEntry
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Size = file.Length,
                    Path = file.FullName,
                    Extension = file.Extension,
                    IsDirectory = false,
                    HasDirectories = false,
                    Created = file.CreationTime,
                    CreatedUtc = file.CreationTimeUtc,
                    Modified = file.LastWriteTime,
                    ModifiedUtc = file.LastWriteTimeUtc
                });
        }

        public IEnumerable<FileManagerEntry> GetDirectories(string path)
        {
            var directory = new DirectoryInfo(path);

            return directory.GetDirectories()
                .Select(subDirectory => new FileManagerEntry
                {
                    Name = subDirectory.Name,
                    Path = subDirectory.FullName,
                    Extension = subDirectory.Extension,
                    IsDirectory = true,
                    HasDirectories = subDirectory.GetDirectories().Length > 0,
                    Created = subDirectory.CreationTime,
                    CreatedUtc = subDirectory.CreationTimeUtc,
                    Modified = subDirectory.LastWriteTime,
                    ModifiedUtc = subDirectory.LastWriteTimeUtc
                });
        }

        public FileManagerEntry GetDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            return new FileManagerEntry
            {
                Name = directory.Name,
                Path = directory.FullName,
                Extension = directory.Extension,
                IsDirectory = true,
                HasDirectories = directory.GetDirectories().Length > 0,
                Created = directory.CreationTime,
                CreatedUtc = directory.CreationTimeUtc,
                Modified = directory.LastWriteTime,
                ModifiedUtc = directory.LastWriteTimeUtc
            };
        }

        public FileManagerEntry GetFile(string path)
        {
            var file = new FileInfo(path);

            return new FileManagerEntry
            {
                Name = Path.GetFileNameWithoutExtension(file.Name),
                Path = file.FullName,
                Size = file.Length,
                Extension = file.Extension,
                IsDirectory = false,
                HasDirectories = false,
                Created = file.CreationTime,
                CreatedUtc = file.CreationTimeUtc,
                Modified = file.LastWriteTime,
                ModifiedUtc = file.LastWriteTimeUtc
            };
        }
    }

}
