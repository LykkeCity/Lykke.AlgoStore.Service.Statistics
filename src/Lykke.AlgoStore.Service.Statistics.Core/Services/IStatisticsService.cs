using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.Statistics.Core.Services
{
    public interface IStatisticsService
    {
        Task<StatisticsSummary> UpdateSummaryAsync(string clientId, string instanceId);
        Task IncreaseTotalTradesAsync(string instanceId);
    }
}
