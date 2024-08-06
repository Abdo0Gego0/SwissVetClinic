using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.DbModels
{
	public class ValidQR 
    {
		[Key]
		public Guid Id { get; set; }

        [ForeignKey("MedicalCenter")]
        public Guid MedicalCenterId { set; get; }
        public string Secretkey { get; set; }
		public DateTime ExpireDate { get; set; }= DateTime.Now.AddSeconds(20);
		public bool QRType { get; set; }

    }

}
