using CmsDataAccess.ModelsDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CmsDataAccess.DbModels
{
    public class AboutUs
    {
        [Key]
        public Guid Id { get; set; }
        public List<AboutUsTranslation>? AboutUsTranslation { get; set; }


        public AboutUsDto ToDto()
        {
            return new AboutUsDto
            {
                Id = Id,
                AboutUsTextEn=HttpUtility.HtmlDecode( AboutUsTranslation.FirstOrDefault(a=>a.LangCode=="en-us").AboutUsText),
                AboutUsTextAr= HttpUtility.HtmlDecode(AboutUsTranslation.FirstOrDefault(a=>a.LangCode=="ar").AboutUsText),
            };

        }

        public AboutUs GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                AboutUs Medic = context.AboutUs
                    .Include(a => a.AboutUsTranslation)
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.AboutUs.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.AboutUs.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AboutUs GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                AboutUs Medic = context.AboutUs
                    .Include(a => a.AboutUsTranslation.Where(a=>a.LangCode==langCode))
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
