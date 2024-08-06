using CmsDataAccess.AutoMapping;
using CmsDataAccess.DbModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ModelsDto
{
    public class MedicalCenterDto: MedicalCenter 
    {

        public bool IsCenterActive
        {
            get
            {
                return IsCenterActive();
            }
        }

        public bool IsOpen
        {
            get
            {
                return IsOpen();
            }
        }


        public MedicalCenter ToModel()
        {
            return MappingToDto.Mapper.Map<MedicalCenter>(this);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (LangCode.IsNullOrEmpty())
            {
                stringBuilder.AppendLine(string.Format("Name: {0}, IsOpen: {1}, IsActive: {2}, Working Hours: {3}",
                this.MedicalCenterTranslation.FirstOrDefault() == null ? "" : this.MedicalCenterTranslation.FirstOrDefault().Name,
                this.IsOpen(),
                this.IsCenterActive(),
                this.WorkHoursToString()
                ));
                return stringBuilder.ToString();
            }
            else
            {
                stringBuilder.AppendLine(string.Format("Name: {0}, IsOpen: {1}, IsActive: {2}, Working Hours: {3}",
                    this.MedicalCenterTranslation.Where(a=>a.LangCode== LangCode).FirstOrDefault() == null ? "" :
                    this.MedicalCenterTranslation.Where(a => a.LangCode == LangCode).FirstOrDefault().Name,
                    this.IsOpen(),
                    this.IsCenterActive(),
                    this.WorkHoursToString()
                    ));
                return stringBuilder.ToString();
            }
        }

    }
}
