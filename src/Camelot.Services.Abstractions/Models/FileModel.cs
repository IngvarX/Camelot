using System;

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