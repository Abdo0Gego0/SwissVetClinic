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
	public class CenterMedicineUnit
    {
		[Key]
		public Guid Id { get; set; }

        [ForeignKey("CenterMedicineList")]
        public Guid CenterMedicineListId { set; get; }

        [Display(Name = nameof(Messages.SmallestUnit), ResourceType = typeof(Messages))]
        public string SmallestUnit { set; get; }

        [Display(Name = nameof(Messages.PricePerUnit), ResourceType = typeof(Messages))]
        public float PricePerUnit { set; get; }

        public bool IsDeleted { get; set; }=false;

        public void DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            CenterMedicineUnit centerMedicineList = context.CenterMedicineUnit.Find(Id);
            centerMedicineList.IsDeleted= true;
            context.CenterMedicineUnit.Attach(centerMedicineList);
            context.Entry(centerMedicineList).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

    }

}
