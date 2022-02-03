using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models;

public class FileModel : NodeModelBase
{
    public long SizeBytes { get; set; }

    public FileType Type { get; set; }

    public string Extension { get; set; }
}