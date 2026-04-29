using System.Web;
using System.Web.Mvc;

namespace Gestion_de_biblotecas_vFinal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
