using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services;

public class PermissionsService : IPermissionsService
{
    private readonly IEnvironmentDirectoryService _environmentDirectoryService;

    public PermissionsService(
        IEnvironmentDirectoryService environmentDirectoryService)
    {
        _environmentDirectoryService = environmentDirectoryService;
    }

    public bool CheckIfHasAccess(string directory)
    {
        try
        {
            _environmentDirectoryService.GetDirectories(directory);

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}