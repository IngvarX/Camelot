using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class SuggestedPathViewModelTests
{
    [Theory]
    [InlineData("/home/camelot", SuggestedPathType.Directory, "camelot")]
    public void TestProperties(string fullPath, SuggestedPathType type, string text)
    {
        var viewModel = new SuggestedPathViewModel(fullPath, type, text);

        Assert.Equal(fullPath, viewModel.FullPath);
        Assert.Equal(type, viewModel.Type);
        Assert.Equal(text, viewModel.Text);
    }
}