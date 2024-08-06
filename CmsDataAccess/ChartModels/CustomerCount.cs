using CmsDataAccess.DbModels;
using CmsResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.ChartModels
{
    public class CustomerCount
    {
        public CustomerCount()
        {
        }

        public CustomerCount(string month, int count, int old, int new_)
        {
            Month = month;
            Old = old;
            New = new_;
        }

        public string Month { get; set; }
        public int Old { get; set; }
        public int New { get; set; }

    }

    public class TopCustomers
    {
        public TopCustomers()
        {
        }

        public TopCustomers(string Name_, int OrdersCount_, int OrdersValue_)
        {
            Name = Name_;
            OrdersCount = OrdersCount_;
            OrdersValue = OrdersValue_;
        }


        [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]

        public string Name { get; set; }
        [Display(Name = nameof(Messages.OrdersCount), ResourceType = typeof(Messages))]

        public int OrdersCount { get; set; }
        [Display(Name = nameof(Messages.OrdersValue), ResourceType = typeof(Messages))]

        public double OrdersValue { get; set; }

    }

    public class TopProducts
    {
        public TopProducts()
        {
        }

        public TopProducts(string Name_, int OrdersCount_, int OrdersValue_)
        {
            Name = Name_;
            OrdersCount = OrdersCount_;
            OrdersValue = OrdersValue_;
        }


        [Display(Name = nameof(Messages.Name), ResourceType = typeof(Messages))]

        public string Name { get; set; }
        [Display(Name = nameof(Messages.OrdersCount), ResourceType = typeof(Messages))]

        public double OrdersCount { get; set; }
        [Display(Name = nameof(Messages.OrdersValue), ResourceType = typeof(Messages))]

        public double OrdersValue { get; set; }
        public List<SubProductImage> SubProductImage { get; set; }


    }

    public class StockDataPoint
    {
        public DateTime Date { get; set; }

        public double Value { get; set; }

    }  
    
    public class VisitCount
    {
        public string category { get; set; }

        public double value { get; set; }

        public string Color {  get; set; }

    }




}
