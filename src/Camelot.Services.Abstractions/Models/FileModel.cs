using System;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models
{
    public class FileModel : ModelBase
    {
        public long SizeBytes { get; set; }

        public FileType Type { get; set; }

        public string Extension { get; set; }

        public DateTime LastWriteTime { get; set; }
    }
}