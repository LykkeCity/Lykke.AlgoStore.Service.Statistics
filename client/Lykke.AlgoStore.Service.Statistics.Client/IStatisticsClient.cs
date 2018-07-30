﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace Lykke.AlgoStore.Service.Statistics.Client
{
    /// <summary>
    /// Statistics client interface.
    /// </summary>
    [PublicAPI]
    [Headers("Authorization")]
    public interface IStatisticsClient
    {
        [Post("/api/v1/statistics/updateSummary")]
        Task UpdateSummaryAsync(string clientId, string instanceId);
    }
}
