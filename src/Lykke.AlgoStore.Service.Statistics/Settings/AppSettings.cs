using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;

namespace Lykke.AlgoStore.Service.Statistics.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public StatisticsSettings AlgoStoreStatisticsService { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public BalancesServiceClientSettings BalancesServiceClient { get; set; }
        public RateCalculatorClientSettings RateCalculatorServiceClient { get; set; }
    }
}
