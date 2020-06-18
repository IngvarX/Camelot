using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsTerminalService : TerminalServiceBase
    {
        public WindowsTerminalService(
            IProcessService processService,
            IUnitOfWorkFactory unitOfWorkFactory)
            : base(processService, unitOfWorkFactory)
        {

        }

        protected override TerminalSettings GetDefaultSettings() =>
            new TerminalSettings
            {
                Command = "cmd",
                Arguments = "/K \"cd /d {0}\""
            };
    }
}