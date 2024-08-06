
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class COrderItems
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("COrder")]
        public Guid COrderId { get; set; }

        [ForeignKey("SubProduct")]
        public Guid SubProductId { get; set; }
        
        public double ItemPrice { get; set; }

        public double ItemQuantity { get; set; }

        public double ItemCost { get
            {
                return ItemPrice * ItemQuantity;
            }
        }


        






    }
}
