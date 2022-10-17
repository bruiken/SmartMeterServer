using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rotom.Attributes
{
    public class AttachErrorFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult vr && vr.Model is Models.ErrorViewModel evm)
            {
                if (context.HttpContext.Request.Cookies.ContainsKey("ErrorTitle") && context.HttpContext.Request.Cookies.ContainsKey("ErrorDetail"))
                {
                    evm.ErrorTitle = context.HttpContext.Request.Cookies["ErrorTitle"].ToString();
                    evm.ErrorDetail = context.HttpContext.Request.Cookies["ErrorDetail"].ToString();

                    context.HttpContext.Response.Cookies.Delete("ErrorTitle");
                    context.HttpContext.Response.Cookies.Delete("ErrorDetail");
                }
            }
        }
    }
}
