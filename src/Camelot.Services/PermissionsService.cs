using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class PermissionsService : IPermissionsService
    {
        public PermissionsService()
        {

        }

        public bool CheckIfHasAccess(string nodePath)
        {
            return false;
        }
    }
}