using Microsoft.AspNetCore.Mvc;

namespace CmsWeb.Areas.FilesAndImages.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;

    public class EditorExportController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EditorExportController(IWebHostEnvironment environment)
        {
            this._hostingEnvironment = environment;
        }

        //[HttpPost]
        //public ActionResult Export(EditorExportData data)
        //{
        //    var settings = new EditorDocumentsSettings();
        //    settings.HtmlImportSettings.LoadFromUri += HtmlImportSettings_LoadFromUri;

        //    try
        //    {
        //        return EditorExport.Export(data, settings);
        //    }
        //    catch
        //    {
        //        return RedirectToAction("import-export", "editor");
        //    }
        //}

  
    }
}
