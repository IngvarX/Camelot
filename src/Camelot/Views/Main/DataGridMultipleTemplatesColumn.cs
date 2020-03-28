using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Camelot.Views.Main
{
    public class DataGridMultipleTemplatesColumn : DataGridTemplateColumn
    {
        private DataTemplates _cellTemplates;
        
        public static readonly DirectProperty<DataGridMultipleTemplatesColumn, DataTemplates> CellTemplatesProperty =
            AvaloniaProperty.RegisterDirect<DataGridMultipleTemplatesColumn, DataTemplates>(
                nameof(CellTemplates),
                o => o.CellTemplates,
                (o, v) => o.CellTemplates = v);
        
        public DataTemplates CellTemplates
        {
            get => _cellTemplates;
            set => SetAndRaise(CellTemplatesProperty, ref _cellTemplates, value);
        }
        
        protected override IControl GenerateElement(DataGridCell cell, object dataItem)
        {
            if (CellTemplates != null)
            {
                var matchedTemplate = CellTemplates.First(t => t.Match(dataItem));
                
                return matchedTemplate.Build(dataItem);
            }
            
            if (Design.IsDesignMode)
            {
                return null;
            }
            
            throw new InvalidOperationException($"Template for type {nameof(DataGridMultipleTemplatesColumn)} is missing");
        }
    }
}