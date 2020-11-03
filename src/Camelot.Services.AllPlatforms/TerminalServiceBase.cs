using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.AllPlatforms
{
    public abstract class TerminalServiceBase : ITerminalService
    {
        private const string TerminalSettingsId = "TerminalSettings";

        private readonly IProcessService _processService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        protected TerminalServiceBase(
            IProcessService processService,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _processService = processService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void Open(string directory)
        {
            var (command, arguments) = GetTerminalSettings();
            var (wrappedCommand, wrappedArguments) = Wrap(command, arguments);
            var escapedDirectory = Escape(directory);

            _processService.Run(wrappedCommand, string.Format(wrappedArguments, escapedDirectory));
        }

        public TerminalSettingsStateModel GetTerminalSettings() => GetSavedSettings() ?? GetDefaultSettings();

        public void SetTerminalSettings(TerminalSettingsStateModel terminalSettingsState)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<TerminalSettings>();
            var dbModel = CreateFrom(terminalSettingsState);

            repository.Upsert(TerminalSettingsId, dbModel);
        }

        protected abstract TerminalSettingsStateModel GetDefaultSettings();

        protected virtual (string, string) Wrap(string command, string arguments) => (command, arguments);

        protected virtual string Escape(string directory) => directory;

        private TerminalSettingsStateModel GetSavedSettings()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<TerminalSettings>();
            var dbModel = repository.GetById(TerminalSettingsId);

            return CreateFrom(dbModel);
        }

        private static TerminalSettingsStateModel CreateFrom(TerminalSettings model) =>
            model is null
                ? null
                : new TerminalSettingsStateModel
                {
                    Arguments = model.Arguments,
                    Command = model.Command
                };

        private static TerminalSettings CreateFrom(TerminalSettingsStateModel stateModel) =>
            new TerminalSettings
            {
                Arguments = stateModel.Arguments,
                Command = stateModel.Command
            };
    }
}