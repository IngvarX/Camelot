using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacTerminalService : TerminalServiceBase
    {
        private const string Command = "/Applications/Utilities/Terminal.app";
        private const string Arguments = "--workfir {0}";

        public MacTerminalService(
            IProcessService processService,
            IUnitOfWorkFactory unitOfWorkFactory)
            : base(processService, unitOfWorkFactory)
        {

        }

        protected override TerminalSettings GetDefaultSettings() =>
            new TerminalSettings {Command = Command, Arguments = Arguments};
    }
}