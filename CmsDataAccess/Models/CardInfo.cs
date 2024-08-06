using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.Models
{
    public class CardInfo
    {
        public string paymentMethodType { get; set; }
        public string CardNumber { get; set; }
        public long? CardExpMonth { get; set; }
        public string CardExpCvc { get; set; }
        public long? CardExpYear { get; set; }

    }

    public class PaymentRequest
    {
        public string PlanId { get; set; }

        public string Token { get; set; }
    }
}
