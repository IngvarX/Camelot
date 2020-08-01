using Camelot.Services.Linux.Interfaces.Builders;

namespace Camelot.Services.Linux.Builders
{
    public class LinuxRemovedFileMetadataBuilderFactory : ILinuxRemovedFileMetadataBuilderFactory
    {
        public ILinuxRemovedFileMetadataBuilder Create() => new LinuxRemovedFileMetadataBuilder();
    }
}