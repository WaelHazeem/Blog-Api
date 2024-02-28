using BlogApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
namespace BlogApi.Services;

public class ArticlesService
{
    private readonly IMongoCollection<Article> _articlesCollection;

    public ArticlesService(
        IOptions<BlogDatabaseSettings> blogDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            blogDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogDatabaseSettings.Value.DatabaseName);

        _articlesCollection = mongoDatabase.GetCollection<Article>(
            blogDatabaseSettings.Value.ArticlesCollectionName);
    }
    public async Task<PagedArticleDto> GetBriefArticles(
            string keyword = "", ArticleState? state = null, DateOnly? scheduledDate = null, int? pageNumber = 1, int? pageSize = 100, string? category = null)
    {
        StringComparison comp = StringComparison.OrdinalIgnoreCase;
        List<Article> articles;
        PagedArticleDto pagedArticleDto = new PagedArticleDto();

        articles = _articlesCollection.Find(article => (category == null || ((category != null) && category == article.CategoryId))
          ).ToList();

        if (state != null)
        {
            articles = articles.Where(article => (article.Title["en"].Contains(keyword, comp) == true || article.Title["ar"].Contains(keyword, comp) == true)
              && article.State == state).ToList();

            if (state == ArticleState.Scheduled && scheduledDate != null)
                articles = articles.Where(article => article.ScheduledDate != null &&
             DateOnly.FromDateTime((DateTime)article.ScheduledDate) >= scheduledDate).ToList();
        }
        else
        {
            articles = articles.Where(article => (article.Title["en"].Contains(keyword, comp) == true || article.Title["ar"].Contains(keyword, comp) == true))
            .ToList();
        }

        pagedArticleDto.totalCount = articles.Count;
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        articles = articles.Skip((int)((pageNumber - 1) * pageSize))
             .Take((int)pageSize).ToList();
        foreach (Article article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(article, false);
                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {

            }
        }
        pagedArticleDto.items = ArticleDtos;
        return pagedArticleDto;
    }

    public async Task<List<Article>> GetArticleListAsync(string[] slugsList)
    {
        List<Article> articles;
        articles = _articlesCollection.Find(p => p.Slug!["ar"] != "" && slugsList.Contains(p.Slug!["ar"])).ToList();
        return articles;
    }
    public async Task<List<Article>> GetLatestAsync()
    {
        List<Article> articles;
        articles = _articlesCollection.Find(article => article.State == ArticleState.Published || (article.State == ArticleState.Scheduled && article.ScheduledDate <= DateTime.Now))
         .SortByDescending(e => e.CreationTime)
        .Limit(3)
        .ToList();
        return articles;
    }
    public async Task<List<Article>> GetAllAsync()
    {
        List<Article> articles = _articlesCollection.Find(article => true).ToList();
        return articles;
    }
    public int countArticlesForCategory(string categoryId)
    {
        List<Article> articles = _articlesCollection.Find(article => article.CategoryId == categoryId).ToList();
        return articles.Count;
    }

    public async Task<List<Article>> GetAsync(
    string keyword = "", String state = "", int? pageNumber = 1, int? pageSize = 100, string? category = null)
    {
        StringComparison comp = StringComparison.OrdinalIgnoreCase;
        List<Article> articles;
        articles = _articlesCollection.Find(article =>
         (article.State == ArticleState.Published || (article.State == ArticleState.Scheduled && article.ScheduledDate <= DateTime.Now))
          && (category == null || ((category != null) && category == article.CategoryId))
              ).Skip((pageNumber - 1) * pageSize)
              .Limit(pageSize).ToList();



        if (state != "")
            articles = articles.Where(article => (article.Title["en"].Contains(keyword, comp) == true || article.Title["ar"].Contains(keyword, comp) == true)
              && (article.State + "") == state).ToList();
        else
        {
            articles = articles.Where(article => (article.Title["en"].Contains(keyword, comp) == true || article.Title["ar"].Contains(keyword, comp) == true))
            .ToList();
        }
        return articles;
    }
    public async Task<List<Article>> Get3RandomAsync(string id)
    {
        var displayedArticle = await GetAsync(id);

        if (displayedArticle is null)
        {
            return null;
        }
        List<Article> articles;
        articles = _articlesCollection.Find(article => displayedArticle.CategoryId == article.CategoryId
        && id != article.Id
        && (article.State == ArticleState.Published || (article.State == ArticleState.Scheduled && article.ScheduledDate <= DateTime.Now))).ToList()
        .OrderBy(arg => Guid.NewGuid()).Take(3).ToList();
        return articles;
    }


    public async Task<Article?> GetAsync(string id) =>
        await _articlesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<Article?> GetWithSlugAsync(string slug) =>
        await _articlesCollection.Find(x => x.Slug!["ar"] == slug || x.Slug!["en"] == slug || x.Slug!["fr"] == slug).FirstOrDefaultAsync();
    public Article GetSync(string id) =>
          _articlesCollection.Find(x => x.Id == id).FirstOrDefault();

    public async Task CreateAsync(Article newArticle) =>
        await _articlesCollection.InsertOneAsync(newArticle);


    public async Task UpdateAsync(string id, Article updatedArticle) =>
        await _articlesCollection.ReplaceOneAsync(x => x.Id == id, updatedArticle);

    public async Task RemoveAsync(string id) =>
        await _articlesCollection.DeleteOneAsync(x => x.Id == id);
}