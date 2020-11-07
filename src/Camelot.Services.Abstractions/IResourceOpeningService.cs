namespace Camelot.Services.Abstractions
{
    public interface IResourceOpeningService
    {
        void Open(string resource);

        void OpenWith(string command, string arguments, string resource);
    }
}