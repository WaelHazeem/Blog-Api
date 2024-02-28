using BlogApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogApi.Services;

public class CategoriesService
{
    private readonly IMongoCollection<Category> _categoriesCollection;

    public CategoriesService(
        IOptions<BlogDatabaseSettings> blogDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            blogDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogDatabaseSettings.Value.DatabaseName);

        _categoriesCollection = mongoDatabase.GetCollection<Category>(
            blogDatabaseSettings.Value.CategoriesCollectionName);
    }

    public async Task<List<Category>> GetAsync() =>
        await _categoriesCollection.Find(_ => true).ToListAsync(); 
    public async Task<List<Category>> AdminGetAsync() =>
        await _categoriesCollection.Find(_ => true).ToListAsync(); 
    public async Task<Category?> GetAsync(string id) =>
        await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<Category?> GetWithSlugAsync(string slug) =>
        await _categoriesCollection.Find(x => x.Slug!["en"] == slug|| x.Slug!["ar"] == slug).FirstOrDefaultAsync();
    public    Category GetSync(string id) =>
          _categoriesCollection.Find(x => x.Id == id).FirstOrDefault();

    public async Task CreateAsync(Category newCategory) =>
        await _categoriesCollection.InsertOneAsync(newCategory);

 
    public async Task UpdateAsync(string id, Category updatedCategory) =>
        await _categoriesCollection.ReplaceOneAsync(x => x.Id == id, updatedCategory);

    public async Task RemoveAsync(string id) =>
        await _categoriesCollection.DeleteOneAsync(x => x.Id == id);
}