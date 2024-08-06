using CmsDataAccess.DbModels;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.AppointmentSRVC
{
    public class PatientViewModel 
    {
        public Guid Value { get; set; }
        public string Text { get; set; }
      
    }
}
