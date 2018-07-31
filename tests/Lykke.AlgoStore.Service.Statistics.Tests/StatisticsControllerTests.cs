using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.Statistics.Controllers;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Statistics.Tests
{
    [TestFixture]
    public class StatisticsControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private StatisticsController _controller;
        private Mock<IStatisticsService> _statisticsServiceMock;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                //cfg.AddProfile<Services.AutoMapperProfile>();
                //cfg.AddProfile<AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            _statisticsServiceMock = new Mock<IStatisticsService>();
            _statisticsServiceMock.Setup(x => x.UpdateSummaryAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(_fixture.Build<StatisticsSummary>().Create()));

            _controller = new StatisticsController(_statisticsServiceMock.Object);
        }

        [Test]
        public void UpdateSummary_WillReturnCorrectResult_Test()
        {
            var result = _controller.UpdateSummary(It.IsAny<string>(), It.IsAny<string>()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
