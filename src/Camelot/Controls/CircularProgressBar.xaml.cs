using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Path = Avalonia.Controls.Shapes.Path;

namespace Camelot.Controls
{
    public class CircularProgressBar : UserControl
    {
        private bool _isInitialized;
        private double _value;
        private ArcSegment _progressArcSegment;
        private ArcSegment _remainingArcSegment;

        public static readonly DirectProperty<CircularProgressBar, double> ValueProperty
            = AvaloniaProperty.RegisterDirect<CircularProgressBar, double>(nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v);

        public static readonly StyledProperty<double> MinValueProperty
            = AvaloniaProperty.Register<CircularProgressBar, double>(nameof(MinValue));

        public static readonly StyledProperty<double> MaxValueProperty
            = AvaloniaProperty.Register<CircularProgressBar, double>(nameof(MaxValue));

        public static readonly StyledProperty<double> RadiusProperty
            = AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Radius));

        public static readonly StyledProperty<double> StrokeThicknessProperty
            = AvaloniaProperty.Register<CircularProgressBar, double>(nameof(StrokeThickness));

        public double Value
        {
            get => _value;
            set
            {
                if (SetAndRaise(ValueProperty, ref _value, value))
                {
                    Refresh();
                }
            }
        }

        public double MinValue
        {
            get => GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double MaxValue
        {
            get => GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public double Radius
        {
            get => GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public CircularProgressBar()
        {
            InitializeComponent();
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            var progressPath = this.FindControl<Path>("PART_ProgressPath");
            var remainingPath = this.FindControl<Path>("PART_RemainingPath");

            Width = Height = remainingPath.Width = remainingPath.Height = progressPath.Width = progressPath.Height = 2 * (Radius + StrokeThickness + 4);
            remainingPath.StrokeThickness = progressPath.StrokeThickness = StrokeThickness;

            var progressPathFigure =
                ((PathFigures) GetPropertyValue(progressPath.Data, "Figures"))
                .Single();
            var remainingPathFigure =
                ((PathFigures) GetPropertyValue(remainingPath.Data, "Figures"))
                .Single();

            _progressArcSegment = (ArcSegment)progressPathFigure.Segments.Single();
            _remainingArcSegment = (ArcSegment)remainingPathFigure.Segments.Single();

            progressPathFigure.StartPoint = remainingPathFigure.StartPoint = new Point(Radius, 0);
            _progressArcSegment.Size = _remainingArcSegment.Size = new Size(Radius, Radius);

            _isInitialized = true;
            Refresh();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Refresh()
        {
            if (!_isInitialized)
            {
                return;
            }

            var angleInRadians = 2 * Math.PI * (Value - MinValue) / (MaxValue - MinValue);
            var x = Radius * (1 + Math.Sin(angleInRadians));
            var y = Radius * (1 - Math.Cos(angleInRadians));
            var point = new Point(x, y);

            _remainingArcSegment.Point = _progressArcSegment.Point = point;
            // _progressArcSegment.Size = _remainingArcSegment.Size = new Size(1000, Radius);
            // _progressArcSegment.Size = _remainingArcSegment.Size = new Size(Radius, Radius);

        }

        private static object GetPropertyValue(object source, string propertyName) =>
            source.GetType().GetProperty(propertyName).GetValue(source, null);
    }
}