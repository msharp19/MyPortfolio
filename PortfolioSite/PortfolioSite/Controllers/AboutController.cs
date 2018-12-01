using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioSite.Controllers
{
    [AllowAnonymous]
    public class AboutController : Controller
    {

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}