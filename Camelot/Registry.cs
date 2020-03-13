using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.ViewModels;
using Splat;

namespace Camelot
{
    public static class Registry
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register<IFileService>(() => new FileService());
            services.Register(() => new MainWindowViewModel(
                resolver.GetService<IFileService>()));
        }
    }
}