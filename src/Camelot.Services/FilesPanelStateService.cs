using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services;

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

    public PanelStateModel GetPanelState()
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        var repository = unitOfWork.GetRepository<PanelModel>();
        var model = repository.GetById(_panelKey) ?? PanelModel.Empty;

        return CreateFrom(model);
    }

    public void SavePanelState(PanelStateModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        using var unitOfWork = _unitOfWorkFactory.Create();
        var repository = unitOfWork.GetRepository<PanelModel>();
        var dbModel = CreateFrom(model);
        repository.Upsert(_panelKey, dbModel);

        unitOfWork.SaveChanges();
    }

    private static PanelStateModel CreateFrom(PanelModel model) =>
        new()
        {
            SelectedTabIndex = model.SelectedTabIndex,
            Tabs = model.Tabs.Select(CreateFrom).ToArray()
        };

    private static TabStateModel CreateFrom(TabModel model) =>
        new()
        {
            Directory = model.Directory,
            SortingSettings = CreateFrom(model.SortingSettings),
            History = GetHistory(model),
            CurrentPositionInHistory = model.CurrentPositionInHistory
        };

    private static SortingSettingsStateModel CreateFrom(SortingSettings model) =>
        new()
        {
            IsAscending = model.IsAscending,
            SortingMode = (SortingMode) model.SortingMode
        };

    private static PanelModel CreateFrom(PanelStateModel model) =>
        new()
        {
            SelectedTabIndex = model.SelectedTabIndex,
            Tabs = model.Tabs.Select(CreateFrom).ToList()
        };

    private static TabModel CreateFrom(TabStateModel model) =>
        new()
        {
            Directory = model.Directory,
            SortingSettings = CreateFrom(model.SortingSettings),
            History = model.History,
            CurrentPositionInHistory = model.CurrentPositionInHistory
        };

    private static SortingSettings CreateFrom(SortingSettingsStateModel model) =>
        new()
        {
            IsAscending = model.IsAscending,
            SortingMode = (int) model.SortingMode
        };

    private static List<string> GetHistory(TabModel model) =>
        model.History.Any() ? model.History : new List<string> {model.Directory};
}