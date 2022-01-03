namespace Camelot.DataAccess.Configuration;

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; }

    public bool UseInMemoryDatabase { get; set; }
}