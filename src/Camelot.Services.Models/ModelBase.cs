using System;

namespace Camelot.Services.Models
{
    public class ModelBase
    {
        public string Name { get; set; }

        public string FullPath { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
    }
}