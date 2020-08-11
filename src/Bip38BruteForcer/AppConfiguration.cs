using System.Configuration;

namespace Bip38BruteForcer
{
    public class AppConfiguration
    {
        public string PublicAddress { get; private set; }

        public string EncryptedPrivateKey { get; private set; }

        public string[] DictionaryFiles { get; private set; }

        public int NumberOfWorkers { get; private set; }

        public int ReportInterval { get; private set; }

        private AppConfiguration()
        { }

        public static AppConfiguration Load()
        {
            return new AppConfiguration()
            { 
                PublicAddress = ConfigurationManager.AppSettings["PublicAddress"],
                EncryptedPrivateKey = ConfigurationManager.AppSettings["EncryptedPrivateKey"],
                DictionaryFiles = ConfigurationManager.AppSettings["DictionaryFiles"].Split(','),
                NumberOfWorkers = int.Parse(ConfigurationManager.AppSettings["NumberOfWorkers"]),
                ReportInterval = int.Parse(ConfigurationManager.AppSettings["ReportInterval"])
            };
        }
    }
}