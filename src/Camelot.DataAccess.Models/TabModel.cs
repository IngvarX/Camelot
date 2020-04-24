namespace Camelot.DataAccess.Models
{
    public class TabModel
    {
        public string Directory { get; set; }
        
        public SortingSettings SortingSettings { get; set; }

        public TabModel()
        {
            SortingSettings = new SortingSettings();
        }
    }
}