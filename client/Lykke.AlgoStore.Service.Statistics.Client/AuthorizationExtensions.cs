namespace Lykke.AlgoStore.Service.Statistics.Client
{
    public static class AuthorizationExtensions
    {
        public static string ToBearerToken(this string instanceAuthToken)
        {
            return instanceAuthToken.StartsWith("Bearer") ? instanceAuthToken : $"Bearer {instanceAuthToken}";
        }
    }
}
