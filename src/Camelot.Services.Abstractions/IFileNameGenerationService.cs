namespace Camelot.Services.Abstractions
{
    public interface IFileNameGenerationService
    {
        string GenerateName(string initialName, string directory);
    }
}