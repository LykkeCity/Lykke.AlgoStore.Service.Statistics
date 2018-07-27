using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.RateCalculator.Client;

namespace Lykke.AlgoStore.Service.Statistics.Services
{
    public class WalletBalanceService : IWalletBalanceService
    {
        private readonly IRateCalculatorClient _rateCalculator;
        private readonly IBalancesClient _balancesClient;

        public WalletBalanceService(IRateCalculatorClient rateCalculator, IBalancesClient balancesClient)
        {
            _rateCalculator = rateCalculator;
            _balancesClient = balancesClient;
        }

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetWalletBalancesAsync(string walletId,
            AssetPair assetPair)
        {
            var clientBalances = await _balancesClient.GetClientBalances(walletId);

            var clientBalanceResponseModels = clientBalances.ToList();

            return clientBalanceResponseModels.Where(b => b.AssetId == assetPair.BaseAssetId || b.AssetId == assetPair.QuotingAssetId);
        }

        public async Task<double> GetTotalWalletBalanceInBaseAssetAsync(string walletId, string baseAssetId,
            AssetPair assetPair)
        {
            decimal totalWalletBalance = 0;

            var balances = await GetWalletBalancesAsync(walletId, assetPair);
            var clientBalanceResponseModels = balances.ToList();

            foreach (var balance in clientBalanceResponseModels)
            {
                if (balance.AssetId == baseAssetId)
                    totalWalletBalance += balance.Balance;
                else
                {
                    var assetBalanceInBase = await _rateCalculator.GetAmountInBaseAsync(balance.AssetId, (double)balance.Balance, baseAssetId);
                    totalWalletBalance += (decimal)assetBalanceInBase;
                }
            }

            if (totalWalletBalance == 0)
            {
                throw new ValidationException($"Initial wallet balance could not be calculated for wallet {walletId}");
            }

            return (double)totalWalletBalance;
        }
    }
}
