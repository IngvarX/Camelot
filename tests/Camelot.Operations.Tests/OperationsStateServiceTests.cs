using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Xunit;

namespace Camelot.Operations.Tests
{
    public class OperationsStateServiceTests
    {
        private readonly IOperationsStateService _operationsStateService;

        public OperationsStateServiceTests()
        {
            _operationsStateService = new OperationsStateService();
        }

        [Fact]
        public void TestAddingOperation()
        {
            Assert.Empty(_operationsStateService.ActiveOperations);

            var callbackCalled = false;
            var operationMock = new Mock<IOperation>();
            _operationsStateService.OperationStarted += (sender, args) => callbackCalled = true;
            _operationsStateService.AddOperation(operationMock.Object);

            Assert.True(_operationsStateService.ActiveOperations.Count == 1);
            var activeOperation = _operationsStateService.ActiveOperations.Single();
            Assert.True(activeOperation == operationMock.Object);
            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestRemovingOperation()
        {
            Assert.Empty(_operationsStateService.ActiveOperations);

            var operationMock = new Mock<IOperation>();
            _operationsStateService.AddOperation(operationMock.Object);
            var args = new OperationStateChangedEventArgs(OperationState.Finished);
            operationMock.Raise(m => m.StateChanged += null, args);

            Assert.Empty(_operationsStateService.ActiveOperations);
        }

        [Fact]
        public void TestOperationChangedState()
        {
            Assert.Empty(_operationsStateService.ActiveOperations);

            var operationMock = new Mock<IOperation>();
            _operationsStateService.AddOperation(operationMock.Object);
            var args = new OperationStateChangedEventArgs(OperationState.InProgress);
            operationMock.Raise(m => m.StateChanged += null, args);

            Assert.NotEmpty(_operationsStateService.ActiveOperations);
        }
    }
}