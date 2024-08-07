using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Areas.CenterEmployeeReception.Models
{
    public class BillEditViewModel
    {
        public Guid Id { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BillProductViewModel> Items { get; set; } = new List<BillProductViewModel>();
    }



}