using CmsDataAccess.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ModelsDto
{
    public class AboutUsDto
    {
        [Key]
        public Guid Id { get; set; }
        public string? AboutUsTextEn { get; set; }
        public string? AboutUsTextAr { get; set; }

        public AboutUs ToModel()
        {
            AboutUs model=new AboutUs();
            model.Id = Id; 
            model.AboutUsTranslation = new List<AboutUsTranslation> {
                new AboutUsTranslation
                    {
                    AboutUsId=Id,
                    AboutUsText=AboutUsTextEn,
                    LangCode="en-us"
                },
                new AboutUsTranslation
                {
                    AboutUsId=Id,
                    AboutUsText=AboutUsTextAr,
                    LangCode="ar"
                },
            };
            return model;
        }
    }
}
