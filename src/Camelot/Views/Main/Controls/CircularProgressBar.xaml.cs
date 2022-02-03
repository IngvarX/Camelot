using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Path = Avalonia.Controls.Shapes.Path;

namespace Camelot.Views.Main.Controls;

public class CircularProgressBar : UserControl
{
    private const int Offset = 2;

    private bool _isInitialized;
    private double _value;
    private ArcSegment _progressArcSegment;
    private ArcSegment _remainingArcSegment;
    private TextBlock _progressTextBlock;

    private double FullRadius => Radius + StrokeThickness;

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _progressTextBlock = this.FindControl<TextBlock>("ProgressTextBlock");

        var progressPath = this.FindControl<Path>("ProgressPath");
        var remainingPath = this.FindControl<Path>("RemainingPath");

        Width = Height = remainingPath.Width =
            remainingPath.Height = progressPath.Width = progressPath.Height = 2 * (FullRadius + Offset);
        remainingPath.StrokeThickness = progressPath.StrokeThickness = StrokeThickness;

        var progressPathFigure = ((PathGeometry) progressPath.Data).Figures.Single();
        var remainingPathFigure = ((PathGeometry) remainingPath.Data).Figures.Single();
        progressPathFigure.StartPoint = remainingPathFigure.StartPoint = new Point(Offset + FullRadius, Offset);
        if (progressPathFigure.Segments is null || remainingPathFigure.Segments is null)
        {
            return;
        }

        _progressArcSegment = (ArcSegment) progressPathFigure.Segments.Single();
        _remainingArcSegment = (ArcSegment) remainingPathFigure.Segments.Single();
        _progressArcSegment.Size = _remainingArcSegment.Size = new Size(FullRadius, FullRadius);

        _isInitialized = true;

        Refresh();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void Refresh()
    {
        if (!_isInitialized)
        {
            return;
        }

        var angleInRadians = 2 * Math.PI * (Value - MinValue) / (MaxValue - MinValue);
        var x = Offset + FullRadius * (1 + Math.Sin(angleInRadians));
        var y = Offset + FullRadius * (1 - Math.Cos(angleInRadians));
        var point = new Point(x, y);

        _remainingArcSegment.Point = _progressArcSegment.Point = point;
        var isLargeArc = angleInRadians > Math.PI;
        _progressArcSegment.IsLargeArc = isLargeArc;
        _remainingArcSegment.IsLargeArc = !isLargeArc;
        _progressTextBlock.Text = ((int) Value).ToString();
    }
}