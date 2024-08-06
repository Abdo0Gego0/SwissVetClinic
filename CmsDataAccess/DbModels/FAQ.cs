using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class FAQ
    {
        [Key]
        public Guid Id { set; get; }
        public List<FAQTranslation> FAQTranslation { get; set; }
    }
}
