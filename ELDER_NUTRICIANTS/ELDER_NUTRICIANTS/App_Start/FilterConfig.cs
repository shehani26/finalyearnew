using System.Web;
using System.Web.Mvc;

namespace ELDER_NUTRICIANTS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
