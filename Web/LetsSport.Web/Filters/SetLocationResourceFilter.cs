namespace LetsSport.Web.Filters
{
    using System;

    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class SetLocationResourceFilter : Attribute, IResourceFilter
    {
        private readonly string city = "city";
        private readonly string country = "country";
        private readonly ILocationLocator locationLocator;

        public SetLocationResourceFilter(ILocationLocator locationLocator)
        {
            this.locationLocator = locationLocator;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var (country, city) = this.locationLocator.GetLocationInfo(ip);

            context.HttpContext.Session.SetString(this.city, city);
            context.HttpContext.Session.SetString(this.country, country);
        }
    }
}
