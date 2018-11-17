using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Models
{
    public class ContactModel
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GoogleMapsAPIKey { get; set; }
        public string FormEmail { get; set; }
        public string FormName  { get; set; }
        public string FormMessage { get; set; }
    }
}