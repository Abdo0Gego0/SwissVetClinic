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
	public class CenterMedicineList
    {
		[Key]
		public Guid Id { get; set; }

        [Display(Name = nameof(Messages.Name_), ResourceType = typeof(Messages))]
        public string Name { set; get; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }

        public List<CenterMedicineUnit> CenterMedicineUnit { set; get; }

        public bool IsDeleted { get; set; }=false;

        public void DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            CenterMedicineList centerMedicineList = context.CenterMedicineList.Find(Id);
            centerMedicineList.IsDeleted= true;
            context.CenterMedicineList.Attach(centerMedicineList);
            context.SaveChanges();
        }

    }

}
