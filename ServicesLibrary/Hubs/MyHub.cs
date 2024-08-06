using CmsDataAccess;
using CmsDataAccess.Models;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ServicesLibrary.UserServices;

namespace CmsWeb.Hubs
{
    public class MyHub:Hub
    {


        // connectionId: from the hub
        // clientId: from the cookie
        // user: is the username
        // message is the message body 

        
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;



        private static Dictionary<string, List<string>> groupConnections = new Dictionary<string, List<string>>();


        public MyHub(
            IEmailSender emailSender,
            IConfiguration config,
            IUserService userService

            )
        {
            _userService = userService;
            
            _emailSender = emailSender;
            _config = config;
        }


        public async Task RefreshNewApplicationCount(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshNewApplicationCount_", "ghj");
        }

        public async Task RefreshNewOrderCount(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshNewOrderCount_", "ghj");
        }

        

        public async Task RefreshClinicStatus(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshClinicStatus_", "ghj");
        }    
        
        public async Task RefreshBillStatus(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshBillStatus_", "ghj");
        }


        
        public async Task AddToGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        }


        public async Task RefreshAdminOrderGrid(string? blabla="blabla")
        {
            await Clients.All.SendAsync("RefreshAdminOrderGrid_", "ghj"); 
        }



        public async Task RefreshEmployeeOrderGrid(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshEmployeeOrderGrid_", "ghj");
        }


        public async Task RefreshAdminChatGrid(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshAdminChatGrid_", "ghj");
        }


        public async Task RefreshEmployeeChatGrid(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshEmployeeChatGrid_", "ghj");
        }

        public async Task RefreshEmployeeNotiList(string? blabla = "blabla")
        {
            await Clients.All.SendAsync("RefreshEmployeeNotiList_", "ghj");
        }

        public async Task RemoveGroubFromHub(string groupName)
        {

            if (groupConnections.ContainsKey(groupName))
            {

                foreach (var item in groupConnections[groupName])
                {
                    await Groups.RemoveFromGroupAsync(item, groupName);
                    groupConnections[groupName].Remove(item);
                }

                
                if (groupConnections[groupName].Count == 0)
                {
                    groupConnections.Remove(groupName);
                }
            }
        }




    }
}
