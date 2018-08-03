using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.Statistics.Client 
{
    /// <summary>
    /// Statistics client settings.
    /// </summary>
    public class StatisticsServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
