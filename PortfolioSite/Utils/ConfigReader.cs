using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ConfigReader
    {
        /*  
        https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest
         */
        private static ConfigReader _reader { get; set; }
        private static KeyVaultClient _kv;

        public int TelegramAPIID => int.Parse(GetSecret("ProfileTelegramAPIID"));
        public string TelegramAPIHash => GetSecret("ProfileTelegramAPIHash");
        public string TelegramChannelTitle => GetSecret("ProfileTelegramChannelTitle");

        private ConfigReader()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            _kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public string GetSecret(string name)
        {
            var secret = _kv.GetSecretAsync(ConfigurationManager.AppSettings["KeyVaultURL"], name).Result;
            return secret.Value;
        }

        public static ConfigReader GetInstance()
        {
            if (_reader == null) _reader = new ConfigReader();
            return _reader;
        }
    }
}
