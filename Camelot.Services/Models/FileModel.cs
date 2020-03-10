using System;

namespace Camelot.Services.Models
{
    public class FileModel
    {
        public string Name { get; set; }

        public int SizeBytes { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public NodeType Type { get; set; }
    }
}