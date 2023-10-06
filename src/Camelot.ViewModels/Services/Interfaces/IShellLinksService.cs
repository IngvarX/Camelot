namespace Camelot.ViewModels.Services.Interfaces;

// Shell link is not same as files system link.
// On Windows, this is ".lnk" file which is maintained in shell level,
// not FileType.Link which is in files system level.
public interface IShellLinksService
{
    bool IsShellLink(string path);
    
    string ResolveLink(string path);
}