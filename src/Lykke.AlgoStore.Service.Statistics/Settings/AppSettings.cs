using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.Statistics.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public StatisticsSettings AlgoStoreStatisticsService { get; set; }        
    }
}
