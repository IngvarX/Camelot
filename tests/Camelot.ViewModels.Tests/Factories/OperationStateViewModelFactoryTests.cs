using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class OperationStateViewModelFactoryTests
    {
        [Fact]
        public void TestCreate()
        {
            var pathServiceMock = new Mock<IPathService>();
            var operationMock = new Mock<IOperation>();

            var factory = new OperationStateViewModelFactory(pathServiceMock.Object);
            var viewModel = factory.Create(operationMock.Object);

            Assert.NotNull(viewModel);
            Assert.IsType<OperationStateViewModel>(viewModel);
        }
    }
}