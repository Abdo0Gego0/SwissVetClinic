using CmsResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class VisistBill
    {

        [Key]
        public Guid Id { get; set; }
        public int Number { get; set; }

        [Display(Name = nameof(Messages.ServiceCost), ResourceType = typeof(Messages))]

        public double? ServiceCost { get; set; }
        [Display(Name = nameof(Messages.MedicnieCost), ResourceType = typeof(Messages))]

        public double? MedicnieCost { get; set; }

        [Display(Name = nameof(Messages.Type), ResourceType = typeof(Messages))]

        public int? PaymentType { get; set; }

        [Display(Name = nameof(Messages.BillStatus), ResourceType = typeof(Messages))]

        public int Status { get; set; } = 0;

        [Display(Name = nameof(Messages.Discount), ResourceType = typeof(Messages))]

        public double Discount { get; set; } = 0;



        [ForeignKey("CenterServices")]
        [Display(Name = nameof(Messages.ServiceType), ResourceType = typeof(Messages))]
        public Guid CenterServicesId { get; set; }


        [ForeignKey("Doctor")]
        [Display(Name = nameof(Messages.Doctor), ResourceType = typeof(Messages))]
        public Guid DoctorId { get; set; }


        [ForeignKey("BaseClinic")]
        [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]
        public Guid BaseClinicId { get; set; }

        [ForeignKey("PetOwner")]

        [Display(Name = nameof(Messages.Patient), ResourceType = typeof(Messages))]

        public Guid PetOwnerId { get; set; }

        [ForeignKey("PatientVisit")]

        public Guid PatientVisitId { get; set; }

        [UIHint("Date")]
        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]

        public DateTime? CreateDate { set; get; }


        [UIHint("Date")]
        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]

        public DateTime? PaymenrDate { set; get; }

    }
}
