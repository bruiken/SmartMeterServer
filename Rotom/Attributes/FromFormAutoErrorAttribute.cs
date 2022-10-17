using Microsoft.AspNetCore.Mvc;

namespace Rotom.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromFormAutoErrorAttribute : FromFormAttribute
    {
        public string? ViewName { get; set; }
    }
}
