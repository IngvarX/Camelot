using System.Collections.Generic;
using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Implementations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class TabViewModelFactoryTests
    {
        private readonly AutoMocker _autoMocker;

        public TabViewModelFactoryTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestDirectoryName()
        {
            var directoryName = Directory.GetCurrentDirectory();
            var tabModel = new TabStateModel
            {
                Directory = directoryName,
                SortingSettings = new SortingSettingsStateModel(),
                History = new List<string> {directoryName}
            };
            _autoMocker
                .Setup<IPathService, string>(m => m.RightTrimPathSeparators(directoryName))
                .Returns(directoryName);

            var tabViewModelFactory = _autoMocker.CreateInstance<TabViewModelFactory>();
            var tabViewModel = tabViewModelFactory.Create(tabModel);

            Assert.Equal(directoryName, tabViewModel.CurrentDirectory);
        }
    }
}