using System.ComponentModel.DataAnnotations;
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
        }

        [Test]
        public void UpdateSummaryAsync_WithNullAsClientId_WillThrowException_Test()
        {
            _service = MockValidService();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync(null, "TEST"));

            Assert.That(ex.Message, Is.EqualTo(Phrases.ClientIdCannotBeEmpty));
        }

        [Test]
        public void UpdateSummaryAsync_WithNullAsInstanceId_WillThrowException_Test()
        {
            _service = MockValidService();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync("TEST", null));

            Assert.That(ex.Message, Is.EqualTo(Phrases.InstanceIdCannotBeEmpty));
        }

        [Test]
        public void UpdateSummaryAsync_WithValidClientIdAndInstanceId_WillThrowException_Test()
        {
            _service = MockValidService();

            var result = _service.UpdateSummaryAsync("TEST", "TEST").Result;

            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdateSummaryAsync_ForNonExistingSummary_WillThrowException_Test()
        {
            _service = MockServiceWithInvalidStatisticsRepository();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync("TEST", "TEST"));

            Assert.That(ex.Message, Is.EqualTo("Could not find statistic summary row for AlgoInstance: TEST"));
        }

        [Test]
        public void UpdateSummaryAsync_ForNonExistingInstanceId_WillThrowException_Test()
        {
            _service = MockServiceWithInvalidAlgoInstanceRepository();

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.UpdateSummaryAsync("TEST", "TEST"));

            Assert.That(ex.Message, Is.EqualTo("Could not find AlgoInstance with InstanceId TEST and ClientId TEST"));
        }

        private IStatisticsService MockValidService()
        {
            var statisticsRepositoryMock = MockValidStatisticsRepository();
            var algoClientInstanceRepositoryMock = MockValidAlgoClientInstanceRepository();
            var assetsServiceWithCacheMock = MockValidAssetsServiceWithCache();
            var walletBalanceServiceMock = MockValidWalletBalanceService();

            return new StatisticsService(statisticsRepositoryMock.Object, algoClientInstanceRepositoryMock.Object,
                assetsServiceWithCacheMock.Object, walletBalanceServiceMock.Object);
        }

        private IStatisticsService MockServiceWithInvalidStatisticsRepository()
        {
            var statisticsRepositoryMock = MockInvalidStatisticsRepository();
            var algoClientInstanceRepositoryMock = MockValidAlgoClientInstanceRepository();
            var assetsServiceWithCacheMock = MockValidAssetsServiceWithCache();
            var walletBalanceServiceMock = MockValidWalletBalanceService();

            return new StatisticsService(statisticsRepositoryMock.Object, algoClientInstanceRepositoryMock.Object,
                assetsServiceWithCacheMock.Object, walletBalanceServiceMock.Object);
        }

        private IStatisticsService MockServiceWithInvalidAlgoInstanceRepository()
        {
            var statisticsRepositoryMock = MockValidStatisticsRepository();
            var algoClientInstanceRepositoryMock = MockInvalidAlgoClientInstanceRepository();
            var assetsServiceWithCacheMock = MockValidAssetsServiceWithCache();
            var walletBalanceServiceMock = MockValidWalletBalanceService();

            return new StatisticsService(statisticsRepositoryMock.Object, algoClientInstanceRepositoryMock.Object,
                assetsServiceWithCacheMock.Object, walletBalanceServiceMock.Object);
        }

        private Mock<IWalletBalanceService> MockValidWalletBalanceService()
        {
            var walletBalanceServiceMock = new Mock<IWalletBalanceService>();

            walletBalanceServiceMock.Setup(x => x.GetWalletBalancesAsync(It.IsAny<string>(), It.IsAny<AssetPair>()))
                .Returns(() => Task.FromResult(_fixture.Build<ClientBalanceResponseModel>().CreateMany()));

            walletBalanceServiceMock.Setup(x =>
                    x.GetTotalWalletBalanceInBaseAssetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AssetPair>()))
                .Returns(() => Task.FromResult(_fixture.Build<double>().Create()));

            return walletBalanceServiceMock;
        }

        private Mock<IAssetsServiceWithCache> MockValidAssetsServiceWithCache()
        {
            var assetsServiceWithCacheMock = new Mock<IAssetsServiceWithCache>();

            assetsServiceWithCacheMock.Setup(x => x.TryGetAssetPairAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(() => Task.FromResult(_fixture.Build<AssetPair>().Create()));

            assetsServiceWithCacheMock.Setup(x => x.TryGetAssetAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(() => Task.FromResult(_fixture.Build<Asset>().Create()));

            return assetsServiceWithCacheMock;
        }

        private Mock<IAlgoClientInstanceRepository> MockValidAlgoClientInstanceRepository()
        {
            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x =>
                    x.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(_fixture.Build<AlgoClientInstanceData>().Create()));

            return algoClientInstanceRepositoryMock;
        }

        private Mock<IAlgoClientInstanceRepository> MockInvalidAlgoClientInstanceRepository()
        {
            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x =>
                    x.GetAlgoInstanceDataByClientIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult<AlgoClientInstanceData>(null));

            return algoClientInstanceRepositoryMock;
        }

        private Mock<IStatisticsRepository> MockValidStatisticsRepository()
        {
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();

            statisticsRepositoryMock.Setup(x => x.GetSummaryAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(_fixture.Build<StatisticsSummary>().Create()));

            statisticsRepositoryMock
                .Setup(x => x.CreateOrUpdateSummaryAsync(_fixture.Build<StatisticsSummary>().Create()))
                .Returns(Task.CompletedTask);

            return statisticsRepositoryMock;
        }

        private Mock<IStatisticsRepository> MockInvalidStatisticsRepository()
        {
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();

            statisticsRepositoryMock.Setup(x => x.GetSummaryAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult<StatisticsSummary>(null));

            statisticsRepositoryMock
                .Setup(x => x.CreateOrUpdateSummaryAsync(_fixture.Build<StatisticsSummary>().Create()))
                .Returns(Task.CompletedTask);

            return statisticsRepositoryMock;
        }
    }
}
