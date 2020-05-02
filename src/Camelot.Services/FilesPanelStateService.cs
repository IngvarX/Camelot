using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;

namespace Camelot.Services
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

        public PanelModel GetPanelState()
        {
            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<PanelModel>();

            return repository.GetById(_panelKey) ?? PanelModel.Empty;
        }

        public void SavePanelState(PanelModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<PanelModel>();
            repository.Upsert(_panelKey, model);

            unitOfWork.SaveChanges();
        }
    }
}