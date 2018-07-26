using Lykke.HttpClientGenerator;

namespace Lykke.Service.Statistics.Client
{
    public class StatisticsClient : IStatisticsClient
    {
        //public IControllerApi Controller { get; }
        
        public StatisticsClient(IHttpClientGenerator httpClientGenerator)
        {
            //Controller = httpClientGenerator.Generate<IControllerApi>();
        }
        
    }
}
