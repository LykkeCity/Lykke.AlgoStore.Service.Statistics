using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Lykke.AlgoStore.Service.Statistics.Services.Strings;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.AlgoStore.Service.Statistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;
        private readonly IAssetsServiceWithCache _assetsService;
        private readonly IWalletBalanceService _walletBalanceService;

        public StatisticsService(IStatisticsRepository statisticsRepository,
            IAlgoClientInstanceRepository algoClientInstanceRepository,
            IAssetsServiceWithCache assetsService,
            IWalletBalanceService walletBalanceService)
        {
            _statisticsRepository = statisticsRepository;
            _algoInstanceRepository = algoClientInstanceRepository;
            _assetsService = assetsService;
            _walletBalanceService = walletBalanceService;
        }

        public async Task<StatisticsSummary> UpdateSummaryAsync(string clientId, string instanceId)
        {
            ValidateInstanceId(instanceId);
            ValidateClientId(clientId);

            var statisticsSummary = await _statisticsRepository.GetSummaryAsync(instanceId);
            if (statisticsSummary == null)
            {
                throw new ValidationException($"Could not find statistic summary row for AlgoInstance: {instanceId}");
            }

            var algoInstance = await _algoInstanceRepository.GetAlgoInstanceDataByClientIdAsync(clientId, instanceId);
            if (algoInstance?.AlgoId == null)
            {
                throw new ValidationException(
                    $"Could not find AlgoInstance with InstanceId {instanceId} and ClientId {clientId}");
            }

            var assetPairResponse = await _assetsService.TryGetAssetPairAsync(algoInstance.AssetPairId);
            ValidateAssetPair(algoInstance.AssetPairId, assetPairResponse);

            var tradedAsset = await _assetsService.TryGetAssetAsync(algoInstance.IsStraight
                ? assetPairResponse.BaseAssetId
                : assetPairResponse.QuotingAssetId);
            ValidateAssetResponse(tradedAsset);

            if (algoInstance.AlgoInstanceType == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Live)
            {
                var walletBalances = await _walletBalanceService.GetWalletBalancesAsync(algoInstance.WalletId, assetPairResponse);
                var clientBalanceResponseModels = walletBalances.ToList();
                var latestWalletBalance = await _walletBalanceService.GetTotalWalletBalanceInBaseAssetAsync(
                    algoInstance.WalletId, statisticsSummary.UserCurrencyBaseAssetId, assetPairResponse);

                statisticsSummary.LastTradedAssetBalance = (double) (clientBalanceResponseModels.FirstOrDefault(b => b.AssetId == tradedAsset.Id)?.Balance ?? 0);
                statisticsSummary.LastAssetTwoBalance = (double) (clientBalanceResponseModels.FirstOrDefault(b => b.AssetId != tradedAsset.Id)?.Balance ?? 0);
                statisticsSummary.LastWalletBalance = latestWalletBalance;
            }

            statisticsSummary.NetProfit = statisticsSummary.InitialWalletBalance.Equals(0.0) ? 0 : Math.Round(
                ((statisticsSummary.LastWalletBalance - statisticsSummary.InitialWalletBalance) /
                 statisticsSummary.InitialWalletBalance) * 100, 2, MidpointRounding.AwayFromZero);

            await _statisticsRepository.CreateOrUpdateSummaryAsync(statisticsSummary);

            return statisticsSummary;
        }

        private static void ValidateAssetResponse(Asset assetResponse)
        {
            if (assetResponse == null)
                throw new ValidationException("Asset was not found");
        }

        private static void ValidateAssetPair(string assetPairId, AssetPair assetPair)
        {
            if (assetPair == null)
                throw new ValidationException($"AssetPair: {assetPairId} was not found");

            if (assetPair.IsDisabled)
                throw new ValidationException($"AssetPair {assetPairId} is temporarily disabled");
        }

        private static void ValidateInstanceId(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ValidationException(Phrases.InstanceIdCannotBeEmpty);
        }

        private static void ValidateClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ValidationException(Phrases.ClientIdCannotBeEmpty);
        }
    }
}
