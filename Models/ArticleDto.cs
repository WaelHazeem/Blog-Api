using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApi.Models;
public class PagedArticleDto
{
    public int totalCount;
    public List<ArticleDto> items;
    public PagedArticleDto()
    {

    }
}
public class ArticleDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Slug { get; set; }
    public  ArticleState? State { get; set; }  

    public Dictionary<string, string> Title { get; set; } = null!;
    public Dictionary<string, string>? _Slug { get; set; }
    public Dictionary<string, string> MetaData { get; set; } = null!;
    public Dictionary<string, string> Description { get; set; } = null!;
    public Dictionary<string, string> Content { get; set; } = null!;
    public string Category { get; set; } = null!;

    public string Image { get; set; } = null!;
    public string? PortraitImage { get; set; } = null!;

    // public string ImageBase64Data { get; set; } = null!;

    public DateTime? CreationTime { get; set; }
    public string? CreatorId { get; set; }
    public string? Creator { get; set; }
    public ArticleDto()
    {

    }
    public static ArticleDto ArticleDtoFromArticle(Article article  ,bool withState = true)
    {
        ArticleDto articleDto = new ArticleDto();
        articleDto.Id = article.Id;
        // articleDto.Slug = article.Slug;
        articleDto._Slug = article.Slug;
        articleDto.Title = article.Title;
        articleDto.Description = article.Description;
        articleDto.CreationTime = article.CreationTime;
        articleDto.Category = article.CategoryId;

          if (withState)
        {
        articleDto.State = article.State;

         }
        return articleDto;
    }
}