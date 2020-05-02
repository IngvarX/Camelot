using System.IO;
using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Implementations;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TabViewModelFactoryTests
    {
        [Fact]
        public void TestDirectoryName()
        {
            var directoryName = Directory.GetCurrentDirectory();
            var tabModel = new TabModel {Directory = directoryName};
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