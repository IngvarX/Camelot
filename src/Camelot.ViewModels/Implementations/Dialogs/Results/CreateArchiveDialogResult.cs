using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class CreateArchiveDialogResult : DialogResultBase
    {
        public string ArchivePath { get; }

        public ArchiveType ArchiveType { get; }

        public CreateArchiveDialogResult(
            string archivePath,
            ArchiveType archiveType)
        {
            ArchivePath = archivePath;
            ArchiveType = archiveType;
        }
    }
}