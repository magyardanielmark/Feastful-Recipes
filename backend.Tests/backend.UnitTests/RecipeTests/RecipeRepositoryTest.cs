using backend.Data;
using backend.Models;
using backend.Services.Repositories.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.UnitTests.RecipeTests;


[TestFixture]
public class RecipeRepositoryTest
{
    private DbContextOptions<FeastfulDbContext> _contextOptions;
    private ILogger<RecipeRepository> _loggerMock;
    private FeastfulDbContext _context;
    private IRecipeRepository _repository;
    private IEnumerable<Recipe> _seededRecipes;
    
    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<RecipeRepository>>().Object;
        _contextOptions = new DbContextOptionsBuilder<FeastfulDbContext>()
            .UseInMemoryDatabase("recipes")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        _context = new FeastfulDbContext(_contextOptions);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        _seededRecipes = new List<Recipe>
        {
            new Recipe(
                1,
                "Creamy Mushroom Risotto",
                new List<string> { "Arborio rice", "Mushrooms", "Butter", "Parmesan cheese", "Broth" },
                new List<string> { "Sauté mushrooms in butter", "Toast rice", "Add broth gradually", "Stir in parmesan" },
                "https://example.com/risotto.jpg"
            ),
            new Recipe(
                2,
                "Spicy Black Bean Burgers",
                new List<string> { "Black beans", "Oats", "Onion", "Garlic", "Spices" },
                new List<string> { "Mash beans", "Mix with oats, onion, garlic, and spices", "Form patties", "Cook on grill or pan" },
                "https://example.com/blackbeanburger.jpg"
            ),
            new Recipe(
                3,
                "Fresh Mango Salsa",
                new List<string> { "Mango", "Red onion", "Cilantro", "Jalapeno", "Lime juice" },
                new List<string> { "Dice mango, onion, and jalapeno", "Chop cilantro", "Combine ingredients", "Add lime juice" },
                "https://example.com/mangosalsa.jpg"
            )
        };
        
        _context.Recipes.AddRange(_seededRecipes);

        _context.SaveChangesAsync();
        
        _repository = new RecipeRepository(_context,_loggerMock);
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }


    [Test]
    public async Task GetAllRecipes_Returns_AllRecipes()
    {
        var result = await _repository.GetAllRecipes();
        
        Assert.That(result, Is.EquivalentTo(_seededRecipes)); 
    }

    [Test]
    public async Task GetRecipeById_Returns_CorrectRecipe()
    {
        var result = await _repository.GetRecipeById(1);
        
        Assert.That(result, Is.EqualTo(_seededRecipes.FirstOrDefault(r => r.Id == 1))); 
    }
    
    [Test]
    public async Task GetRecipeByTitle_Returns_CorrectRecipe()
    {
        var expectedResult = _seededRecipes.First();
       
        var result = await _repository.GetRecipeByTitle("Creamy Mushroom Risotto");
        
        Assert.That(result,Is.EqualTo(expectedResult));
    }
    
    [Test]
    public async Task CreateRecipe_AddsRecipeToDatabase()
    {
        var expectedResult = new Recipe(
            4,
            "Chicken Noodle Soup",
            new List<string> { "Chicken broth", "Egg noodles", "Cooked chicken", "Carrots", "Celery", "Onion" },
            new List<string> { "Simmer broth with vegetables", "Add chicken and noodles", "Cook until noodles are tender" },
            "https://example.com/chickennoodlesoup.jpg"
        );
        await _repository.CreateRecipe(expectedResult);
        
        var recipes = await _repository.GetAllRecipes();
        
        Assert.That(recipes, Has.Member(expectedResult));
    }
    
    [Test]
    public async Task UpdateRecipe_UpdatesRecipeInDatabase()
    {
        var existingRecipe = _seededRecipes.First();
        var updatedRecipe = new Recipe(
            existingRecipe.Id,
            "Updated Recipe Title", 
            new List<string> { "New Ingredient 1", "New Ingredient 2" },
            new List<string> { "New Step 1", "New Step 2" },
            "https://example.com/updatedimage.jpg"
        );
        var result = await _repository.UpdateRecipe(updatedRecipe);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(updatedRecipe)); 

        
        var updatedFromDb = await _repository.GetRecipeById(result.Id);
        Assert.That(updatedFromDb, Is.EqualTo(updatedRecipe));
    }
    
    [Test]
    public Task UpdateRecipe_ThrowsKeyNotFoundException_IfRecipeDoesNotExist()
    {
        
        var nonExistentRecipe = new Recipe(999, "Nonexistent Recipe", new List<string>(), new List<string>(), "");
        
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.UpdateRecipe(nonExistentRecipe));
        return Task.CompletedTask;
    }

    
    [Test]
    public async Task DeleteRecipe_RemovesRecipeFromDatabase()
    {
        var recipes = await _repository.GetAllRecipes();
        
        await _repository.DeleteRecipe(1);
        var recipesAgain = await _repository.GetAllRecipes();
        
        Assert.That(recipesAgain.Count(),Is.EqualTo(recipes.Count() - 1));
    }
}