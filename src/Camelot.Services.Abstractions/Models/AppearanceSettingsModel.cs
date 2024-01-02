namespace Camelot.Services.Abstractions.Models;

public class AppearanceSettingsModel
{
    public bool ShowKeyboardShortcuts { get; }

    public AppearanceSettingsModel(bool showKeyboardShortcuts)
    {
        ShowKeyboardShortcuts = showKeyboardShortcuts;
    }
}