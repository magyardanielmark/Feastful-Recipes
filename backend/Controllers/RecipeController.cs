using backend.Services.Repositories.Recipe;

namespace backend.Controllers;
using Models;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/v1/recipes")]
public class RecipeController : ControllerBase
{
    private readonly ILogger<RecipeController> _logger;
    private readonly IRecipeRepository _recipeRepository;

    public RecipeController(ILogger<RecipeController> logger, IRecipeRepository recipeRepository)
    {
        _logger = logger;
        _recipeRepository = recipeRepository;
    }
    
     // GET: api/v1.0/recipes
    [HttpGet]
    public async Task<ActionResult> GetAllRecipes()
    {
        try
        {
            var recipes = await _recipeRepository.GetAllRecipes();
            return Ok(new { message = "Recipes found successfully.", data = recipes });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving recipes.");
            return NotFound(new { message = "An error occurred while retrieving recipes." });
        }
    }

    // GET: api/v1.0/recipes/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetRecipeById(int id)
    {
        try
        {
            var recipe = await _recipeRepository.GetRecipeById(id);
            if (recipe == null)
            {
                return NotFound(new { message = "Recipe not found." });
            }
            return Ok(new { message = "Recipe found successfully.", data = recipe });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving the recipe with ID {Id}.", id);
            return NotFound( new { message = $"An error occurred while retrieving the recipe with ID {id}." });
        }
    }
    
    // GET: api/v1.0/recipes/{title}
    [HttpGet("title")]
    public async Task<ActionResult> GetRecipeByTitle([FromQuery]string title)
    {
        try
        {
            var recipe = await _recipeRepository.GetRecipeByTitle(title);
            if (recipe == null)
            {
                return NotFound(new { message = "Recipe not found." });
            }
            return Ok(new { message = "Recipe found successfully.", data = recipe });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving the recipe with Title {title}.", title);
            return NotFound( new { message = $"An error occurred while retrieving the recipe with Title {title}." });
        }
    }

    // POST: api/v1.0/recipes
    [HttpPost]
    public async Task<ActionResult> CreateRecipe(Recipe recipe)
    {
        try
        {
            var newRecipe = await _recipeRepository.CreateRecipe(recipe);
            return CreatedAtAction(nameof(GetRecipeById), new { id = newRecipe.Id, version = "1.0" }, new { message = "Recipe created successfully.", data = newRecipe });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating the recipe.");
            return BadRequest( new { message = "An error occurred while creating the recipe." });
        }
    }

    // PUT: api/v1.0/recipes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return BadRequest(new { message = "The ID in the request body does not match the ID in the URL." });
        }

        try
        {
            var updatedRecipe = await _recipeRepository.UpdateRecipe(recipe);
            if (updatedRecipe == null)
            {
                return NotFound(new { message = "Recipe not found." });
            }
            return NoContent(); 
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating the recipe with ID {Id}.", id);
            return BadRequest( new { message = $"An error occurred while updating the recipe with ID {id}." });
        }
    }

    // DELETE: api/v1.0/recipes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        try
        {
            var result = await _recipeRepository.DeleteRecipe(id);
            if (!result)
            {
                return NotFound(new { message = "Recipe not found." });
            }
            return NoContent(); 
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting the recipe with ID {Id}.", id);
            return StatusCode(500, new { message = $"An error occurred while deleting the recipe with ID {id}." });
        }
    }
}