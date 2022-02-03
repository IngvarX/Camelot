using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux.Implementations;

public class DriveNameService : IDriveNameService
{
    private const string ShowMountsCommand = "cat";
    private const string ShowMountsArguments = "/proc/mounts";
        
    private readonly IProcessService _processService;
    private readonly IEnvironmentService _environmentService;

    public DriveNameService(
        IProcessService processService,
        IEnvironmentService environmentService)
    {
        _processService = processService;
        _environmentService = environmentService;
    }
        
    public async Task<string> GetDriveNameAsync(string rootDirectory)
    {
        var mounts = await GetAllDrivesAsync();
        var dictionary = Parse(mounts);

        return dictionary[rootDirectory];
    }

    private Task<string> GetAllDrivesAsync() =>
        _processService.ExecuteAndGetOutputAsync(ShowMountsCommand, ShowMountsArguments);

    private IDictionary<string, string> Parse(string output)
    {
        var lines = output.Split(_environmentService.NewLine);
        var dictionary = new Dictionary<string, string>();

        foreach (var line in lines)
        {
            var data = line.Split();
            if (data.Length < 2)
            {
                continue;
            }

            dictionary[data[1]] = data[0];
        }

        return dictionary;
    }
}