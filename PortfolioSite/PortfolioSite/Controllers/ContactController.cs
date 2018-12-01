using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using System.Configuration;
using PortfolioSite.Models;
using System.Web.Routing;
using CaptchaMvc.Attributes;
using CaptchaMvc.Infrastructure;
using TelegramMessenger;
using System.Threading.Tasks;

namespace PortfolioSite.Controllers
{
    [AllowAnonymous]
    public class ContactController : Controller
    {
        [HttpGet]
        public ActionResult Contact(string email, string name, string message)
        {
            CaptchaUtils.CaptchaManager.StorageProvider = new CookieStorageProvider();
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

        [HttpPost]
        public ActionResult Message(string email, string name, string message)
        {
            var isValid = this.IsCaptchaValid("Captcha is not valid");
            if (isValid)
            {
                MessageClient.SendMessageAsync($"From:{email}({name})-{message}");
                return new JsonResult()
                {
                    Data = new { Status = "Success", Message = "Thanks for the message!", Url = Url.Action("Contact", "Contact", null, Request.Url.Scheme) },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
            //Failed captcha
            var dictionary = new RouteValueDictionary() {  };
            dictionary.Add("email", email);
            dictionary.Add("name", name);
            dictionary.Add("message", message);
            return new JsonResult() {
                Data = new { Status = "Failure", Message = "Captcha is not correct.", Url= Url.Action("Contact", "Contact", dictionary, Request.Url.Scheme) },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
    }
}