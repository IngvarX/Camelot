using System;

namespace Camelot.Services.Linux.Interfaces.Builders
{
    public interface ILinuxRemovedFileMetadataBuilder
    {
        ILinuxRemovedFileMetadataBuilder WithFilePath(string filePath);

        ILinuxRemovedFileMetadataBuilder WithRemovingDateTime(DateTime removingDateTime);

        string Build();
    }
}