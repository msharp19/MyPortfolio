using Microsoft.AspNet.SignalR;
using PortfolioSite.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Utils
{
    public class Functions
    {
        public static bool SendProgress(string connectionId, string progressMessage)
        {
            //IN ORDER TO INVOKE SIGNALR FUNCTIONALITY DIRECTLY FROM SERVER SIDE WE MUST USE THIS
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ANNHub>();
            //PUSHING DATA TO ALL CLIENTS
            hubContext.Clients.Client(connectionId).AddProgress(progressMessage);
            return true;
        }
    }
}