using CmsDataAccess.DbModels;
using CmsDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Web;

namespace ServicesLibrary.PrintServices
{
    public  class DiagnosisTemplate
    {
       


        public string ComapnyLogo { get; set; } = "https://swissvetclinic.com/siteimages/logo.png";



        public string FileHeader = @" <html>
                                    <head>
                                        <style>
                                        body {
                                        font-family: 'Arial', 'DejaVuSans', 'sans-serif';
                                        }
                                        </style>
                                    </head>";
        public string FileBody { get; set; }

        public DiagnosisTemplate(PatientVisit temp)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            CenterServices CenterServices=context.CenterServices.Include(a=>a.CenterServicesTranslation).FirstOrDefault(a => a.Id == temp.CenterServicesId);
            string ServiceName = CenterServices.CenterServicesTranslation[0].Name+" " + CenterServices.CenterServicesTranslation[1].Name;

            string BillCompanyName= "SWISS VETERINARY CLINIC (SVC)";
            
            string ClientName = context.PetOwner.Find(temp.PetOwnerId).FullName;
            string ClinicName = context.BaseClinic.Include(a=>a.BaseClinicTranslation).FirstOrDefault(a=>a.Id==temp.BaseClinicId).ClinicName;
            string DoctorName = context.Doctor.Find(temp.DoctorId).FullName;
            
            List<VisitMedicine> BillItems = context.VisitMedicine.Where(a => a.PatientVisitId == temp.Id).ToList();

            string billDate = "";
            
            if (temp.VisistDate != null )
            {
                billDate= temp.VisistDate.ToString("yyyy-MM-dd HH:mm");

            }


            string BillFile = FileHeader;

            string logoAndName = $"    <center>\r\n        <img src='{ComapnyLogo}' />\r\n        <br />\r\n        <h1>{BillCompanyName}</h1>\r\n    </center>";

            BillFile += logoAndName;
            BillFile += "<br/>";
        



            FileBody = @$"<body dir='LTR'>{BillFile}</body></html>";

        }
    }
}
