
namespace Camelot.Services.Abstractions.Models;

public record QuickSearchNodeModel
{
    public string Name { get; init; }

    public bool IsFiltered { get; init; }

    public bool Selected { get; set; }
}
