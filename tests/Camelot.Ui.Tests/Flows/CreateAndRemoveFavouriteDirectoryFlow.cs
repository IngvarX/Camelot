using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Main.FavouriteDirectories;
using Xunit;

namespace Camelot.Ui.Tests.Flows;

public class CreateAndRemoveFavouriteDirectoryFlow
{
    [Fact(DisplayName = "Toggle favourite directory")]
    public async Task ToggleFavouriteDirectoryTest()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        Assert.Equal(1, GetFavouriteDirectoriesCount(window));
        Assert.Equal(2, GetFavouriteDirectoriesActiveIconsCount(window));

        ToggleFavouriteDirectoryStep.ToggleFavouriteDirectory(window);
        await WaitService.WaitForConditionAsync(() => GetFavouriteDirectoriesCount(window) == 0);
        await WaitService.WaitForConditionAsync(() => GetFavouriteDirectoriesActiveIconsCount(window) == 0);

        ToggleFavouriteDirectoryStep.ToggleFavouriteDirectory(window);
        await WaitService.WaitForConditionAsync(() => GetFavouriteDirectoriesCount(window) == 1);
        await WaitService.WaitForConditionAsync(() => GetFavouriteDirectoriesActiveIconsCount(window) == 2);
    }

    private static int GetFavouriteDirectoriesCount(IVisual window) =>
        window
            .GetVisualDescendants()
            .OfType<FavouriteDirectoryView>()
            .Count();

    private static int GetFavouriteDirectoriesActiveIconsCount(IVisual window) =>
        window
            .GetVisualDescendants()
            .OfType<Image>()
            .Count(i => i.Classes.ToHashSet().Contains("favouriteDirectoryImage") && i.IsVisible);
}