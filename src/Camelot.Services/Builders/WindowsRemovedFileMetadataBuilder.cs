using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camelot.Services.Builders
{
    public class WindowsRemovedFileMetadataBuilder
    {
        private const long MetadataHeader = 2;
        private const short EndOfLine = 0;

        private long _deletedFileSize;
        private DateTime _removingDateTime;
        private string _filePath;

        public WindowsRemovedFileMetadataBuilder WithFileSize(long deletedFileSize)
        {
            _deletedFileSize = deletedFileSize;

            return this;
        }
        
        public WindowsRemovedFileMetadataBuilder WithRemovingDateTime(DateTime removingDateTime)
        {
            _removingDateTime = removingDateTime;

            return this;
        }
        
        public WindowsRemovedFileMetadataBuilder WithFilePath(string filePath)
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
        
        private IEnumerable<byte> GetFilePathLengthAsBytes() => BitConverter.GetBytes(GetFileLength());

        private IEnumerable<byte> GetFilePathAsBytes() => Encoding.Unicode.GetBytes(_filePath);
        
        private static IEnumerable<byte> GetEndOfLineAsBytes() => BitConverter.GetBytes(EndOfLine);

        private int GetFileLength() => _filePath.Length + 1;
    }
}