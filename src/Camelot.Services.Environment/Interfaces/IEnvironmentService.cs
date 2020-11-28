namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentService
    {
        string NewLine { get; }

        int ProcessorsCount { get; }

        bool Is64BitProcess { get; }

        string GetEnvironmentVariable(string variableName);
    }
}