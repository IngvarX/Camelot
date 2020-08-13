using System.Text.RegularExpressions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications
{
    public class FileNameRegexSpecification : ISpecification<NodeModelBase>
    {
        private readonly string _regex;
        private readonly bool _isCaseSensitive;

        public FileNameRegexSpecification(string regex, bool isCaseSensitive)
        {
            _regex = regex;
            _isCaseSensitive = isCaseSensitive;
        }

        public bool IsSatisfiedBy(NodeModelBase nodeModel) =>
            Regex.IsMatch(nodeModel.Name, _regex,
                _isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
    }
}