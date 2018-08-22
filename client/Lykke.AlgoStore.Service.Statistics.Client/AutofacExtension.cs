using Autofac;
using System;

namespace Lykke.AlgoStore.Service.Statistics.Client
{
    public static class AutofacExtension
    {
        public static void RegisterStatisticsClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            var statisticsService = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl(serviceUrl);

            builder.RegisterInstance(statisticsService.Create().Generate<IStatisticsClient>())
                .As<IStatisticsClient>().SingleInstance();
        }
    }
}
