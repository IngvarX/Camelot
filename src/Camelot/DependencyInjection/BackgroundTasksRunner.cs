using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Splat;

namespace Camelot.DependencyInjection;

public static class BackgroundTasksRunner
{
    public static void Start(IReadonlyDependencyResolver resolver) =>
        Task.Run(() => RunTasksAsync(resolver));

    private static async Task RunTasksAsync(IReadonlyDependencyResolver resolver)
    {
        await InitializeApplicationsList(resolver);
    }

    private static async Task InitializeApplicationsList(IReadonlyDependencyResolver resolver)
    {
        var applicationService = resolver.GetRequiredService<IApplicationService>();

        await applicationService.GetInstalledApplicationsAsync();
    }
}