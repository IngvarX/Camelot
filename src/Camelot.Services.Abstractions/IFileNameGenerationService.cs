namespace Camelot.Services.Abstractions
{
    public interface IFileNameGenerationService
    {
        string GenerateFullName(string filePath);

        string GenerateFullNameWithoutExtension(string filePath);

        string GenerateName(string initialName, string directory);
    }
}