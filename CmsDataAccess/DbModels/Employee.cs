using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using CmsDataAccess.Utils.FilesUtils;
using Microsoft.EntityFrameworkCore;
using CmsResources;


namespace CmsDataAccess.DbModels
{
    [Table("Employee")]

    public class Employee : Person
    {

        [Display(Name = nameof(Messages.EmployeeRole), ResourceType = typeof(Messages))]
        public int EmployeeRole { get; set; } // {0: Reception, 1: OrdersManagement }



        [ForeignKey("Clinic")]
        [Display(Name = nameof(Messages.ClinicId), ResourceType = typeof(Messages))]
        public Guid? ClinicId { get; set; }

        [ForeignKey("MedicalCenter")]
        [Display(Name = nameof(Messages.MedicalCenterId), ResourceType = typeof(Messages))]


        public Guid? MedicalCenterId { get; set; }

        public BaseClinic? BaseClinic()
        {
            try
            {
                return new ApplicationDbContext().BaseClinic.Find(ClinicId);
            }
            catch (Exception)
            {

                return null;
            }
        }

        [NotMapped]
        public string? _LangCode;

        [NotMapped]
        public string? LangCode
        {
            get { return _LangCode; }
            set
            {

                try
                {
                    _LangCode = value.ToLower();
                }
                catch { }
            }
        }





        public Employee GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                Employee Medic = context.Employee
                    .Include(a => a.User)
                    .FirstOrDefault(a => a.Id == Id);

                return Medic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertIntoDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                context.Employee.Add(this);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Employee temp = GetFromDb();
                FileHandler.DeleteImageFile(temp.ImageName);

                temp = GetFromDb();
                context.Employee.Remove(temp);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool SoftDeleteFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                Employee temp = GetFromDb();

                temp.IsDeleted=true;

                context.Employee.Attach(temp);
                context.Entry(temp).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }








    }
}
