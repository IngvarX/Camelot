using System.Text.RegularExpressions;

namespace Camelot.Services.Environment.Interfaces
{
    public interface IRegexService
    {
        bool ValidateRegex(string regex);

        bool CheckIfMatches(string input, string pattern, RegexOptions options);
    }
}