using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class PatientArchivedNotification
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Title { get; set; }

        public string Message { get; set; } = "";
        public int? NotiType { get; set; }

        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }
        public bool IsAuto { get; set; } = true;
        public bool IsRead { get; set; } = true;
    }
}
