using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using CmsDataAccess.Utils.FilesUtils;
using Microsoft.EntityFrameworkCore;
using CmsResources;

namespace CmsDataAccess.DbModels
{
    public class ReceiptProduct
    {

        public Guid ReceiptProductID { get; set; } = Guid.NewGuid();

        [ForeignKey("Receipt")]
        public Guid ReceiptID { get; set; }

        [ForeignKey("Product")]
        public Guid ProductID { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        
    }
}
