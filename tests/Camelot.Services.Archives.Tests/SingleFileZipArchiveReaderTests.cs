using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Archives.Interfaces;
using Moq.AutoMock;
using Xunit;
using IoFile = System.IO.File;

namespace Camelot.Services.Archives.Tests
{
    public class SingleFileZipArchiveReaderTests : IDisposable
    {
        private const string File = "archive.lz";
        private const string FileNameWithoutExtension = "archive";
        private const string Directory = "Dir";
        private const string NewFilePath = "Dir/archive";
        private const string Output = "output";

        private readonly AutoMocker _autoMocker;
        private readonly byte[] _data;

        public SingleFileZipArchiveReaderTests()
        {
            _autoMocker = new AutoMocker();
            _data = new byte[] {1, 2, 3, 42};
        }

        [Fact]
        public async Task TestExtractAsync()
        {
            await using var inMemoryStream = new MemoryStream();
            await inMemoryStream.WriteAsync(_data);
            inMemoryStream.Seek(0, SeekOrigin.Begin);

            await using var outFileStream = IoFile.OpenWrite(Output);

            var nullStream = Stream.Null;
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead(File))
                .Returns(nullStream);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenWrite(NewFilePath))
                .Returns(outFileStream);
            _autoMocker
                .Setup<IStreamFactory, Stream>(m => m.Create(nullStream))
                .Returns(inMemoryStream);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(File))
                .Returns(FileNameWithoutExtension);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(Directory, FileNameWithoutExtension))
                .Returns(NewFilePath);
            _autoMocker
                .Setup<IFileNameGenerationService, string>(m => m.GenerateFullName(NewFilePath))
                .Returns(NewFilePath);

            var reader = _autoMocker.CreateInstance<SingleFileZipArchiveReader>();

            await reader.ExtractAsync(File, Directory);

            var bytes = await IoFile.ReadAllBytesAsync(Output);

            Assert.Equal(_data, bytes);
        }

        public void Dispose()
        {
            if (IoFile.Exists(Output))
            {
                IoFile.Delete(Output);
            }
        }
    }
}