using System.Text.RegularExpressions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;

public class NodeNameRegexSpecification : SpecificationBase
{
    private readonly IRegexService _regexService;
    private readonly string _regex;
    private readonly bool _isCaseSensitive;

    public NodeNameRegexSpecification(
        IRegexService regexService,
        string regex,
        bool isCaseSensitive,
        bool isRecursive)
        : base(isRecursive)
    {
        _regexService = regexService;
        _regex = regex;
        _isCaseSensitive = isCaseSensitive;
    }

    public override bool IsSatisfiedBy(NodeModelBase nodeModel) =>
        _regexService.CheckIfMatches(nodeModel.Name, _regex,
            _isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
}