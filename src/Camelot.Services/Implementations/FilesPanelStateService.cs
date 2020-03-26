using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FilesPanelStateService : IFilesPanelStateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly string _panelKey;

        public FilesPanelStateService(
            IUnitOfWorkFactory unitOfWorkFactory,
            string panelKey)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _panelKey = panelKey;
        }

        public PanelState GetPanelState()
        {
            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<PanelState>();

            return repository.GetById(_panelKey) ?? PanelState.Empty;
        }

        public void SavePanelState(PanelState state)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<PanelState>();
            repository.Upsert(_panelKey, state);

            unitOfWork.SaveChanges();
        }
    }
}