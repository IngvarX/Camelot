using System.Collections.Generic;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services
{
    public class OpenWithApplicationService : IOpenWithApplicationService
    {
        private const string OpenWithApplicationSettingsId = nameof(OpenWithApplicationSettings);

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public OpenWithApplicationService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public ApplicationModel GetSelectedApplication(string fileExtension)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<OpenWithApplicationSettings>();

            var applicationSettings = repository.GetById(OpenWithApplicationSettingsId);
            if (applicationSettings.ApplicationByExtension.TryGetValue(fileExtension, out var application))
            {
                return CreateFrom(application);
            }

            return null;
        }

        public void SaveSelectedApplication(string fileExtension, ApplicationModel selectedApplication)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<OpenWithApplicationSettings>();

            var applicationSettings = repository.GetById(OpenWithApplicationSettingsId)
                                      ?? new OpenWithApplicationSettings();

            if (applicationSettings.ApplicationByExtension.ContainsKey(fileExtension))
            {
                applicationSettings.ApplicationByExtension[fileExtension] = CreateFrom(selectedApplication);
            }
            else
            {
                applicationSettings.ApplicationByExtension.Add(fileExtension, CreateFrom(selectedApplication));
            }

            repository.Upsert(OpenWithApplicationSettingsId, applicationSettings);
        }

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
