using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Operations.Tests
{
    public class AsyncOperationStateMachineTests
    {
        private readonly AutoMocker _autoMocker;

        public AsyncOperationStateMachineTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestProperties()
        {
            const double progress = 42;
            var operationInfo = new OperationInfo(OperationType.Copy, new BinaryFileSystemOperationSettings(
                new string[0], new string[0], new string[0],
                new string[0], new Dictionary<string, string>(0), new string[0]));

            _autoMocker
                .Setup<ICompositeOperation, OperationInfo>(m => m.Info)
                .Returns(operationInfo);
            _autoMocker
                .Setup<ICompositeOperation, double>(m => m.CurrentProgress)
                .Returns(progress);

            var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();
            Assert.Equal(operationInfo, machine.Info);
            Assert.Equal(progress, machine.CurrentProgress);
        }
    }
}