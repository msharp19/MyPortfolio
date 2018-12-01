using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace TelegramMessenger
{
    public class MessageClient
    {
        public static void SendMessageAsync(string message)
        {
            var configReader = ConfigReader.GetInstance();
            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
            urlString = String.Format(urlString, $"{configReader.TelegramAPIID}:{configReader.TelegramAPIHash}",
                configReader.TelegramChannelTitle, message);
            WebRequest request = WebRequest.Create(urlString);
        }
    }
}
