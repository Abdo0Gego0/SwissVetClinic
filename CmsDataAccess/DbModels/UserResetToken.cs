using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
    public class UserResetToken
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);
    }
}
