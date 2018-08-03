using System;
using Autofac;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Lykke.AlgoStore.Service.Statistics.Services;
using Lykke.AlgoStore.Service.Statistics.Settings;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.RateCalculator.Client;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Service.Statistics.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            var reloadingDbManager =
                _appSettings.ConnectionString(x => x.AlgoStoreStatisticsService.Db.DataStorageConnectionString);

            builder.Register(x =>
                {
                    var log = x.Resolve<ILogFactory>().CreateLog(this);

                    var repository = AzureRepoFactories.CreateAlgoClientInstanceRepository(reloadingDbManager, log);

                    return repository;
                })
                .As<IAlgoClientInstanceRepository>()
                .SingleInstance();

            builder.Register(x =>
                {
                    var log = x.Resolve<ILogFactory>().CreateLog(this);

                    var repository = AzureRepoFactories.CreateStatisticsRepository(reloadingDbManager, log);

                    return repository;
                })
                .As<IStatisticsRepository>()
                .SingleInstance();

            builder.RegisterType<StatisticsService>()
                .As<IStatisticsService>()
                .SingleInstance();

            builder.RegisterType<WalletBalanceService>()
                .As<IWalletBalanceService>()
                .SingleInstance();

            builder.RegisterAssetsClient(
                AssetServiceSettings.Create(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl),
                    _appSettings.CurrentValue.AlgoStoreStatisticsService.Dictionaries.CacheExpirationPeriod)
            );
            builder.RegisterBalancesClient(_appSettings.CurrentValue.BalancesServiceClient);
            builder.RegisterRateCalculatorClient(_appSettings.CurrentValue.RateCalculatorServiceClient.ServiceUrl);
        }
    }
}
