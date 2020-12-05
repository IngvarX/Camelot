namespace Camelot.Services.Windows.Interfaces
{
    public interface IApplicationInfoProvider
    {
        (string Name, string StartCommand, string ExecutePath) GetInfo(string applicationFile);
    }
}