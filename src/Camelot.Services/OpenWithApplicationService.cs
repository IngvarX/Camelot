using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services
{
    public class OpenWithApplicationService : IOpenWithApplicationService
    {
        private const string OpenWithApplicationSettingsId = "OpenWithApplicationSettings";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public OpenWithApplicationService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public ApplicationModel GetSelectedApplication(string fileExtension)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<OpenWithApplicationSettings>();

            var applicationSettings = GetSettings(repository);
            
            return applicationSettings.ApplicationByExtension.TryGetValue(fileExtension, out var application)
                ? CreateFrom(application)
                : null;
        }

        public void SaveSelectedApplication(string fileExtension, ApplicationModel selectedApplication)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<OpenWithApplicationSettings>();

            var applicationSettings = GetSettings(repository);

            applicationSettings.ApplicationByExtension[fileExtension] = CreateFrom(selectedApplication);

            repository.Upsert(OpenWithApplicationSettingsId, applicationSettings);
        }

        private static OpenWithApplicationSettings GetSettings(IRepository<OpenWithApplicationSettings> repository) =>
            repository.GetById(OpenWithApplicationSettingsId) ?? OpenWithApplicationSettings.Empty;

        private static Application CreateFrom(ApplicationModel application) =>
            application is null
                ? null
                : new Application
                {
                    DisplayName = application.DisplayName,
                    Arguments = application.Arguments,
                    ExecutePath = application.ExecutePath
                };

        private static ApplicationModel CreateFrom(Application application) =>
            application is null
                ? null
                : new ApplicationModel
                {
                    DisplayName = application.DisplayName,
                    Arguments = application.Arguments,
                    ExecutePath = application.ExecutePath
                };
    }
}
