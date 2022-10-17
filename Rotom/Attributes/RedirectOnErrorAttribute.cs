namespace Rotom.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RedirectOnErrorAttribute : Attribute
    {
        public string Action { get; set; }

        public string Controller { get; set; }
    }
}
