using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace NSPC.Common
{
    public class ConfigCollection
    {
        private readonly IConfigurationRoot configuration;
        private readonly JObject translation;

        public static ConfigCollection Instance { get; } = new ConfigCollection();

        protected ConfigCollection()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.Production.json", optional: true, reloadOnChange: false)
                .Build();

            FrontendUrl = configuration["HostUrl"];
            StaticFiles_Folder = configuration["StaticFiles:Folder"];
            StaticFiles_Host = configuration["StaticFiles:Host"];
            ChatGPT_Token = configuration["ChatGPT:Token"];
            ChatGPT_ChatCompletionsUrl = configuration["ChatGPT:ChatCompletionsUrl"];
            ChatGPT_Model = configuration["ChatGPT:Model"];
            // Translation
            if (File.Exists(@"Resources/Translate/translate.json"))
                translation = JObject.Parse(File.ReadAllText(@"Resources/Translate/translate.json"));
        }

        public IConfigurationRoot GetConfiguration()
        {
            return configuration;
        }

        public string FrontendUrl { get; private set; }
        public string StaticFiles_Folder { get; private set; }
        public string StaticFiles_Host { get; private set; }
        public string ChatGPT_Token { get; private set; }
        public string ChatGPT_ChatCompletionsUrl { get; private set; }
        public string ChatGPT_Model { get; private set; }
        public JObject GetTranslation()
        {
            return translation;
        }
    }
}
