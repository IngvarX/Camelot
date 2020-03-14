using System.IO;

namespace Camelot.Services.Models
{
    public class FileModel : ModelBase
    {
        public int SizeBytes { get; set; }

        public FileType Type { get; set; }

        public string Extension => Path.GetExtension(Name);
    }
}