using Kendo.Mvc.UI;
using Microsoft.Extensions.Hosting.Internal;

namespace CmsWeb.Areas.FilesAndImages.Controllers
{
    public class FileBrowserController : EditorFileBrowserController
    {
        private const string contentFolderRoot = "PatientFiles/";
        private const string folderName = "Files/";
        private static readonly string[] foldersToCopy = new[] { "PatientFiles/UserFiles/Files/" };
        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }

        /// <summary>
        /// Gets the valid file extensions by which served files will be filtered.
        /// </summary>
        public override string Filter
        {
            get
            {
                return "*.pdf, *.txt, *.doc, *.docx, *.xls, *.xlsx, *.ppt, *.pptx, *.zip, *.rar, *.jpg, *.jpeg, *.gif, *.png";
            }
        }

        public FileBrowserController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
            : base(hostingEnvironment)
        {
        }
        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, "UserFiles", folderName);
            string path = Path.Combine(HostingEnvironment.ContentRootPath, contentFolderRoot, "UserFiles", folderName);


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
    }
}
