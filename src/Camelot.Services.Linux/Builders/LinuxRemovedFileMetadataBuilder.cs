using System;
using System.Text;
using Camelot.Services.Linux.Interfaces.Builders;

namespace Camelot.Services.Linux.Builders
{
    public class LinuxRemovedFileMetadataBuilder : ILinuxRemovedFileMetadataBuilder
    {
        private string _filePath;
        private DateTime _removingDateTime;

        public ILinuxRemovedFileMetadataBuilder WithFilePath(string filePath)
        {
            _filePath = filePath;

            return this;
        }

        public ILinuxRemovedFileMetadataBuilder WithRemovingDateTime(DateTime removingDateTime)
        {
            _removingDateTime = removingDateTime;

            return this;
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("[Trash Info]");
            stringBuilder.AppendLine($"Path={GetUtf8String(_filePath)}");
            stringBuilder.AppendLine($"DeletionDate={_removingDateTime:s}");

            return stringBuilder.ToString();
        }

        private static string GetUtf8String(string s)
        {
            var bytes = Encoding.Default.GetBytes(s);

            return Encoding.UTF8.GetString(bytes);
        }
    }
}