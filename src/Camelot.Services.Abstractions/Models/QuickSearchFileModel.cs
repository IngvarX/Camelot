
namespace Camelot.Services.Abstractions.Models;

public record QuickSearchFileModel
{
    public string Name { get; init; }
    public object Tag { get; init; }
    public bool Found { get; set; }
    public bool Selected { get; set; }
}
