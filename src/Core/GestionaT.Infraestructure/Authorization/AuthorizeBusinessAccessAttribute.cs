using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Infraestructure.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeBusinessAccessAttribute : TypeFilterAttribute
    {
        public AuthorizeBusinessAccessAttribute(string routeParameterName)
            : base(typeof(BusinessAccessFilter))
        {
            Arguments = new object[] { routeParameterName };
        }
    }
}
