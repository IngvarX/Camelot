using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models.State;
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

        protected override TerminalSettingsStateModel GetDefaultSettings() =>
            new TerminalSettingsStateModel
            {
                Command = "open",
                Arguments = "-a Terminal \"{0}\""
            };

        protected override string Escape(string directory) =>
            directory.Replace("\"", @"\""");
    }
}