using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services
{
    public class CreateArchiveStateService : ICreateArchiveStateService
    {
        private const string Key = "CreateArchiveState";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public CreateArchiveStateService(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public CreateArchiveStateModel GetState()
        {
            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<CreateArchiveState>();
            var dbModel = repository.GetById(Key) ?? CreateArchiveState.Empty;

            return CreateFrom(dbModel);
        }

        public void SaveState(CreateArchiveStateModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using var unitOfWork = _unitOfWorkFactory.Create();
            var repository = unitOfWork.GetRepository<CreateArchiveState>();
            var dbModel = CreateFrom(model);
            repository.Upsert(Key, dbModel);

            unitOfWork.SaveChanges();
        }

        private static CreateArchiveStateModel CreateFrom(CreateArchiveState model) =>
            new CreateArchiveStateModel
            {
                ArchiveType = (ArchiveType) model.ArchiveType
            };

        private static CreateArchiveState CreateFrom(CreateArchiveStateModel model) =>
            new CreateArchiveState
            {
                ArchiveType = (int) model.ArchiveType
            };
    }
}