using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class RegexService : IRegexService
    {
        public bool ValidateRegex(string regex)
        {
            if (string.IsNullOrEmpty(regex))
            {
                return false;
            }

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Regex(regex);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public bool CheckIfMatches(string input, string pattern, RegexOptions options) =>
            Regex.IsMatch(input, pattern, options);

        public IList<Match> GetMatches(string input, string pattern, RegexOptions options) =>
            Regex.Matches(input, pattern, options);
    }
}