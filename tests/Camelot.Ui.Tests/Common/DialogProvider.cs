using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;

namespace Camelot.Ui.Tests.Common;

public static class DialogProvider
{
    public static TDialog GetDialog<TDialog>(IClassicDesktopStyleApplicationLifetime app) =>
        app
            .Windows
            .OfType<TDialog>()
            .SingleOrDefault();
}