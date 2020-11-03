using System.Collections.Generic;
using Camelot.ViewModels.Implementations.Dialogs.Archives;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IArchiveTypeViewModelFactory
    {
        IReadOnlyList<ArchiveTypeViewModel> CreateForSingleFile();

        IReadOnlyList<ArchiveTypeViewModel> CreateForMultipleFiles();
    }
}