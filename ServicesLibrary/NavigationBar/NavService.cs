using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.NavigationBar
{
    public interface INavService
    {
        string GetStylGetStyleMainNav(List<string> controllers, List<string> actions, string activeStyle, string inactiveStyle);

        bool IsNavItemSelected(List<string> controllers, List<string> actions);

        List<string> GetSiteMap();
    }
    public class NavService : INavService
    {
        private readonly IActionContextAccessor _actionContextAccessor;

        public NavService(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        public string GetStylGetStyleMainNav(List<string> controllers, List<string> actions, string activeStyle, string inactiveStyle)
        {
            var routeData = _actionContextAccessor.ActionContext.RouteData.Values;
            var controller = routeData["controller"].ToString();
            var action = routeData["action"].ToString();

            foreach (var ctrl in controllers)
            {
                if (ctrl == controller)
                {
                    if (actions.Contains(action))
                        return activeStyle;
                }
            }

            return inactiveStyle;
        }

        public bool IsNavItemSelected(List<string> controllers, List<string> actions)
        {
            var routeData = _actionContextAccessor.ActionContext.RouteData.Values;
            var controller = routeData["controller"].ToString();
            var action = routeData["action"].ToString();

            foreach (var ctrl in controllers)
            {
                if (ctrl == controller)
                {
                    if (actions.Contains(action))
                        return true;
                }
            }
            return false;
        }


        public List<string> GetSiteMap()
        {
            var routeData = _actionContextAccessor.ActionContext.RouteData.Values;
            string area = routeData["area"].ToString();
            string controller = routeData["controller"].ToString();
            string action = routeData["action"].ToString();

            return new List<string> { area, controller, action };
        }


    }
}
