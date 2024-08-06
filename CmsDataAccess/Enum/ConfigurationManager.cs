using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.Enum
{
    public class ConfigurationManager
    {
        private static IConfigurationRoot Configuration { get; }

        static ConfigurationManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Adjust the path accordingly
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public static string ApiUrl => Configuration["ApiUrl"];
    }
}
