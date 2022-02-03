using System;
using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;

public class NodeNameTextSpecification : SpecificationBase
{
    private readonly string _text;
    private readonly bool _isCaseSensitive;

    public NodeNameTextSpecification(
        string text,
        bool isCaseSensitive,
        bool isRecursive)
        : base(isRecursive)
    {
        _text = text;
        _isCaseSensitive = isCaseSensitive;
    }

    public override bool IsSatisfiedBy(NodeModelBase nodeModel) =>
        nodeModel.Name.Contains(_text,
            _isCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
}