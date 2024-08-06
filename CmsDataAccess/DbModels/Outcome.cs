using Microsoft.AspNetCore.Http;
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
    public class Outcome
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey("Employee")]
        [Display(Name = nameof(Messages.Employee), ResourceType = typeof(Messages))]
        public Guid EmployeeId { get; set; }


        [Display(Name = nameof(Messages.Title), ResourceType = typeof(Messages))]
        public string? Title { get; set; }


        [Display(Name = nameof(Messages.Description), ResourceType = typeof(Messages))]
        public string? Description { get; set; }


        [Display(Name = nameof(Messages.Amount), ResourceType = typeof(Messages))]
        public double Amount { get; set; }


        [Display(Name = nameof(Messages.CreateDate), ResourceType = typeof(Messages))]
        public DateTime CreateDate { get; set; }

    }
}
