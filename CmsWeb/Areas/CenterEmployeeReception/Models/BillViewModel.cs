using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsWeb.Areas.CenterEmployeeReception.Models
{
    public class BillViewModel
    {
        public DateTime BillDate { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }

        public List<BillItemViewModel> Items { get; set; }

        public BillViewModel()
        {
            Items = new List<BillItemViewModel>();
        }
    }

    public class BillItemViewModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }
}

