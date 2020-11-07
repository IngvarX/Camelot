namespace Camelot.Services.Abstractions.Behaviors
{
    public interface IFileSystemNodeOpeningBehavior
    {
        void Open(string node);

        void OpenWith(string command, string arguments, string node);
    }
}