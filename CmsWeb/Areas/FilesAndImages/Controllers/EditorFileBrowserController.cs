using Kendo.Mvc.UI;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CmsWeb.Areas.FilesAndImages.Controllers
{
    public abstract class EditorFileBrowserController : BaseFileBrowserController
    {
        /// <summary>
        /// Gets the valid file extensions by which served files will be filtered.
        /// </summary>
        public override string Filter
        {
            get
            {
                return EditorFileBrowserSettings.DefaultFileTypes;
            }
        }

        public EditorFileBrowserController(IHostingEnvironment hostingEnvironment)
            : base(hostingEnvironment)
        {

        }
    }
}