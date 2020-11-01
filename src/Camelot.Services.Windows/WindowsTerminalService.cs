using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models.State;
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

        protected override TerminalSettingsStateModel GetDefaultSettings() =>
            new TerminalSettingsStateModel
            {
                Command = "cmd",
                Arguments = "/K \"cd /d {0}\""
            };
    }
}