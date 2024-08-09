using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.Utils.ProductUtil
{
    public class SubProductDetails
    {
        public Guid Id { get; set; }
        public List<string> Images { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public List<string> Characteristics { get; set; }
        public bool Available { get; set; }
        public object IsInBasket { get; set; }
    }
}
