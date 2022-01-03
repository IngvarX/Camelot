namespace Camelot.Services.Abstractions;

public interface IPermissionsService
{
    bool CheckIfHasAccess(string directory);
}