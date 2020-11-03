using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Implementations.Dialogs.Archives
{
    public class ArchiveTypeViewModel : ViewModelBase
    {
        public ArchiveType ArchiveType { get; }

        public string Name { get; }

        public ArchiveTypeViewModel(ArchiveType archiveType, string name)
        {
            ArchiveType = archiveType;
            Name = name;
        }
    }
}