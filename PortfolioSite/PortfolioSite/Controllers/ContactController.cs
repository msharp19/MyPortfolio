using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using System.Configuration;
using PortfolioSite.Models;
using System.Web.Routing;

namespace PortfolioSite.Controllers
{
    public class ContactController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact(string email, string name, string message)
        {
            var model = new ContactModel()
            {
                PhoneNumber = ConfigurationManager.AppSettings["PhoneNumber"],
                Email = ConfigurationManager.AppSettings["Email"],
                GoogleMapsAPIKey = ConfigurationManager.AppSettings["GoogleMapsAPIKey"],
                FormEmail = email,
                FormName = name,
                FormMessage = message
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Message(string email, string name, string message, string captcha)
        {
            if (this.IsCaptchaValid(captcha))
                return new JsonResult(){
                    Data = new { Status="Success", Message = "Thanks for the message!" },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet };
            //Failed captcha
            var dictionary = new RouteValueDictionary() {  };
            dictionary.Add("email", email);
            dictionary.Add("name", name);
            dictionary.Add("message", message);
            return new JsonResult() {
                Data = new { Status = "Failure", Message = "You any good at maths?", Url= Url.Action("Contact", "Contact", dictionary, Request.Url.Scheme) },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
    }
}