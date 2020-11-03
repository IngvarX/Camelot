using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Implementations;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class TabViewModelFactoryTests
    {
        [Fact]
        public void TestDirectoryName()
        {
            var directoryName = Directory.GetCurrentDirectory();
            var tabModel = new TabStateModel {Directory = directoryName, SortingSettings = new SortingSettingsStateModel()};
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.TrimPathSeparators(directoryName))
                .Returns(directoryName);

            var tabViewModelFactory = new TabViewModelFactory(pathServiceMock.Object);
            var tabViewModel = tabViewModelFactory.Create(tabModel);

            Assert.Equal(directoryName, tabViewModel.CurrentDirectory);
        }
    }
}