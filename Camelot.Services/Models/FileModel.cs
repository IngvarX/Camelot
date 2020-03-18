using System;
using System.IO;

namespace Camelot.Services.Models
{
    public class FileModel : ModelBase
    {
        public long SizeBytes { get; set; }

        public FileType Type { get; set; }

        public string Extension => Path.GetExtension(Name);

        public DateTime LastWriteTime { get; set; }
    }
}