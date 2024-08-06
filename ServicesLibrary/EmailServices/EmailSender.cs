using Azure.Core;
using CmsDataAccess.DbModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ServicesLibrary.EmailServices.EmailServices
{
	public class EmailSender : IEmailSender
	{

		private readonly IConfiguration _config;

		public EmailSender(IConfiguration config)
		{
			_config = config;
		}

		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{

			ApplicationDbContext applicationDbContext = new ApplicationDbContext();

			MySystemConfiguration mySystemConfiguration = applicationDbContext.MySystemConfiguration.FirstOrDefault();

			MailMessage msg = null;

			SmtpClient client = new SmtpClient
			{
				Port = int.Parse(mySystemConfiguration.EmailPort),
				Host = mySystemConfiguration.EmailHost,
				//EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(mySystemConfiguration.EmailUsername,mySystemConfiguration.EmailPassword)
			};

			msg = new MailMessage(mySystemConfiguration.EmailUsername, email, subject, htmlMessage);

			msg.IsBodyHtml = true;

			return client.SendMailAsync(msg);
		}

	}
}
