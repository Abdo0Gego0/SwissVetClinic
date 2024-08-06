using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class Family
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FamilyId { get; set; }
        public Guid PersonId { get; set; }

    }
}
