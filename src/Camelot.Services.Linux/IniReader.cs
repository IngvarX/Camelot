using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux
{
    public class IniReader : IIniReader
    {
        private const string KeyDelimiter = ":";
        
        public async Task<IReadOnlyDictionary<string, string>> ReadAsync(Stream stream)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using var reader = new StreamReader(stream);
            var sectionPrefix = string.Empty;

            while (reader.Peek() != -1)
            {
                var rawLine = await reader.ReadLineAsync();
                var line = rawLine?.Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                switch (line[0])
                {
                    case ';':
                    case '#':
                    case '/':
                        continue;
                    case '[' when line[^1] == ']':
                        sectionPrefix = line.Substring(1, line.Length - 2) + KeyDelimiter;
                        continue;
                }

                var separator = line.IndexOf('=');
                if (separator < 0)
                {
                    continue;
                }

                var key = sectionPrefix + line.Substring(0, separator).Trim();
                var value = line.Substring(separator + 1).Trim();

                if (value.Length > 1 && value[0] == '"' && value[^1] == '"')
                {
                    value = value.Substring(1, value.Length - 2);
                }

                data[key] = value;
            }

            return data;
        }
    }
}