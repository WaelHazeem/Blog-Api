namespace BlogApi.Models;

public class BlogDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string ArticlesCollectionName { get; set; } = null!;
    public string CategoriesCollectionName { get; set; } = null!;
}