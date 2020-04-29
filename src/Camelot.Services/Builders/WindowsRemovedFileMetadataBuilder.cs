using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camelot.Services.Builders
{
    public class WindowsRemovedFileMetadataBuilder
    {
        private const int MetadataHeader = 2;

        private long _deletedFileSize;
        private DateTime _removingDate;
        private string _filePath;

        public WindowsRemovedFileMetadataBuilder WithFileSize(long deletedFileSize)
        {
            _deletedFileSize = deletedFileSize;

            return this;
        }
        
        public WindowsRemovedFileMetadataBuilder WithRemovingDate(DateTime removingDate)
        {
            _removingDate = removingDate;

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

            return headerBytes
                .Concat(fileSizeBytes)
                .Concat(removingDateTimeBytes)
                .Concat(filePathLengthBytes)
                .Concat(filePathBytes)
                .ToArray();
        }
        
        private static IEnumerable<byte> GetHeaderAsBytes() => BitConverter.GetBytes(MetadataHeader);
        
        private IEnumerable<byte> GetFileSizeAsBytes() => BitConverter.GetBytes(_deletedFileSize);

        private IEnumerable<byte> GetRemovingDateTimeAsBytes()
        {
            var result = new byte[4];
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = _removingDate - epoch;
            var passedSeconds = (int)timeSpan.TotalSeconds;
            var copyBytes = BitConverter.GetBytes(passedSeconds);
            Array.Copy(copyBytes, 0, result, 0, 4);

            return result;
        }
        
        private IEnumerable<byte> GetFilePathLengthAsBytes() => BitConverter.GetBytes(_filePath.Length);

        private IEnumerable<byte> GetFilePathAsBytes() => Encoding.Unicode.GetBytes(_filePath);
    }
}