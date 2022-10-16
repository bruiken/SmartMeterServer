using Microsoft.AspNetCore.Mvc;

namespace SmartMeterServer.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromFormAutoErrorAttribute : FromFormAttribute
    {
        public string? ViewName { get; set; }
    }
}
