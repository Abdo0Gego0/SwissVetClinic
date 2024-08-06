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
    public class TermsAndConditions
    {
        [Key]
        public Guid Id { get; set; }
        public List<TermsAndConditionsTranslation>? TermsAndConditionsTranslation { get; set; }

        public TermsAndConditionsDto ToDto()
        {
            return new TermsAndConditionsDto
            {
                Id = Id,
                TermsAndConditionsTextEn = HttpUtility.HtmlDecode(TermsAndConditionsTranslation.FirstOrDefault(a => a.LangCode == "en-us").TermsAndConditionsText),
                TermsAndConditionsTextAr = HttpUtility.HtmlDecode(TermsAndConditionsTranslation.FirstOrDefault(a => a.LangCode == "ar").TermsAndConditionsText),
            };
        }


        public TermsAndConditions GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                TermsAndConditions Medic = context.TermsAndConditions
                    .Include(a => a.TermsAndConditionsTranslation)
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
                context.TermsAndConditions.Add(this);
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
                context.TermsAndConditions.Remove(GetFromDb());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TermsAndConditions GetModelByLnag(string langCode = "en-US")
        {
            langCode = langCode.ToLower();

            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                TermsAndConditions Medic = context.TermsAndConditions
                    .Include(a => a.TermsAndConditionsTranslation.Where(a=>a.LangCode==langCode))
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
