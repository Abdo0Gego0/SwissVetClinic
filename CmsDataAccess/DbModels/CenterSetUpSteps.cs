using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class CenterSetUpSteps
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("CenterAdmin")]
        public Guid CenterAdminId { set; get; }
        public int StepNumer {  get; set; }
        
    }
}
