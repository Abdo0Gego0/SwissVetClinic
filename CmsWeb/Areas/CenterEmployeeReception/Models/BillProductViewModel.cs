namespace CmsWeb.Areas.CenterEmployeeReception.Models
{
    public class BillProductViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }
}