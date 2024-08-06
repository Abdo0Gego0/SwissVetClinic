using CmsDataAccess.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ModelsDto
{
    public class TermsAndConditionsDto
    {
        [Key]
        public Guid Id { get; set; }
        public string? TermsAndConditionsTextEn { get; set; }
        public string? TermsAndConditionsTextAr { get; set; }

        public TermsAndConditions ToModel()
        {
            TermsAndConditions model =new TermsAndConditions();
            model.Id = Id;

            model.TermsAndConditionsTranslation = new List<TermsAndConditionsTranslation> {
                new TermsAndConditionsTranslation
                    {
                    TermsAndConditionsId=Id,
                    TermsAndConditionsText=TermsAndConditionsTextEn,
                    LangCode="en-us"
                },
                new TermsAndConditionsTranslation
                {
                    TermsAndConditionsId=Id,
                    TermsAndConditionsText=TermsAndConditionsTextAr,
                    LangCode="ar"
                },
            };
            return model;
        }
    }
}
