using Microsoft.AspNetCore.Http.Features.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.Enum
{
    public static class EmailMessages
    {
        public static string NewApplicationMessage(string baseUrl ,string token)
        {
            return $"<h1>Welcome to Myth medical platform</h1>" +
                $"<p>please click the following link to confirmyour application</p>" +
                $"<a href='{baseUrl}/Auth/Login/VerifyNewSubscriptionApplication/?token={token}' > Confirm my application</a>" +
                $"";
        }

        public static string NewAccountMessage(string baseUrl, string username, string password)
        {
            return $"<h1>Welcome to Myth medical platform</h1>" +
                $"<p>Your account credentials:</p>" +
                $"<h1>Username: {username}</h1>" +
                $"<h1>Password: {password}</h1>" +
                $"<p>Login url:</p>" +
                $"<a href={baseUrl}>Myth</a>" +
                $"";
        }

        public static string AcceptMessage(string msg)
        {
            return $"<h1>Welcome to Myth medical platform</h1>" +
                    $"<p>You application had been accepted</p>";
        }
        public static string AcceptSubject
        {
            get
            {
                return "You application had been accepted";
            }
        }
        public static string RejectMessage(string msg)
        {
            return $"<h1>Welcome to Myth medical platform</h1>" +
        $"<p>You application had been rejected</p>" +
        $"<p>{msg}</p>";
        }
        public static string RejectSubject
        {
            get
            {
                return "You application had been rejected";
            }
        }

    }
}
