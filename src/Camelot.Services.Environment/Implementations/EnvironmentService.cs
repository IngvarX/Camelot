using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class EnvironmentService : IEnvironmentService
    {
        public string NewLine => System.Environment.NewLine;
        
        public int ProcessorsCount => System.Environment.ProcessorCount;

        public string GetEnvironmentVariable(string variableName) =>
            System.Environment.GetEnvironmentVariable(variableName);
    }
}