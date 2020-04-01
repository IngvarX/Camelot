using Camelot.Services.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TabViewModelTests
    {
        private readonly TabViewModel _tabViewModel;
        
        public TabViewModelTests()
        {
            var pathServiceMock = new Mock<IPathService>();
            
            _tabViewModel = new TabViewModel(pathServiceMock.Object, string.Empty);
        }
        
        [Fact]
        public void TestActivateCommand()
        {
            var activationRequested = false;
            _tabViewModel.ActivationRequested += (sender, args) => activationRequested = true;
            _tabViewModel.ActivateCommand.Execute(null);
            
            Assert.True(activationRequested);
        }
        
        [Fact]
        public void TestNewTabCommand()
        {
            var newTabRequested = false;
            _tabViewModel.NewTabRequested += (sender, args) => newTabRequested = true;
            _tabViewModel.NewTabCommand.Execute(null);
            
            Assert.True(newTabRequested);
        }
        
        [Fact]
        public void TestCloseCommand()
        {
            var closeRequested = false;
            _tabViewModel.CloseRequested += (sender, args) => closeRequested = true;
            _tabViewModel.CloseTabCommand.Execute(null);
            
            Assert.True(closeRequested);
        }
        
        [Fact]
        public void TestCloseTabsToTheLeftCommand()
        {
            var closeTabsToTheLeftRequested = false;
            _tabViewModel.ClosingTabsToTheLeftRequested += (sender, args) => closeTabsToTheLeftRequested = true;
            _tabViewModel.CloseTabsToTheLeftCommand.Execute(null);
            
            Assert.True(closeTabsToTheLeftRequested);
        }
        
        [Fact]
        public void TestCloseTabsToTheRightCommand()
        {
            var closeTabsToTheRightRequested = false;
            _tabViewModel.ClosingTabsToTheRightRequested += (sender, args) => closeTabsToTheRightRequested = true;
            _tabViewModel.CloseTabsToTheRightCommand.Execute(null);
            
            Assert.True(closeTabsToTheRightRequested);
        }
        
        [Fact]
        public void TestCloseAllTabsButThisCommand()
        {
            var closeAllTabsButThisRequested = false;
            _tabViewModel.ClosingAllTabsButThisRequested += (sender, args) => closeAllTabsButThisRequested = true;
            _tabViewModel.CloseAllTabsButThisCommand.Execute(null);
            
            Assert.True(closeAllTabsButThisRequested);
        }
    }
}