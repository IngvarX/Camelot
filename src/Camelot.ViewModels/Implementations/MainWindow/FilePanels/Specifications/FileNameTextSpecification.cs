using System;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications
{
    public class FileNameTextSpecification : ISpecification<NodeModelBase>
    {
        private readonly string _text;
        private readonly bool _isCaseSensitive;

        public FileNameTextSpecification(string text, bool isCaseSensitive)
        {
            _text = text;
            _isCaseSensitive = isCaseSensitive;
        }

        public bool IsSatisfiedBy(NodeModelBase nodeModel) =>
            nodeModel.Name.Contains(_text,
                _isCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
    }
}