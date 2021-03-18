using Avalonia.Controls.ApplicationLifetimes;

namespace Camelot.Ui.Tests
{
    public interface IUiTest
    {
        void Execute(IClassicDesktopStyleApplicationLifetime app);
    }
}