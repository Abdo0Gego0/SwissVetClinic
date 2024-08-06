using CmsResources;
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
    public class ContactInfo
    { 
        [Key]
        public Guid Id { get; set; }

        [Display(Name = nameof(Messages.ContactType), ResourceType = typeof(Messages))]

        public string? ContactType { get; set; } = "";

        [Display(Name = nameof(Messages.ContactValue), ResourceType = typeof(Messages))]

        public string? ContactValue { get; set; } = "";


        public ContactInfo GetFromDb()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            try
            {
                ContactInfo Medic = context.ContactInfo
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
                context.ContactInfo.Add(this);
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
                context.ContactInfo.Remove(GetFromDb());
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
