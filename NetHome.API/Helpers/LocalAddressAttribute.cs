using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;

namespace NetHome.API.Helpers
{
    public class LocalAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IPAddress ip = context.HttpContext.Connection.RemoteIpAddress;
            byte[] ipbytes = ip.GetAddressBytes();
            if (ipbytes[0] != 192 || ipbytes[1] != 168)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
