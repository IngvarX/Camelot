using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions;

public interface IFileTypeMapper
{
    FileMimeType GetFileType(string extension);
}