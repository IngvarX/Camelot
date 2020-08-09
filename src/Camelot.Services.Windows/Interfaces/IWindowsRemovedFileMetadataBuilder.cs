using System;

namespace Camelot.Services.Windows.Interfaces
{
    public interface IWindowsRemovedFileMetadataBuilder
    {
        IWindowsRemovedFileMetadataBuilder WithFileSize(long deletedFileSize);

        IWindowsRemovedFileMetadataBuilder WithRemovingDateTime(DateTime removingDateTime);

        IWindowsRemovedFileMetadataBuilder WithFilePath(string filePath);

        byte[] Build();
    }
}