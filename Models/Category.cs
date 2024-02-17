using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApi.Models;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Dictionary<string, string> Name { get; set; } = null!;
     public Dictionary<string, string>? Slug { get; set; }
    public Dictionary<string, string>? SeoTitle { get; set; } = null!;
    public Dictionary<string, string>? MetaData { get; set; } = null!;
    public Dictionary<string, string>? Keywords { get; set; } = null!;
    public string? ParentCategoryId { get; set; } = null!;
    public Category()
    {
    }
}
 
