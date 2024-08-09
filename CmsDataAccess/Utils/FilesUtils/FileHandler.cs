using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;


namespace CmsDataAccess.Utils.FilesUtils
{
    public  class FileHandler
    {
        public static string getUploadfolder()
        {
            
            var configurationBuilder = new ConfigurationBuilder();
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            var images = root.GetSection("Images");
            var pathToImages = images.GetSection("PathToImages").Value;

            // Assuming PathToImages in appsettings.json is a relative path
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            var fullPathToImages = Path.Combine(parentDirectory, pathToImages);

            return fullPathToImages;
        }


        public static void AppendToFile(string fullPath, IFormFile content)
        {
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    content.CopyTo(stream);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public static string SaveUploadedFile(IEnumerable<IFormFile> ClinicImage)
        {


            if (ClinicImage != null)
            {
                foreach (var file in ClinicImage)
                {
                    string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "."+System.IO.Path.GetExtension(file.FileName);
                    string path = getUploadfolder() + uniqueFileName;
                    AppendToFile(path, file);

                    return uniqueFileName;

                }
            }
            return "";


        }

        public static string SaveUploadedFile(IFormFile file)
        {
            if (file != null)
            {
                string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "." + System.IO.Path.GetExtension(file.FileName);
                string path = getUploadfolder() + uniqueFileName;
                AppendToFile(path, file);

                return uniqueFileName;
            }
            return "";
        }

        public static string SaveUploadedFile(IFormFile file, string CutomPath)
        {
            if (file != null)
            {
                AppendToFile(CutomPath, file);

                return "";
            }
            return "";
        }

        public static string SaveUploadedFileFrom64(string data)
        {
            //if (file != null)
            //{
            //    string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "." + System.IO.Path.GetExtension(file.FileName);
            //    string path = getUploadfolder() + uniqueFileName;
            //    AppendToFile(path, file);
            //    return uniqueFileName;
            //}

            System.Drawing.Image image;

            string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + ".png" ;
            string path = getUploadfolder() + uniqueFileName;

            byte[] bytes = Convert.FromBase64String(data.Split(';')[1].Split(',')[1]);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = System.Drawing.Image.FromStream(ms);
            }

            var ImageToSave = new Bitmap(image);
            ImageToSave.Save(path, ImageFormat.Png);

            return uniqueFileName;
        }

        public static string UpdateProfileImage(IFormFile file,string OldImage)
        {

            string path = getUploadfolder() + OldImage;

            if (System.IO.File.Exists(path))
            {
                File.Delete(path);
            }

            if (file != null)
            {
                string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "." + System.IO.Path.GetExtension(file.FileName);
                path = getUploadfolder() + uniqueFileName;
                AppendToFile(path, file);

                return uniqueFileName;
            }

            return "";

        }

        public static void DeleteImageFile(string fileName)
        {
            string path = getUploadfolder() + fileName;

            if(System.IO.File.Exists(path))
            {
               File.Delete(path);
            }


        }

        public static string UpdateImageFile(string oldImage,IEnumerable<IFormFile> ClinicImage)
        {


            if (ClinicImage.Count()>0)
            {
                DeleteImageFile(oldImage);

                foreach (var file in ClinicImage)
                {
                    string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "." + System.IO.Path.GetExtension(file.FileName);
                    string path = getUploadfolder() + uniqueFileName;
                    AppendToFile(path, file);

                    return uniqueFileName;

                }
            }
            return oldImage;


        }

        public static string UpdateImageFile(string? oldImage, IFormFile file)
        {

            if (file != null)
            {
                if(!string.IsNullOrEmpty(oldImage))
                {
                    DeleteImageFile(oldImage);
                }

                //string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString() + "." + System.IO.Path.GetExtension(file.FileName);
                string uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks.ToString()  + System.IO.Path.GetExtension(file.FileName);
                string path = getUploadfolder() + uniqueFileName;
                AppendToFile(path, file);

                return uniqueFileName;

            }

            return oldImage;
            
        }

    }
}
