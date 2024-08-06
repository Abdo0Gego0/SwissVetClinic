using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;


namespace NoificationManager.MobileModels
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel); 
    }

    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _fcmNotificationSetting = settings.Value;
        }

        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                //var message = new
                //{
                //    token = notificationModel.Token.ToString(),
                //    data=new
                //    {
                //        body = notificationModel.Body,
                //        title = notificationModel.Title,
                //    }
                //};
                //SendNotification_(message);


                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("mythclinicfb-firebase-adminsdk-phe4b-014e20d881.json"),
                });

                var registrationTokens = new List<string>()
{
    "YOUR_REGISTRATION_TOKEN_1",
    // ...
    "YOUR_REGISTRATION_TOKEN_n",
};

                var message = new Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Body",notificationModel.Body },
                        { "Title", notificationModel.Title },
                    },
                    Token = notificationModel.Token,
                };

                string responseText = await FirebaseMessaging.DefaultInstance.SendAsync(message);


                response.IsSuccess = true;
                response.Message = responseText;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
                return response;
            }
        }

        public void SendNotification_(object data)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data); 
            Byte[] byteArray=Encoding.UTF8.GetBytes(json);
            SendNotification_(byteArray);
        }
        public void SendNotification_(Byte[] byteArray)
        {
            try
            {
                string serverKey = _fcmNotificationSetting.ServerKey;
                string senderId = _fcmNotificationSetting.SenderId;

                //WebRequest request = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                WebRequest request = WebRequest.Create("https://fcm.googleapis.com/v1/projects/myproject-b5ae1/messages:send");
                request.Method = "POST";
                request.ContentType = "application/json";
                //request.Headers.Add($"Authorization: key=={serverKey}");
                request.Headers.Add($"Authorization: Bearer=={serverKey}");
                request.Headers.Add($"Sender: id=={serverKey}");

                request.ContentLength = byteArray.Length;
                Stream datastream= request.GetRequestStream();
                datastream.Write( byteArray, 0, byteArray.Length ); 
                datastream.Close();

                WebResponse response = request.GetResponse();    
                datastream = response.GetResponseStream();

                StreamReader streamReader = new StreamReader (datastream );

                string responseFromServer= streamReader.ReadToEnd();

                streamReader.Close();
                datastream.Close ();
                response.Close();


            }
            catch (Exception)
            {

                throw;
            }

        }

    }


}
