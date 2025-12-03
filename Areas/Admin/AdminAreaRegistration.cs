using System.Web.Mvc;

namespace DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "DoAn_WebBanCayCanh_24DKTPM1A.Areas.Admin.Controllers" }
            );
        }
    }
}