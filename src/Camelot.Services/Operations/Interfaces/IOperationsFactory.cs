using System.Collections.Generic;
using Camelot.Services.Operations.Settings;

namespace Camelot.Services.Operations.Interfaces
{
    public interface IOperationsFactory
    {
        IOperation CreateMoveOperation(IList<BinaryFileOperationSettings> parameters);

        IOperation CreateCopyOperation(IList<BinaryFileOperationSettings> parameters);

        IOperation CreateDeleteFileOperation(IList<UnaryFileOperationSettings> files);

        IOperation CreateDeleteDirectoryOperation(IList<UnaryFileOperationSettings> directories);
    }
}