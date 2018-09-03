using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace Lykke.AlgoStore.Service.Statistics.Client
{
    /// <summary>
    /// Statistics client interface.
    /// </summary>
    [PublicAPI]
    public interface IStatisticsClient
    {
        [Post("/api/v1/statistics/updateSummary")]
        Task UpdateSummaryAsync(string clientId, string instanceId, [Header("Authorization")] string instanceAuthToken);

        [Post("/api/v1/statistics/increaseTotalTrades")]
        Task IncreaseTotalTradesAsync([Header("Authorization")] string instanceAuthToken);
    }
}
