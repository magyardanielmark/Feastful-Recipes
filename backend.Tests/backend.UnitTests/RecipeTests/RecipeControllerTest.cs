using backend.Controllers;
using backend.Models;
using backend.Services.Repositories.Recipe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.UnitTests.RecipeTests;

[TestFixture]
public class RecipeControllerTest
{
    private Mock<ILogger<RecipeController>> _loggerMock;
    private Mock<IRecipeRepository> _recipeRepositoryMock;
    private RecipeController _controller;
    private List<Recipe> _recipes;
    
    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<RecipeController>>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _controller = new RecipeController(_loggerMock.Object, _recipeRepositoryMock.Object);
        _recipes = new List<Recipe> { new Recipe(1,
            "Creamy Mushroom Risotto",
            new List<string> { "Arbor rice", "Mushrooms", "Butter", "Parmesan cheese", "Broth" },
            new List<string> { "Sauté mushrooms in butter", "Toast rice", "Add broth gradually", "Stir in parmesan" },
            "https://example.com/risotto.jpg")};
    }
    
    [Test]
    public async Task GetAllRecipes_ReturnsOkWithRecipes()
    {
        
        _recipeRepositoryMock.Setup(x => x.GetAllRecipes()).ReturnsAsync(_recipes);

        var result = await _controller.GetAllRecipes();
        
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetRecipeById_ReturnsOkWithRecipe()
    {
        _recipeRepositoryMock.Setup(x => x.GetRecipeById(1)).ReturnsAsync(_recipes[0]);
        
        var result = await _controller.GetRecipeById(_recipes[0].Id);
        
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetRecipeById_ReturnsNotFoundIfRecipeDoesNotExist()
    {
        var result = await _controller.GetRecipeById(999);
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
    
    [Test]
    public async Task GetRecipeByTitle_ReturnsOkWithRecipe()
    {
        _recipeRepositoryMock.Setup(x => x.GetRecipeByTitle(_recipes[0].Title)).ReturnsAsync(_recipes[0]);
        
        var result = await _controller.GetRecipeByTitle(_recipes[0].Title);
        
        Assert.IsInstanceOf<OkObjectResult>(result);
    }
    
    [Test]
    public async Task GetRecipeByTitle_ReturnsNotFoundIfRecipeDoesNotExist()
    {
        var result = await _controller.GetRecipeByTitle("something");
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
    
      [Test]
    public async Task CreateRecipe_ReturnsCreatedAtActionOnSuccess()
    {
        var newRecipe = new Recipe(4, "New Recipe", new List<string>(), new List<string>(), "");

        _recipeRepositoryMock.Setup(x => x.CreateRecipe(newRecipe)).ReturnsAsync(newRecipe);

        var result = await _controller.CreateRecipe(newRecipe);

        Assert.IsInstanceOf<CreatedAtActionResult>(result);
        var createdAtActionResult = (CreatedAtActionResult)result;
        Assert.That(createdAtActionResult.ActionName, Is.EqualTo("GetRecipeById"));
        Assert.That(createdAtActionResult.RouteValues?["id"], Is.EqualTo(newRecipe.Id));
    }
    
    [Test]
    public async Task CreateRecipe_ReturnsBadRequestOnError()
    {
        var newRecipe = new Recipe(4, "New Recipe", new List<string>(), new List<string>(), "");
        _recipeRepositoryMock.Setup(x => x.CreateRecipe(newRecipe)).ThrowsAsync(new Exception());

        var result = await _controller.CreateRecipe(newRecipe);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }
    
    [Test]
    public async Task UpdateRecipe_ReturnsNoContentOnSuccess()
    {
        var existingRecipe = _recipes[0];
        var updatedRecipe = new Recipe(existingRecipe.Id, "Updated Recipe", new List<string>(), new List<string>(), "");

        _recipeRepositoryMock.Setup(x => x.UpdateRecipe(updatedRecipe)).ReturnsAsync(updatedRecipe);

        var result = await _controller.UpdateRecipe(existingRecipe.Id, updatedRecipe);

        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task UpdateRecipe_ReturnsBadRequestIfIdMismatch()
    {
        var existingRecipe = _recipes[0];
        var updatedRecipe = new Recipe(999, "Updated Recipe", new List<string>(), new List<string>(), ""); 

        var result = await _controller.UpdateRecipe(existingRecipe.Id, updatedRecipe);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }
    
    [Test]
    public async Task UpdateRecipe_ReturnsNotFoundIfRecipeDoesNotExist()
    {
        var nonExistentRecipe = new Recipe(999, "Nonexistent Recipe", new List<string>(), new List<string>(), ""); 

        _recipeRepositoryMock.Setup(x => x.UpdateRecipe(nonExistentRecipe)).ReturnsAsync((Recipe)null!);

        var result = await _controller.UpdateRecipe(nonExistentRecipe.Id, nonExistentRecipe);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
    
    [Test]
    public async Task UpdateRecipe_ReturnsBadRequestOnError()
    {
        var existingRecipe = _recipes[0];
        var updatedRecipe = new Recipe(existingRecipe.Id, "Updated Recipe", new List<string>(), new List<string>(), "");

        _recipeRepositoryMock.Setup(x => x.UpdateRecipe(updatedRecipe)).ThrowsAsync(new Exception());

        var result = await _controller.UpdateRecipe(existingRecipe.Id, updatedRecipe);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task DeleteRecipe_ReturnsNoContentOnSuccess()
    {
        _recipeRepositoryMock.Setup(x => x.DeleteRecipe(1)).ReturnsAsync(true);

        var result = await _controller.DeleteRecipe(1); 

        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task DeleteRecipe_ReturnsNotFoundIfRecipeDoesNotExist()
    {
        _recipeRepositoryMock.Setup(x => x.DeleteRecipe(999)).ReturnsAsync(false);

        var result = await _controller.DeleteRecipe(999);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
    
    [Test]
    public async Task DeleteRecipe_ReturnsInternalServerErrorOnError()
    {
        _recipeRepositoryMock.Setup(x => x.DeleteRecipe(1)).ThrowsAsync(new Exception());

        var result = await _controller.DeleteRecipe(1);

        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }
}