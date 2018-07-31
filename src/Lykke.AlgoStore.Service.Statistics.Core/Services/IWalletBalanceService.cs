using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;

namespace Lykke.AlgoStore.Service.Statistics.Core.Services
{
    public interface IWalletBalanceService
    {
        Task<IEnumerable<ClientBalanceResponseModel>> GetWalletBalancesAsync(string walletId, AssetPair assetPair);
        Task<double> GetTotalWalletBalanceInBaseAssetAsync(string walletId, string baseAssetId, AssetPair assetPair);
    }
}
