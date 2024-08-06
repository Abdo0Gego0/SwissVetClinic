using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class COrder
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = nameof(Messages.GeneralNumber), ResourceType = typeof(Messages))]
        public int OrderNumber {  get; set; }


        [Display(Name = nameof(Messages.CustomerName), ResourceType = typeof(Messages))]


        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }
        

        

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { get; set; }

        [Display(Name = nameof(Messages.CreatedDate), ResourceType = typeof(Messages))]

        public DateTime CreatedDate { get; set; }

        [Display(Name = nameof(Messages.DeliveryDate), ResourceType = typeof(Messages))]

        public DateTime DeliveryDate { get; set; }

        [Display(Name = nameof(Messages.Note), ResourceType = typeof(Messages))]

        public string? CustomerNotes { get; set; } = "";

        [Display(Name = nameof(Messages.Address), ResourceType = typeof(Messages))]

        public string RecipientAddress { get; set; }

        [Display(Name = nameof(Messages.Phone), ResourceType = typeof(Messages))]

        public string RecipientTelephone { get; set; } = "";


        [Display(Name = nameof(Messages.OrderStatus), ResourceType = typeof(Messages))]


        /*      
                new { Id=0,Name="Intialized"},
                new { Id=1,Name="Paid"},
                new { Id=2,Name="In Progress"},
                new { Id=3,Name="On the Way"},
                new { Id=4,Name="Done"},
        */

        public int Status { get; set; }


        [Display(Name = nameof(Messages.TotalCost), ResourceType = typeof(Messages))]

        public double TotalCost { get
            {
                if (COrderItems==null)
                {
                    return 0;
                }

                if (COrderItems.Count == 0)
                {
                    return 0;
                }

                return COrderItems.Sum(a => a.ItemCost);
            }

        }

        [Display(Name = nameof(Messages.DeliveryCost), ResourceType = typeof(Messages))]

        public double DeliveryCost { get; set; } = 0;


        [Display(Name = nameof(Messages.COrderItems), ResourceType = typeof(Messages))]

        public List<COrderItems> COrderItems { get; set; }


        [Display(Name = nameof(Messages.CustomerName), ResourceType = typeof(Messages))]

        public string CustomerName
        {
            get
            {
                try
                {
                    return new ApplicationDbContext().PetOwner.Find(PetOwnerId).FullName;
                }
                catch (Exception ex) {
                
                    return string.Empty;
                }
            }
        }


    }
}
