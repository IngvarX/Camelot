using Camelot.Services.Environment.Interfaces;
using SysEnv = System.Environment;

namespace Camelot.Services.Environment.Implementations
{
    public class EnvironmentService : IEnvironmentService
    {
        public string NewLine => SysEnv.NewLine;

        public int ProcessorsCount => SysEnv.ProcessorCount;

        public bool Is64BitProcess => SysEnv.Is64BitProcess;

        public string GetEnvironmentVariable(string variableName) =>
            SysEnv.GetEnvironmentVariable(variableName);
    }
}