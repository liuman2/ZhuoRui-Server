using System.Web;
using System.Web.Mvc;
using WebCenter.Web.Filters;

namespace WebCenter.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new JsonAttribute());
          
            filters.Add(new HandleErrorAttribute());
        }
    }
}
