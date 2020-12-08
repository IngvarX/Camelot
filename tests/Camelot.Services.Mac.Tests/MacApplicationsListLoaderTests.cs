using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacApplicationsListLoaderTests
    {
        private readonly AutoMocker _autoMocker;

        public MacApplicationsListLoaderTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestGetInstalledApps()
        {
            var userAppDir = new DirectoryModel
            {
                FullPath = "A",
                Name = "A.app"
            };
            var systemAppDir = new DirectoryModel
            {
                FullPath = "B",
                Name = "B.app"
            };

            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                    m.GetChildDirectories("/Applications/", null))
                .Returns(new[] {userAppDir});
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                    m.GetChildDirectories("/System/Applications/", null))
                .Returns(new[] {systemAppDir});

            var service = _autoMocker.CreateInstance<MacApplicationsListLoader>();

            var apps = service.GetInstalledApplications();

            Assert.NotNull(apps);
            Assert.Equal(2, apps.Count);
            Assert.True(apps.All(a => a.Arguments == "{0}"));

            var userApp = apps.Single(a => a.ExecutePath == userAppDir.FullPath);
            Assert.Equal("A", userApp.DisplayName);

            var systemApp = apps.Single(a => a.ExecutePath == systemAppDir.FullPath);
            Assert.Equal("B", systemApp.DisplayName);
        }
    }
}