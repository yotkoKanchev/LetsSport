namespace LetsSport.Web.Filters
{
    using System;

    using LetsSport.Common;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class SetLocationResourceFilter : Attribute, IResourceFilter
    {
        private readonly ILocationLocator locationLocator;

        // made for learning purpose
        // not used anywere -> replaced by SetLocation middleware
        public SetLocationResourceFilter(ILocationLocator locationLocator)
        {
            this.locationLocator = locationLocator;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Session.GetString(GlobalConstants.Country) == null
                || context.HttpContext.Session.GetString(GlobalConstants.City) == null)
            {
                var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var (country, city) = this.locationLocator.GetLocationInfo(ip);

                context.HttpContext.Session.SetString(GlobalConstants.City, city);
                context.HttpContext.Session.SetString(GlobalConstants.Country, country);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
