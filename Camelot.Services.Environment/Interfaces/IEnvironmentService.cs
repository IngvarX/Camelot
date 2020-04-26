namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentService
    {
        string NewLine { get; }
        
        int ProcessorsCount { get; }
        
        string GetEnvironmentVariable(string variableName);
    }
}