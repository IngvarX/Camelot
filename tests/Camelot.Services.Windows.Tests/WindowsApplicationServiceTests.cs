using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Windows.Enums;
using Camelot.Services.Windows.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;
using Match = System.Text.RegularExpressions.Match;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsApplicationServiceTests
    {
        private const string Extension = "json";
        private const string Code = "x";
        private const string AppName = "appNamd";
        private const string AppCommand = "app";
        private const string ExecutePath = "/home/app";

        private readonly AutoMocker _autoMocker;

        public WindowsApplicationServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("  \t ")]
        public async Task TestEmptyExtension(string extension)
        {
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(extension);
            
            Assert.NotNull(apps);
            Assert.Empty(apps);
        }
        
        [Fact]
        public async Task TestGetAssociatedApplicationsAsyncCurrUser32List()
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValue("MRUList"))
                .Returns(Code);
            subKeyMock
                .Setup(m => m.GetValue(Code))
                .Returns(AppName);
            
            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.json\OpenWithList"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.CurrentUser))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            Assert.NotNull(apps);

            var appsArray = apps.ToArray();
            Assert.Single(appsArray);

            var app = appsArray.Single();
            Assert.Equal(AppName, app.DisplayName);
            Assert.Equal(AppCommand, app.Arguments);
            Assert.Equal(ExecutePath, app.ExecutePath);
        }
        
        [Fact]
        public async Task TestGetAssociatedApplicationsAsyncCurrUser32Progids()
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValueNames())
                .Returns(new[] {AppName});

            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.json\OpenWithProgids"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.CurrentUser))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            Assert.NotNull(apps);

            var appsArray = apps.ToArray();
            Assert.Single(appsArray);

            var app = appsArray.Single();
            Assert.Equal(AppName, app.DisplayName);
            Assert.Equal(AppCommand, app.Arguments);
            Assert.Equal(ExecutePath, app.ExecutePath);
        }
        
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public async Task TestGetAssociatedApplicationsAsyncCurrUser64List(bool is64BitProcess, int resultsCount)
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValue("MRUList"))
                .Returns(Code);
            subKeyMock
                .Setup(m => m.GetValue(Code))
                .Returns(AppName);
            
            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.json\OpenWithList"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.CurrentUser))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            _autoMocker
                .Setup<IEnvironmentService, bool>(m => m.Is64BitProcess)
                .Returns(is64BitProcess);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            Assert.NotNull(apps);

            var appsArray = apps.ToArray();
            Assert.Equal(resultsCount, appsArray.Length);

            if (appsArray.Any())
            {
                var app = appsArray.Single();
                Assert.Equal(AppName, app.DisplayName);
                Assert.Equal(AppCommand, app.Arguments);
                Assert.Equal(ExecutePath, app.ExecutePath);
            }
        }
        
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public async Task TestGetAssociatedApplicationsAsyncCurrUser64Progids(bool is64BitProcess, int resultsCount)
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValueNames())
                .Returns(new[] {AppName});

            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.json\OpenWithProgids"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.CurrentUser))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            _autoMocker
                .Setup<IEnvironmentService, bool>(m => m.Is64BitProcess)
                .Returns(is64BitProcess);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            var appsArray = apps.ToArray();
            Assert.Equal(resultsCount, appsArray.Length);

            if (appsArray.Any())
            {
                var app = appsArray.Single();
                Assert.Equal(AppName, app.DisplayName);
                Assert.Equal(AppCommand, app.Arguments);
                Assert.Equal(ExecutePath, app.ExecutePath);
            }
        }
        
        [Fact]
        public async Task TestGetAssociatedApplicationsAsyncClassesRootList()
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValue("MRUList"))
                .Returns(Code);
            subKeyMock
                .Setup(m => m.GetValue(Code))
                .Returns(AppName);
            
            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"\.json\OpenWithList"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.ClassesRoot))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            Assert.NotNull(apps);

            var appsArray = apps.ToArray();
            Assert.Single(appsArray);

            var app = appsArray.Single();
            Assert.Equal(AppName, app.DisplayName);
            Assert.Equal(AppCommand, app.Arguments);
            Assert.Equal(ExecutePath, app.ExecutePath);
        }
        
        [Fact]
        public async Task TestGetAssociatedApplicationsAsyncClassesRootProgids()
        {
            var subKeyMock = new Mock<IRegistryKey>();
            subKeyMock
                .Setup(m => m.GetValueNames())
                .Returns(new[] {AppName});

            var registryKeyMock = new Mock<IRegistryKey>();
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(It.IsAny<string>()))
                .Returns(new Mock<IRegistryKey>().Object);
            registryKeyMock
                .Setup(m =>
                    m.OpenSubKey(@"\.json\OpenWithProgids"))
                .Returns(subKeyMock.Object);

            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(It.IsAny<RootRegistryKey>()))
                .Returns(new Mock<IRegistryKey>().Object);
            _autoMocker
                .Setup<IRegistryService, IRegistryKey>(m => m.GetRegistryKey(RootRegistryKey.ClassesRoot))
                .Returns(registryKeyMock.Object);
            _autoMocker
                .Setup<IApplicationInfoProvider, (string Name, string StartCommand, string ExecutePath)>(m => m.GetInfo(AppName))
                .Returns((AppName, AppCommand, ExecutePath));
            _autoMocker
                .Setup<IRegexService, IList<Match>>(m => m.GetMatches(It.IsAny<string>(), "%.", RegexOptions.Compiled))
                .Returns(new Match[0]);
            
            var service = _autoMocker.CreateInstance<WindowsApplicationService>();

            var apps = await service.GetAssociatedApplicationsAsync(Extension);
            
            Assert.NotNull(apps);

            var appsArray = apps.ToArray();
            Assert.Single(appsArray);

            var app = appsArray.Single();
            Assert.Equal(AppName, app.DisplayName);
            Assert.Equal(AppCommand, app.Arguments);
            Assert.Equal(ExecutePath, app.ExecutePath);
        }
    }
}