using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows.Builders
{
    public class WindowsRemovedFileMetadataBuilderFactory : IWindowsRemovedFileMetadataBuilderFactory
    {
        public IWindowsRemovedFileMetadataBuilder Create() => new WindowsRemovedFileMetadataBuilder();
    }
}