using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows.Builders
{
    public class WindowsRemovedFileMetadataBuilder : IWindowsRemovedFileMetadataBuilder
    {
        private const long MetadataHeader = 2;
        private const short EndOfLine = 0;

        private long _deletedFileSize;
        private DateTime _removingDateTime;
        private string _filePath;

        public IWindowsRemovedFileMetadataBuilder WithFileSize(long deletedFileSize)
        {
            _deletedFileSize = deletedFileSize;

            return this;
        }

        public IWindowsRemovedFileMetadataBuilder WithRemovingDateTime(DateTime removingDateTime)
        {
            _removingDateTime = removingDateTime;

            return this;
        }

        public IWindowsRemovedFileMetadataBuilder WithFilePath(string filePath)
        {
            _filePath = filePath;

            return this;
        }

        public byte[] Build()
        {
            var headerBytes = GetHeaderAsBytes();
            var fileSizeBytes = GetFileSizeAsBytes();
            var removingDateTimeBytes = GetRemovingDateTimeAsBytes();
            var filePathLengthBytes = GetFilePathLengthAsBytes();
            var filePathBytes = GetFilePathAsBytes();
            var endOfLineBytes = GetEndOfLineAsBytes();

            return headerBytes
                .Concat(fileSizeBytes)
                .Concat(removingDateTimeBytes)
                .Concat(filePathLengthBytes)
                .Concat(filePathBytes)
                .Concat(endOfLineBytes)
                .ToArray();
        }

        private static IEnumerable<byte> GetHeaderAsBytes() => BitConverter.GetBytes(MetadataHeader);

        private IEnumerable<byte> GetFileSizeAsBytes() => BitConverter.GetBytes(_deletedFileSize);

        private IEnumerable<byte> GetRemovingDateTimeAsBytes() => BitConverter.GetBytes(_removingDateTime.ToFileTime());

        private IEnumerable<byte> GetFilePathLengthAsBytes() => BitConverter.GetBytes(GetFilePathLength());

        private IEnumerable<byte> GetFilePathAsBytes() => Encoding.Unicode.GetBytes(_filePath);

        private static IEnumerable<byte> GetEndOfLineAsBytes() => BitConverter.GetBytes(EndOfLine);

        private int GetFilePathLength() => _filePath.Length + 1;
    }
}