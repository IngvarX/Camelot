using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsFactory
    {
        IOperation CreateMoveOperation(IList<BinaryFileOperationSettings> parameters);

        IOperation CreateCopyOperation(IList<BinaryFileOperationSettings> parameters);

        IOperation CreateDeleteOperation(
            IList<UnaryFileOperationSettings> directories,
            IList<UnaryFileOperationSettings> files);

        // TODO: ???
        IOperation CreateDeleteToTrashOperation(IList<UnaryFileOperationSettings> parameters);
    }
}