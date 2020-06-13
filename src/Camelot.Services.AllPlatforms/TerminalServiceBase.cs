using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.AllPlatforms
{
    public abstract class TerminalServiceBase : ITerminalService
    {
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
            var (command, arguments) = GetSavedCommand() ?? GetDefaultCommand();

            _processService.Run(command, string.Format(arguments, directory));
        }

        protected abstract TerminalSettings GetDefaultCommand();

        private TerminalSettings GetSavedCommand()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<TerminalSettings>();
            const string defaultSettingsId = "Default";

            return repository.GetById(defaultSettingsId);
        }
    }
}