﻿using PortfolioSite.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioSite.Controllers
{
    [AllowAnonymous]
    public class WorkController : Controller
    {
        [HttpGet]
        public ActionResult Project(Project project)
        {
            return View(project.ToString());
        }

    }
}