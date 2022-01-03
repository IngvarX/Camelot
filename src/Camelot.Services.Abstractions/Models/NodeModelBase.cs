using System;

namespace Camelot.Services.Abstractions.Models;

public class NodeModelBase
{
    public string Name { get; set; }

    public string FullPath { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime LastModifiedDateTime { get; set; }

    public DateTime LastAccessDateTime { get; set; }
}