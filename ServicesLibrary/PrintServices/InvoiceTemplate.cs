using CmsDataAccess.DbModels;
using CmsDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Web;

namespace ServicesLibrary.PrintServices
{
    public  class InvoiceTemplate
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

        public InvoiceTemplate(VisistBill temp)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            CenterServices CenterServices=context.CenterServices.Include(a=>a.CenterServicesTranslation).FirstOrDefault(a => a.Id == temp.CenterServicesId);
            string ServiceName = CenterServices.CenterServicesTranslation[0].Name+" " + CenterServices.CenterServicesTranslation[1].Name;

            string BillCompanyName= "SWISS VETERINARY CLINIC (SVC)";
            string? BillNumber = temp.Number.ToString();
            string ClientName = context.PetOwner.Find(temp.PetOwnerId).FullName;
            string ClinicName = context.BaseClinic.Include(a=>a.BaseClinicTranslation).FirstOrDefault(a=>a.Id==temp.BaseClinicId).ClinicName;
            string DoctorName = context.Doctor.Find(temp.DoctorId)?.FullName ?? "Unknown";

            var doctor = context.Doctor.Find(temp.DoctorId);
            if (doctor == null)
            {
                // Log this incident
                Console.WriteLine($"Doctor with ID {temp.DoctorId} not found in the database.");
                DoctorName = "Doctor Not Found";
            }
            else if (string.IsNullOrEmpty(doctor.FullName))
            {
                // Log this incident
                Console.WriteLine($"Doctor with ID {temp.DoctorId} has a null or empty FullName.");
                DoctorName = "Name Not Set";
            }
            else
            {
                DoctorName = doctor.FullName;
            }

            List<VisitMedicine> BillItems = context.VisitMedicine.Where(a => a.PatientVisitId == temp.PatientVisitId).ToList();

            string billDate = "";
            
            if (temp.CreateDate != null )
            {
                billDate= temp.CreateDate.Value.ToString("yyyy-MM-dd HH:mm");

            }

            double? totalCost = temp.ServiceCost + temp.MedicnieCost;
            double?  discountValue = (temp.ServiceCost + temp.MedicnieCost) * temp.Discount / 100;
            double? totalCostWithDiscount = totalCost - discountValue;

            string BillFile = FileHeader;

            string logoAndName = $"    <center>\r\n        <img src='{ComapnyLogo}' />\r\n        <br />\r\n        <h1>{BillCompanyName}</h1>\r\n    </center>";

            BillFile += logoAndName;
            BillFile += "<br/>";

            string BillInfo = $"<div style=\"margin:5px\">\r\n" +
                $"    <table>\r\n " +
                $"       <tr style=\"    border-bottom: 1px solid gray;\">\r\n   " +
                $"         <td>\r\n       " +
                $"         <h2>Bill Number:</h2>\r\n  " +
                $"          </td>\r\n      " +
                $"      <td >\r\n    " +
                $"            <h3>{BillNumber}</h3>\r\n" +
                $"            </td>\r\n" +
                $"            <td style=\"padding-left:5px;padding-right:5px\">\r\n" +
                $"                -\r\n     " +
                $"       </td>\r\n " +
                $"           <td>\r\n" +
                $"                <h2>Bill Date:</h2>\r\n\r\n" +
                $"            </td>\r\n   " +
                $"         <td colspan=\"2\">\r\n " +
                $"               <h3>{billDate}</h3>\r\n " +
                $"           </td>\r\n    " +
                $"    </tr>\r\n     " +
                $"   <tr style=\"    border-bottom: 1px solid gray;\">\r\n\r\n " +
                $"           <td colspan=\"2\">\r\n     " +
                $"           <h2>Client:</h2>\r\n       " +
                $"     </td>\r\n     " +
                $"       <td colspan=\"4\">\r\n   " +
                $"             <h3>{ClientName}</h3>\r\n" +
                $"            </td>\r\n    " +
                $"    </tr>\r\n\r\n      " +
                $"  <tr style=\"    border-bottom: 1px solid gray;\">\r\n\r\n  " +
                $"          <td colspan=\"2\">\r\n           " +
                $"     <h2>Clinic:</h2>\r\n       " +
                $"     </td>\r\n          " +
                $"  <td colspan=\"4\">\r\n      " +
                $"          <h3>{ClinicName}</h3>\r\n  " +
                $"          </td>\r\n " +
                $"       </tr>\r\n  " +
                $"      <tr style=\"    border-bottom: 1px solid gray;\">\r\n\r\n    " +
                $"        <td colspan=\"2\">\r\n     " +
                $"           <h2>Service:</h2>\r\n   " +
                $"         </td>\r\n      " +
                $"      <td colspan=\"4\">\r\n   " +
                $"             <h3>{ServiceName}</h3>\r\n  " +
                $"          </td>\r\n     " +
                $"   </tr>\r\n       " +
                $" <tr style=\"    border-bottom: 1px solid gray;\">\r\n\r\n   " +
                $"         <td colspan=\"2\">\r\n       " +
                $"         <h2>Doctor:</h2>\r\n   " +
                $"         </td>\r\n        " +
                $"    <td colspan=\"4\">\r\n      " +
                $"          <h3>{DoctorName}</h3>\r\n  " +
                $"          </td>\r\n       " +
                $" </tr>\r\n   " +
                $" </table>\r\n   " +
                $" \r\n</div>";


            BillFile += BillInfo;
            BillFile += "<br/>";


            string BillCost = $"\r\n<div style=\"margin:5px\">\r\n   " +
                $" <table>\r\n\r\n    " +
                $"    <tr>\r\n      " +
                $"      <td>\r\n     " +
                $"           <h2>Service Cost:</h2>\r\n    " +
                $"        </td>\r\n       " +
                $"     <td colspan=\"3\">\r\n    " +
                $"            <h3>{temp.ServiceCost}</h3>\r\n   " +
                $"         </td>\r\n     " +
                $"   </tr>      \r\n   " +
                $"     <tr>\r\n     " +
                $"       <td>\r\n     " +
                $"           <h2>Medicine Cost:</h2>\r\n       " +
                $"     </td>\r\n      " +
                $"      <td colspan=\"3\">\r\n       " +
                $"         <h3>{temp.MedicnieCost}</h3>\r\n   " +
                $"         </td>\r\n    " +
                $"    </tr>\r\n\r\n   " +
                $"     <tr>\r\n     " +
                $"       <td>\r\n      " +
                $"          <h2>Total Cost:</h2>\r\n " +
                $"           </td>\r\n     " +
                $"       <td colspan=\"3\">\r\n       " +
                $"         <h3>{totalCost}</h3>\r\n  " +
                $"          </td>\r\n\r\n      " +
                $"  </tr>\r\n\r\n    " +
                $"    <tr>\r\n     " +
                $"       <td>\r\n   " +
                $"             <h2>Discount:</h2>\r\n    " +
                $"        </td>\r\n     " +
                $"       <td colspan=\"3\">\r\n  " +
                $"              <h3>{temp.Discount}</h3>\r\n   " +
                $"         </td>\r\n\r\n    " +
                $"    </tr>\r\n\r\n\r\n    " +
                $"    <tr>\r\n         " +
                $"   <td>\r\n              " +
                $"  <h2>Required Amount to Pay:</h2>\r\n    " +
                $"        </td>\r\n      " +
                $"      <td colspan=\"3\">\r\n     " +
                $"           <h3>{totalCostWithDiscount}</h3>\r\n   " +
                $"         </td>\r\n\r\n     " +
                $"   </tr>\r\n\r\n\r\n   " +
                $" </table>\r\n" +
                $"\r\n</div>";


            BillFile += BillCost;
            BillFile += "<br/>";



            FileBody = @$"<body dir='LTR'>{BillFile}</body></html>";

        }
    }
}
