using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BlogApi.Models;
 
  public enum ArticleState
    {
        Draft=0,
        Scheduled=1,
        Published=2,
    }
public class Article
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public ArticleState? State { get; set; } = ArticleState.Draft;
    public DateTime? ScheduledDate   { get; set; }  
    public Dictionary<string, string> Title { get; set; } = null!;
    public Dictionary<string, string> Description { get; set; } = null!;
    public Dictionary<string, string> Content { get; set; } = null!;
    public Dictionary<string, string>? Slug { get; set; }
    public Dictionary<string, string> SeoTitle { get; set; } = null!;
    public Dictionary<string, string> MetaData { get; set; } = null!;
    public Dictionary<string, string>? Keywords   { get; set; } 
    public string? CategoryId { get; set; } = null!;
    public string? LandscapeImage { get; set; } = null!;
    public string? PortraitImage { get; set; } = null!;

    public DateTime? CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public DateTime? DeletionTime { get; set; }
    public  List<string> ? RelatedArticles { get; set; } = null!;
    public Article()
    {
    }

}