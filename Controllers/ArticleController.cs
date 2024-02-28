using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ArticlesController : ControllerBase
{
    private readonly ArticlesService _articlesService;
    private readonly CategoriesService _categoriesService;


    public ArticlesController(ArticlesService articlesService,
     CategoriesService categoriesService)
    {
        _articlesService = articlesService;
        _categoriesService = categoriesService;

    }


    [HttpGet("GetArticles")]
    public async Task<List<ArticleDto>> GetArticles(int pageNumber = 1, int pageSize = 4, string? category = null)
    {
        var articles = await _articlesService.GetAsync("", "", pageNumber, pageSize, category);
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        foreach (Article article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(article);

                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {

            }
        }
        //var res = articles.Where(i => i.DisplayInSlider == true).ToList();
        return ArticleDtos;
    }

    [HttpGet("GetLatestArticles")]
    [OutputCache(PolicyName = "ExpireAfterHour")]
    [ResponseCache(Duration = 86400)]
    public async Task<List<ArticleDto>> GetLatestArticles()
    {
        var articles = await _articlesService.GetLatestAsync();
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        foreach (Article article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(article);
                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {
            }
        }
        return ArticleDtos;
    }

    [HttpGet("GetBriefArticles")]
    public async Task<object> GetBriefArticles(string? Keyword = "", ArticleState? State = null, DateOnly? scheduledDate = null, int? PageNumber = 1, int? MaxResultCount = 100)
    {
        var articles = await _articlesService.GetBriefArticles(Keyword, State, scheduledDate, PageNumber, MaxResultCount);

        return new { totalCount = articles.totalCount, items = articles.items };
    }

    [HttpGet]
    public async Task<List<Article>> Get() =>
     await _articlesService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Article>> Get(string id)
    {
        var article = await _articlesService.GetAsync(id);

        if (article is null)
        {
            return NotFound();
        }


        return article;
    }
    [HttpGet("GetRandomArticles/{id:length(24)}")]
    public async Task<ActionResult<List<ArticleDto>>> GetRandomArticles(string id)
    {
        var articles = await _articlesService.Get3RandomAsync(id);
        if (articles is null)
        {
            return NoContent();
        }
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        foreach (Article _article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(_article);
                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {
            }
        }
        return ArticleDtos;
    }


    [HttpGet("GetArticleList")]
    public async Task<List<ArticleDto>> GetArticleList([FromQuery] string[] slugs)
    {
        var articles = await _articlesService.GetArticleListAsync(slugs);
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        foreach (Article article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(article);
                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {
            }
        }
        return ArticleDtos;
    }

    [HttpGet("GetCategoryArticles")]
    public async Task<ActionResult<List<ArticleDto>>> GetCategoryArticles
       (string slug, int? pageNumber = 1, int? pageSize = 6)
    {
        var category = await _categoriesService.GetWithSlugAsync(slug);

        if (category is null)
        {
            return NotFound();
        }
        var articles = await _articlesService.GetAsync("", "", pageNumber, pageSize, category.Id);
        List<ArticleDto> ArticleDtos = new List<ArticleDto>();
        foreach (Article article in articles)
        {
            try
            {
                ArticleDto articleDto = ArticleDto.ArticleDtoFromArticle(article);
                ArticleDtos.Add(articleDto);
            }
            catch (Exception ex)
            {
            }
        }
        return ArticleDtos;
    }

    [HttpGet("GetWithSlug/{slug}")]
    public async Task<ActionResult<Article>> GetWithSlug(string slug)
    {
        var article = await _articlesService.GetWithSlugAsync(slug);

        if (article is null)
        {
            return NotFound();
        }


        return article;
    }

    private const string SessionKeyName = "_UserID";

    [HttpPost]
    public async Task<IActionResult> Article(Article newArticle)
    {
        newArticle.CreationTime = System.DateTime.Now;
        newArticle.Slug!["ar"] = newArticle.Slug!["ar"].Trim().Replace(' ', '-');
        newArticle.Slug!["en"] = newArticle.Slug!["en"].Trim().Replace(' ', '-');

        SaveImagesToContent(newArticle);
        await _articlesService.CreateAsync(newArticle);

        return CreatedAtAction(nameof(Get), new { id = newArticle.Id }, newArticle);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Article updatedarticle)
    {

        var article = await _articlesService.GetAsync(id);

        if (article is null)
        {
            return NotFound();
        }
        updatedarticle.Id = article.Id;
        SaveImagesToContent(updatedarticle);
        updatedarticle.Slug!["ar"] = updatedarticle.Slug!["ar"].Trim().Replace(' ', '-');
        updatedarticle.Slug!["en"] = updatedarticle.Slug!["en"].Trim().Replace(' ', '-');
        await _articlesService.UpdateAsync(id, updatedarticle);

        return Ok();
    }
    [HttpGet("SaveImagesToContent")]
    public void SaveImagesToContent(Article newArticle)
    {
        string featuredImage = ImageHelper.SaveArticleImage(newArticle.LandscapeImage);
        newArticle.LandscapeImage = featuredImage;

        string portraitFeaturedImage = ImageHelper.SaveArticleImage(newArticle.PortraitImage);
        newArticle.PortraitImage = portraitFeaturedImage;


    }
    [HttpPut("UpdateCategory")]


    public async Task<IActionResult> UpdateCategory(string id, string newCategory = "")
    {

        var article = await _articlesService.GetAsync(id);

        if (article is null)
        {
            return NotFound();
        }
        article.CategoryId = newCategory;
        await _articlesService.UpdateAsync(id, article);
        return Ok();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var article = await _articlesService.GetAsync(id);

        if (article is null)
        {
            return NotFound();
        }
        await _articlesService.RemoveAsync(id);
        return NoContent();
    }

}