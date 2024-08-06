using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.MobileViewModels.AuthModels
{
    public class UserViewModel
    {
        public string PhoneNumber { get; set; } = "";
        //public string Title { get; set; } = "";
        public string FirstName { get; set; }
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        //public string Nationality { get; set; } = "";
        //public string NationalCardId { get; set; } = "";
    }
}
