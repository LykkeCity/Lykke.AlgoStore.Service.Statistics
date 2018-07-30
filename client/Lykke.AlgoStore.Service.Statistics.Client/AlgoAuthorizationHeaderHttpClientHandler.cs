using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.Statistics.Client
{
    public class AlgoAuthorizationHeaderHttpClientHandler : DelegatingHandler
    {
        private readonly string _token;

        public AlgoAuthorizationHeaderHttpClientHandler(string token)
        {
            _token = token.StartsWith("Bearer") ? token : $"Bearer {token}";
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation("Authorization", _token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
