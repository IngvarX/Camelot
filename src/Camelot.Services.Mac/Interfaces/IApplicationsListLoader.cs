using System.Collections.Immutable;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Mac.Interfaces
{
    public interface IApplicationsListLoader
    {
        IImmutableSet<ApplicationModel> GetInstalledApplications();
    }
}