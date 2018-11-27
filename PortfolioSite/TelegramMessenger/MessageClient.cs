using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using Utils;

namespace TelegramMessenger
{
    public class MessageClient
    {
        public async static void SendMessage(string message)
        {
            var configReader = ConfigReader.GetInstance();
            var client = new TelegramClient(configReader.TelegramAPIID, configReader.TelegramAPIHash);
            await client.ConnectAsync();
            //get user dialogs
            var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
            //find channel by title
            var chat = dialogs.Chats
              .Where(c => c.GetType() == typeof(TLChannel))
              .Cast<TLChannel>()
              .FirstOrDefault(c => c.Title == configReader.TelegramChannelTitle);
            //send message
            await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = chat.Id, AccessHash = chat.AccessHash.Value }, message);
        }
    }
}
