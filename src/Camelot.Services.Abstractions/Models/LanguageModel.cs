namespace Camelot.Services.Abstractions.Models;

public class LanguageModel
{
    public string Name { get; }

    public string NativeName { get; }

    public string Code { get; }

    public LanguageModel(string name, string nativeName, string code)
    {
        Name = name;
        NativeName = nativeName;
        Code = code;
    }
}