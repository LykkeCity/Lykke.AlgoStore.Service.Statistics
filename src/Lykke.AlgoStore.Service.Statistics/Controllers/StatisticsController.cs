using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Service.Statistics.Controllers
{
    [Authorize]
    [Route("api/v1/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpPost("updateSummary")]
        [SwaggerOperation("UpdateSummary")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateSummary(string clientId, string instanceId)
        {
            await _service.UpdateSummaryAsync(clientId, instanceId);

            return NoContent();
        }

        [HttpPost("increaseTotalTrades")]
        [SwaggerOperation("IncreaseTotalTrades")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> IncreaseTotalTrades()
        {
            await _service.IncreaseTotalTradesAsync(User.GetInstanceData().InstanceId);

            return NoContent();
        }
    }
}
