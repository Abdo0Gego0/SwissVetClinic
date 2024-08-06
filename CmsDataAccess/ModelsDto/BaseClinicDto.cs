using CmsDataAccess.AutoMapping;
using CmsDataAccess.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ModelsDto
{
    public class BaseClinicDto : BaseClinic 
    {

        public ClinicSpecialty ClinicSpecialty
        {
            get { 
            return this.ClinicSpecialty;
            }
        }


    }
}
