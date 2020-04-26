namespace Camelot.Services.Environment.Interfaces
{
    public interface IProcessService
    {
        void Run(string command);

        void Run(string command, string arguments);
    }
}