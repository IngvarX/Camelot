using System;

namespace Camelot.Services.Abstractions.Models
{
    public class ModelBase
    {
        public string Name { get; set; }

        public string FullPath { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
    }
}