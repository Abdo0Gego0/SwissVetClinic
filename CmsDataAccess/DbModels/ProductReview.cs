using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class ProductReview
    {

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        

        [ForeignKey("PetOwner")]
        public Guid PetOwnerId { get; set; }

        public double Value { get; set; }
        public string? Comment { get; set; } = "";

    }
}
