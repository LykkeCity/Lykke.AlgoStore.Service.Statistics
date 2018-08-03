using System;
using JetBrains.Annotations;
using Lykke.AlgoStore.Security.InstanceAuth;

namespace Lykke.AlgoStore.Service.Statistics.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class StatisticsSettings
    {
        public DbSettings Db { get; set; }
        public InstanceAuthSettings StatisticsServiceAuth { get; set; }
        public DictionariesSettings Dictionaries { get; set; }
    }

    public class DictionariesSettings
    {
        public TimeSpan CacheExpirationPeriod { get; set; }
    }
}
