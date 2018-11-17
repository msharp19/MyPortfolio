using PortfolioSite.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioSite.Controllers
{
    public class WorkController : Controller
    {
        // GET: Work
        public ActionResult Project(Project project)
        {
            return View(project.ToString());
        }
    }
}