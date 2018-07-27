﻿using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Statistics.Core.Services;
using Lykke.AlgoStore.Service.Statistics.Services;
using Lykke.AlgoStore.Service.Statistics.Services.Strings;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.Statistics.Tests
{
    [TestFixture]
    public class StatisticsServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IStatisticsService _service;

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

            _service = MockService();
        }

        [Test]
        public void UpdateSummaryAsync_WithNullAsClientId_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync(null, "TEST"));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdCannotBeEmpty));
        }

        [Test]
        public void UpdateSummaryAsync_WithNullAsinstanceId_WillThrowException_Test()
        {
            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync("TEST", null));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdCannotBeEmpty));
        }

        private IStatisticsService MockService()
        {
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();

            statisticsRepositoryMock.Setup(x => x.GetSummaryAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(_fixture.Build<StatisticsSummary>().Create()));

            statisticsRepositoryMock
                .Setup(x => x.CreateOrUpdateSummaryAsync(_fixture.Build<StatisticsSummary>().Create()))
                .Returns(Task.CompletedTask);

            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x =>
                    x.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(_fixture.Build<AlgoClientInstanceData>().Create()));

            var assetsServiceWithCacheMock = new Mock<IAssetsServiceWithCache>();

            assetsServiceWithCacheMock.Setup(x => x.TryGetAssetPairAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(() => Task.FromResult(_fixture.Build<AssetPair>().Create()));

            var walletBalanceServiceMock = new Mock<IWalletBalanceService>();

            walletBalanceServiceMock.Setup(x => x.GetWalletBalancesAsync(It.IsAny<string>(), It.IsAny<AssetPair>()))
                .Returns(() => Task.FromResult(_fixture.Build<ClientBalanceResponseModel>().CreateMany()));

            walletBalanceServiceMock.Setup(x => x.GetTotalWalletBalanceInBaseAssetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AssetPair>()))
                .Returns(() => Task.FromResult(_fixture.Build<double>().Create()));

            return new StatisticsService(statisticsRepositoryMock.Object, algoClientInstanceRepositoryMock.Object,
                assetsServiceWithCacheMock.Object, walletBalanceServiceMock.Object);
        }
    }
}
