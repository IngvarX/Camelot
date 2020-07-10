using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacTerminalService : TerminalServiceBase
    {
        public MacTerminalService(
            IProcessService processService,
            IUnitOfWorkFactory unitOfWorkFactory)
            : base(processService, unitOfWorkFactory)
        {

        }

        protected override TerminalSettings GetDefaultSettings() =>
            new TerminalSettings
            {
                Command = "open",
                Arguments = "-a Terminal \"{0}\""
            };
    }
}