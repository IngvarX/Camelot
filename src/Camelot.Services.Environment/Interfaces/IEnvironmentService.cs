using System;

namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentService
    {
        DateTime Now { get; }
        
        string NewLine { get; }
        
        int ProcessorsCount { get; }
        
        string GetEnvironmentVariable(string variableName);
    }
}