using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _categoriesService;
    private readonly ArticlesService _postsService;

    public CategoriesController(CategoriesService categoriesService, ArticlesService postsService)
    {

        _categoriesService = categoriesService;
        _postsService = postsService;
    }

    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public async Task<List<Category>> Get() =>
     await _categoriesService.GetAsync();


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Category>> Get(string id)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return category;
    }

    [HttpGet("GetWithSlug/{slug}")]
    public async Task<ActionResult<Category>> GetWithSlug(string slug)
    {
        var category = await _categoriesService.GetWithSlugAsync(slug);

        if (category is null)
        {
            return NotFound();
        }

        return category;
    }


    async Task<bool> ckeckSlugExisted(Category categoryParam)
    {

        return false;
    }

    [HttpPost]
    public async Task<IActionResult> Category(Category newCategory)
    {

        if (await ckeckSlugExisted(newCategory) == true)
            return BadRequest("Slug value was already existed");

        await _categoriesService.CreateAsync(newCategory);

        return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
    }


    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Category updatedcategory)
    {

        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        updatedcategory.Id = category.Id;
        if (updatedcategory.Slug != category.Slug)
            if (await ckeckSlugExisted(updatedcategory) == true)
                return BadRequest("Slug value was already existed");

        await _categoriesService.UpdateAsync(id, updatedcategory);

        return Ok();
    }


    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        await _categoriesService.RemoveAsync(id);

        return NoContent();
    }
}