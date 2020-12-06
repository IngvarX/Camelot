using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Linux.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxApplicationServiceTests
    {
        private const string FullPath = "FullPath";
        private const string FileName = "FileName";
        private const string AppName = "Camelot";
        private const string AppExec = "camelot";
        private const string MimeType = "application/json";
        private const string Extension = "json";
        
        private readonly AutoMocker _autoMocker;

        public LinuxApplicationServiceTests()
        {
            _autoMocker = new AutoMocker();
            
        }
        
        [Theory]
        [InlineData("Application", AppName, AppExec, 1)]
        [InlineData("Application", "", AppExec, 0)]
        [InlineData("Application", AppName, "", 0)]
        [InlineData("App", AppName, AppExec, 0)]
        public async Task TestGetInstalledApplicationsAsync(string entryType, string appName, string appExec,
            int appsCount)
        {
            var files = new[]
            {
                new FileModel
                {
                    FullPath = FullPath
                }
            };
            await using var fileStream = new MemoryStream();
            await using var mimeFileStream = new MemoryStream();
            await using var defaultsListFileStream = new MemoryStream();
            
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<FileModel>>()))
                .Returns(files)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead(FullPath))
                .Returns(fileStream);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead("/usr/share/applications/defaults.list"))
                .Returns(defaultsListFileStream);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead("/etc/mime.types"))
                .Returns(mimeFileStream);
            _autoMocker
                .Setup<IMimeTypesReader, Task<IReadOnlyDictionary<string, List<string>>>>(m => m.ReadAsync(mimeFileStream))
                .ReturnsAsync(new Dictionary<string, List<string>>
                {
                    {
                        MimeType, new List<string>
                        {
                            Extension
                        }
                    }
                });
            _autoMocker
                .Setup<IIniReader, Task<IReadOnlyDictionary<string, string>>>(m => m.ReadAsync(fileStream))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    {"Desktop Entry:Type", entryType},
                    {"Desktop Entry:Name", appName},
                    {"Desktop Entry:Exec", appExec},
                    {"Desktop Entry:MimeType", MimeType},
                });
            _autoMocker
                .Setup<IIniReader, Task<IReadOnlyDictionary<string, string>>>(m => m.ReadAsync(defaultsListFileStream))
                .ReturnsAsync(new Dictionary<string, string>
                {
                });
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(FullPath))
                .Returns(FileName);
            
            var service = _autoMocker.CreateInstance<LinuxApplicationService>();
            var apps = await service.GetInstalledApplicationsAsync();
            
            Assert.NotNull(apps);
            var appsArray = apps.ToArray();
            
            Assert.Equal(appsCount, appsArray.Length);

            if (appsCount == 1)
            {
                var app = appsArray.Single();
                
                Assert.Equal(AppName, app.DisplayName);
                Assert.Equal("{0}", app.Arguments);
                Assert.Equal(AppExec, app.ExecutePath);
            }
        }
        
        [Theory]
        [InlineData("Application", AppName, AppExec, Extension, 1, FileName)]
        [InlineData("Application", "", AppExec, Extension, 0, FileName)]
        [InlineData("Application", "", AppExec, Extension, 0, "App")]
        [InlineData("Application", AppName, "", Extension, 0, FileName)]
        [InlineData("App", AppName, AppExec, Extension, 0, "123")]
        [InlineData("Application", AppName, AppExec, "test", 0, FileName)]
        public async Task TestGetAssociatedApplicationsAsync(string entryType, string appName, string appExec,
            string extension, int appsCount, string desktopFileName)
        {
            var files = new[]
            {
                new FileModel
                {
                    FullPath = FullPath
                }
            };
            await using var fileStream = new MemoryStream();
            await using var mimeFileStream = new MemoryStream();
            await using var defaultsListFileStream = new MemoryStream();
            
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<FileModel>>()))
                .Returns(files)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead(FullPath))
                .Returns(fileStream);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead("/usr/share/applications/defaults.list"))
                .Returns(defaultsListFileStream);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead("/etc/mime.types"))
                .Returns(mimeFileStream);
            _autoMocker
                .Setup<IMimeTypesReader, Task<IReadOnlyDictionary<string, List<string>>>>(m => m.ReadAsync(mimeFileStream))
                .ReturnsAsync(new Dictionary<string, List<string>>
                {
                    {
                        MimeType, new List<string>
                        {
                            Extension
                        }
                    }
                });
            _autoMocker
                .Setup<IIniReader, Task<IReadOnlyDictionary<string, string>>>(m => m.ReadAsync(fileStream))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    {"Desktop Entry:Type", entryType},
                    {"Desktop Entry:Name", appName},
                    {"Desktop Entry:Exec", appExec},
                    {"Desktop Entry:MimeType", MimeType},
                });
            _autoMocker
                .Setup<IIniReader, Task<IReadOnlyDictionary<string, string>>>(m => m.ReadAsync(defaultsListFileStream))
                .ReturnsAsync(new Dictionary<string, string>
                {
                    {"Default Applications:application/csv", "test.desktop"},
                    {"Default Applications:application/json", desktopFileName}
                });
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(FullPath))
                .Returns(FileName);
            
            var service = _autoMocker.CreateInstance<LinuxApplicationService>();
            var apps = await service.GetAssociatedApplicationsAsync(extension);
            
            Assert.NotNull(apps);
            var appsArray = apps.ToArray();
            
            Assert.Equal(appsCount, appsArray.Length);

            if (appsCount == 1)
            {
                var app = appsArray.Single();
                
                Assert.Equal(AppName, app.DisplayName);
                Assert.Equal("{0}", app.Arguments);
                Assert.Equal(AppExec, app.ExecutePath);
            }
        }
        
        [Fact]
        public async Task TestGetInstalledApplicationsAsyncMultiple()
        {
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<FileModel>>()))
                .Returns(new FileModel[0])
                .Verifiable();
            _autoMocker
                .Setup<IIniReader, Task<IReadOnlyDictionary<string, string>>>(m => m.ReadAsync(It.IsAny<Stream>()))
                .ReturnsAsync(new Dictionary<string, string>());
            
            var service = _autoMocker.CreateInstance<LinuxApplicationService>();
            for (var i = 0; i < 10; i++)
            {
                await service.GetInstalledApplicationsAsync();
            }
            
            _autoMocker
                .Verify<IFileService>(m => m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<FileModel>>()),
                    Times.Once);
        }
    }
}