using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux
{
    public class MimeTypesReader : IMimeTypesReader
    {
        public async Task<IReadOnlyDictionary<string, List<string>>> ReadAsync(Stream stream)
        {
            var data = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            using var streamReader = new StreamReader(stream);
            while (streamReader.Peek() != -1)
            {
                var rawLine = await streamReader.ReadLineAsync();
                var line = rawLine?.Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!char.IsLetter(line[0]))
                {
                    continue;
                }

                var mimeTypeInfo = line.Split(new[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);

                var key = mimeTypeInfo[0];
                if (!key.Contains("/"))
                {
                    continue;
                }

                var values = mimeTypeInfo.Skip(1);

                if (data.ContainsKey(key))
                {
                    data[key].AddRange(values);
                }
                else
                {
                    data[key] = new List<string>(values);
                }
            }

            return data;
        }
    }
}