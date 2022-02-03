using System.Threading.Tasks;

namespace Camelot.Services.Environment.Interfaces;

public interface IProcessService
{
    void Run(string command, string arguments);

    Task<string> ExecuteAndGetOutputAsync(string command, string arguments);
}