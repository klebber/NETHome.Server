using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
