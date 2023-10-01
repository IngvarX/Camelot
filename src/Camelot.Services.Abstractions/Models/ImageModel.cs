
namespace Camelot.Services.Abstractions.Models;

// Motivation for abstract class:
// System.Drawing is only for Windows,
// and Avalonia is not referenced in Camelot.Services.Abstractions,
// hence we need an image model which can be used in Camelot.Services.Abstractions.
public abstract class ImageModel
{

}