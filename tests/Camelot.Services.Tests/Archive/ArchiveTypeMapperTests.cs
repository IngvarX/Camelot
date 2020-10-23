using System.Collections.Generic;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Archive;
using Camelot.Services.Configuration;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Archive
{
    public class ArchiveTypeMapperTests
    {
        private readonly AutoMocker _autoMocker;

        public ArchiveTypeMapperTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("archive.tar", "archive", "tar", "tar", ArchiveType.Tar)]
        [InlineData("archive.tar.gz", "archive.tar", "gz", "tar.gz", ArchiveType.TarGz)]
        [InlineData("archive.zip", "archive", "zip", "zip", ArchiveType.Zip)]
        [InlineData("archive.bz2", "archive", "bz2", "bz2", ArchiveType.Bz2)]
        public void TestGetArchiveTypeFrom(string filePath, string fileName, string extension, string fullExtension,
            ArchiveType archiveType)
        {
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(filePath))
                .Returns(fileName);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(filePath))
                .Returns(extension);

            var configuration = new ArchiveTypeMapperConfiguration
            {
                ExtensionToArchiveTypeDictionary = new Dictionary<string, ArchiveType>
                {
                    [fullExtension] = archiveType
                }
            };
            _autoMocker.Use(configuration);

            var mapper = _autoMocker.CreateInstance<ArchiveTypeMapper>();
            var actualType = mapper.GetArchiveTypeFrom(filePath);

            Assert.NotNull(actualType);
            Assert.Equal(archiveType, actualType);
        }

        [Theory]
        [InlineData("archive.txt", "archive", "txt")]
        public void TestGetArchiveTypeFromFailed(string filePath, string fileName, string extension)
        {
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(filePath))
                .Returns(fileName);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(filePath))
                .Returns(extension);

            var mapper = _autoMocker.CreateInstance<ArchiveTypeMapper>();
            var actualType = mapper.GetArchiveTypeFrom(filePath);

            Assert.Null(actualType);
        }
    }
}