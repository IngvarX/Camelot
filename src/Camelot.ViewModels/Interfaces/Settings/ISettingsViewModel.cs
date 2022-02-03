namespace Camelot.ViewModels.Interfaces.Settings;

public interface ISettingsViewModel
{
    bool IsChanged { get; }

    void Activate();

    void SaveChanges();
}