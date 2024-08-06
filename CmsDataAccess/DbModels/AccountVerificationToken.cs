using CmsDataAccess.ModelsDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CmsDataAccess.DbModels
{
    public class AccountVerificationToken
    {
        [Key]
        public Guid Id { get; set; }

        public string username { get; set; }
        public string code { get; set; }

    }
}
