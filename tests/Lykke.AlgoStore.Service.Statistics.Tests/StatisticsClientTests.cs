using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.Statistics.Client;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Statistics.Tests
{
    public class StatisticsClientTests
    {
        private readonly string _authToken = "0e72f8d0-ed37-4e59-87e8-977e56e88f02";
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

            _client = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl("http://localhost:5000")
                .Create()
                .Generate<IStatisticsClient>();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void UpdateSummaryAsync_Test()
        {
            //REMARK: Must use existing client and instance ids
            var clientId = "71a4471d-7b4f-4264-93c6-235fd1eda4ad";
            var instanceId = "0d09e70c-0bbc-461d-bee5-adca9b4ecc69";

            _client.UpdateSummaryAsync(clientId, instanceId, _authToken.ToBearerToken()).Wait();
        }
    }
}
