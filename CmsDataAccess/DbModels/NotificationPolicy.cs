using CmsResources;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class NotificationPolicy
    {

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("MedicalCenter")]
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]


        public Guid MedicalCenterId { set; get; }

        [Display(Name = nameof(Messages.AppointmentNotification), ResourceType = typeof(Messages))]

        [Range(minimum: 0, maximum: int.MaxValue)]
        public int? HoursBeforeAppointment { set; get; } = 1;


        [Display(Name = nameof(Messages.VidoeNotfication), ResourceType = typeof(Messages))]

        [Range(minimum: 0, maximum: int.MaxValue)]
        public int? SessionVideos { set; get; } = 1;

    }

}
