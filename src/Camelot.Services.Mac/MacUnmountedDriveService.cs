using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Mac.Configuration;

namespace Camelot.Services.Mac;

public class MacUnmountedDriveService : UnmountedDriveServiceBase
{
    private const string MountDriveCommand = "diskutil";
    private const string MountDriveArguments = "mountDisk {0}";
    private const string ListAllDrivesCommand = "diskutil";
    private const string ListAllDrivesArguments = "list";
    private const string ListMountedDrivesCommand = "df";
    private const string ListMountedDrivesArguments = "-Hl";

    private readonly IProcessService _processService;
    private readonly IEnvironmentService _environmentService;
    private readonly UnmountedDrivesConfiguration _configuration;

    public MacUnmountedDriveService(
        IProcessService processService,
        IEnvironmentService environmentService,
        UnmountedDrivesConfiguration configuration)
    {
        _processService = processService;
        _environmentService = environmentService;
        _configuration = configuration;
    }

    public override void Mount(string drive)
    {
        var arguments = string.Format(MountDriveArguments, drive);

        _processService.Run(MountDriveCommand, arguments);
    }

    protected override async Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync()
    {
        if (!_configuration.IsEnabled)
        {
            return Array.Empty<UnmountedDriveModel>();
        }

        try
        {
            return await LoadUnmountedDrivesAsync();
        }
        catch
        {
            return Array.Empty<UnmountedDriveModel>();
        }
    }

    private async Task<IReadOnlyList<UnmountedDriveModel>> LoadUnmountedDrivesAsync()
    {
        var allDrives = await GetAllDrivesAsync();
        var mountedDrives = await GetMountedDrivesAsync();

        return allDrives
            .Except(mountedDrives)
            .Select(CreateFrom)
            .ToArray();
    }

    private async Task<IEnumerable<string>> GetAllDrivesAsync()
    {
        var drives = await _processService.ExecuteAndGetOutputAsync(ListAllDrivesCommand, ListAllDrivesArguments);

        return drives
            .Split(_environmentService.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Split()[0])
            .Where(d => d.StartsWith("/dev/") && !d.StartsWith("/dev/disk0"));
    }

    private async Task<IEnumerable<string>> GetMountedDrivesAsync()
    {
        var drives = await _processService.ExecuteAndGetOutputAsync(ListMountedDrivesCommand, ListMountedDrivesArguments);

        return drives
            .Split(_environmentService.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(ExtractBaseDriveName);
    }

    private static string ExtractBaseDriveName(string driveInfo)
    {
        var fullName = driveInfo.Split()[0];

        return fullName[..^2];
    }

    private static UnmountedDriveModel CreateFrom(string driveName) =>
        new UnmountedDriveModel
        {
            Name = GetShortName(driveName),
            FullName = driveName
        };

    private static string GetShortName(string fullName) =>
        fullName[(fullName.LastIndexOf("/", StringComparison.InvariantCulture) + 1)..];
}