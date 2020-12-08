using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Mac.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacApplicationServiceTests
    {
        private const string Extension = "pdf";

        private readonly AutoMocker _autoMocker;

        public MacApplicationServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestGetInstalledApps()
        {
            var apps = new[]
            {
                new ApplicationModel()
            };

            _autoMocker
                .Setup<IApplicationsListLoader, IImmutableSet<ApplicationModel>>(m => m.GetInstalledApplications())
                .Returns(apps.ToImmutableHashSet);

            var service = _autoMocker.CreateInstance<MacApplicationService>();

            var actualApps = await service.GetInstalledApplicationsAsync();

            Assert.NotNull(actualApps);
            var actualAppsArray = actualApps.ToArray();
            Assert.Single(actualAppsArray);
            Assert.Equal(apps.Single(), actualAppsArray.Single());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(10, 1)]
        public async Task TestGetInstalledAppsMultiple(int iterationsCount, int expectedCallsCount)
        {
            var apps = new[]
            {
                new ApplicationModel()
            };

            _autoMocker
                .Setup<IApplicationsListLoader, IImmutableSet<ApplicationModel>>(m => m.GetInstalledApplications())
                .Returns(apps.ToImmutableHashSet)
                .Verifiable();

            var service = _autoMocker.CreateInstance<MacApplicationService>();

            for (var i = 0; i < iterationsCount; i++)
            {
                await service.GetInstalledApplicationsAsync();
            }

            _autoMocker
                .Verify<IApplicationsListLoader>(m => m.GetInstalledApplications(),
                    Times.Exactly(expectedCallsCount));
        }

        [Fact]
        public async Task TestGetAssociatedApps()
        {
            var apps = new[]
            {
                new ApplicationModel()
            };

            _autoMocker
                .Setup<IApplicationsListLoader, IImmutableSet<ApplicationModel>>(m => m.GetInstalledApplications())
                .Returns(apps.ToImmutableHashSet);
            _autoMocker
                .Setup<IApplicationsAssociationsLoader, Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>>>(m => m.LoadAssociatedApplicationsAsync(It.IsAny<IEnumerable<ApplicationModel>>()))
                .Returns<IEnumerable<ApplicationModel>>(a => Task.FromResult((IReadOnlyDictionary<string, ISet<ApplicationModel>>) new Dictionary<string, ISet<ApplicationModel>>
                {
                    {Extension, new HashSet<ApplicationModel>(a)}
                }));

            var service = _autoMocker.CreateInstance<MacApplicationService>();

            var actualApps = await service.GetAssociatedApplicationsAsync(Extension);

            Assert.NotNull(actualApps);
            var actualAppsArray = actualApps.ToArray();
            Assert.Single(actualAppsArray);
            Assert.Equal(apps.Single(), actualAppsArray.Single());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(10, 1)]
        public async Task TestGetAssociatedAppsMultiple(int iterationsCount, int expectedCallsCount)
        {
            var apps = new[]
            {
                new ApplicationModel()
            };

            _autoMocker
                .Setup<IApplicationsListLoader, IImmutableSet<ApplicationModel>>(m => m.GetInstalledApplications())
                .Returns(apps.ToImmutableHashSet);
            _autoMocker
                .Setup<IApplicationsAssociationsLoader, Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>>>(m => m.LoadAssociatedApplicationsAsync(It.IsAny<IEnumerable<ApplicationModel>>()))
                .Returns<IEnumerable<ApplicationModel>>(a => Task.FromResult((IReadOnlyDictionary<string, ISet<ApplicationModel>>) new Dictionary<string, ISet<ApplicationModel>>
                {
                    {Extension, new HashSet<ApplicationModel>(a)}
                }))
                .Verifiable();

            var service = _autoMocker.CreateInstance<MacApplicationService>();

            for (var i = 0; i < iterationsCount; i++)
            {
                await service.GetAssociatedApplicationsAsync(Extension);
            }

            _autoMocker
                .Verify<IApplicationsAssociationsLoader, Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>>>(
                    m => m.LoadAssociatedApplicationsAsync(It.IsAny<IEnumerable<ApplicationModel>>()),
                    Times.Exactly(expectedCallsCount));
        }
    }
}