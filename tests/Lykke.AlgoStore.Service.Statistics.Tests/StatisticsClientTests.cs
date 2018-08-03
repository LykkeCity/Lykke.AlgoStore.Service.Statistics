using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.Statistics.Client;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Statistics.Tests
{
    public class StatisticsClientTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IStatisticsClient _client;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            //REMARK: Must use auth token for existing algo instance
            var authHandler = new AlgoAuthorizationHeaderHttpClientHandler("fc362170-e7ab-46d0-8913-143b946a04a7");

            _client = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl("http://localhost:5000")
                .WithAdditionalDelegatingHandler(authHandler)
                .Create()
                .Generate<IStatisticsClient>();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void UpdateSummaryAsync_Test()
        {
            //REMARK: Must use existing client and instance ids
            var clientId = "637ed5dd-54e2-42fa-bcfd-0454fb54f761";
            var instanceId = "b1acb6b4-55fc-4f83-8804-281a0b1e95a3";

            _client.UpdateSummaryAsync(clientId, instanceId).Wait();
        }
    }
}
