namespace Camelot.Services.Linux.Interfaces;

public interface IShellCommandWrappingService
{
    (string, string) WrapWithNohup(string command, string arguments);
}