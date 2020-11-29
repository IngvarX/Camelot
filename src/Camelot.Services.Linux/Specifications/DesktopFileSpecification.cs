using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.Services.Linux.Specifications
{
    public class DesktopFileSpecification : ISpecification<FileModel>
    {
        private const string DesktopFileExtension = "desktop";

        public bool IsSatisfiedBy(FileModel fileModel) => fileModel.Extension == DesktopFileExtension;
    }
}