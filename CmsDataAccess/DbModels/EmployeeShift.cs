using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class EmployeeShift
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("IdentityUser")]
        public Guid IdentityUserId { set; get; }
        [ForeignKey("ShiftTable")]
        public Guid ShiftTableId { set; get; }
        public EmployeeShift GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                EmployeeShift Medic = context.EmployeeShift
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
                context.EmployeeShift.Add(this);
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
                context.EmployeeShift.Remove(GetFromDb());
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
