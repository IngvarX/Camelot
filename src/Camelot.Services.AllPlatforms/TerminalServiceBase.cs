using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
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

        public TerminalSettings GetTerminalSettings() => GetSavedSettings() ?? GetDefaultSettings();

        public void SetTerminalSettings(TerminalSettings terminalSettings)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<TerminalSettings>();

            repository.Upsert(TerminalSettingsId, terminalSettings);
        }

        protected abstract TerminalSettings GetDefaultSettings();

        protected virtual (string, string) Wrap(string command, string arguments) => (command, arguments);

        protected virtual string Escape(string directory) => directory;

        private TerminalSettings GetSavedSettings()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<TerminalSettings>();

            return repository.GetById(TerminalSettingsId);
        }
    }
}