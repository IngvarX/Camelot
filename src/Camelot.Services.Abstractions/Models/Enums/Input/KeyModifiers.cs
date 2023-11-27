using System;

namespace Camelot.Services.Abstractions.Models.Enums.Input;

/// <summary>
/// Needed to be duplicated here, since nor Avalonia, nor Windows.Forms, etc
/// are referenced in "Camelot.Services.Abstractions"
/// Used in feature of 'quick search' to determine if shift key is down.
/// </summary>

[Flags]
public enum KeyModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Meta = 8,
}
