using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebCenter.Web.Filters
{
    public class JsonAttribute:ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string contentType = filterContext.HttpContext.Response.ContentType;
            if (contentType == "application/json")
                filterContext.HttpContext.Response.ContentType = "text/html";
            base.OnResultExecuted(filterContext);
        }
    }
}