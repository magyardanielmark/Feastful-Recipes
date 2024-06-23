namespace backend.Services.Repositories.Recipe;
using Data;
using Microsoft.EntityFrameworkCore;



public class RecipeRepository : IRecipeRepository
{
    private readonly FeastfulDbContext _feastfulDbContext;
    private readonly ILogger<RecipeRepository> _logger;

    public RecipeRepository(FeastfulDbContext feastfulDbContext, ILogger<RecipeRepository> logger)
    {
        _feastfulDbContext = feastfulDbContext;
        _logger = logger;
    }


    public async Task<Models.Recipe> CreateRecipe(Models.Recipe recipe)
    {
        try
        {
            var newRecipe = await _feastfulDbContext.Recipes.AddAsync(recipe);
            await _feastfulDbContext.SaveChangesAsync();
            return newRecipe.Entity;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a recipe: {Recipe}", recipe);
            throw new Exception("Cannot create new Recipe.");
        }
    }

    public async Task<Models.Recipe?> GetRecipeById(int id)
    {
        try
        {
            var result = await _feastfulDbContext.Recipes.FirstOrDefaultAsync(recipe => recipe.Id == id);
            if (result == null) throw new KeyNotFoundException("Recipe not found.");

            return result;
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "Recipe with ID {RecipeId} not found.", id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching recipe with ID {RecipeId}", id);
            throw new Exception("An unexpected error occured, cannot get recipe.");
        }
    }

    public async Task<Models.Recipe?> GetRecipeByTitle(string title)
    {
        try
        {
            var result = await _feastfulDbContext.Recipes.FirstOrDefaultAsync(recipe => recipe.Title.ToLower().Contains(title.ToLower()));
            if (result == null)
            {
                throw new KeyNotFoundException($"Recipe with title '{title}' not found.");
            }

            return result;
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "Recipe with title '{RecipeTitle}' not found.", title);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching recipe with title '{RecipeTitle}'", title);
            throw new Exception("An unexpected error occured, cannot get recipe.");
        }
    }

    public async Task<IEnumerable<Models.Recipe>> GetAllRecipes()
    {
        try
        {
            return await _feastfulDbContext.Recipes.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching all recipes");
            throw new Exception("Cannot get recipes.");
        }
    }

    public async Task<Models.Recipe?> UpdateRecipe(Models.Recipe recipe)
    {
        try
        {
            var existingRecipe = await _feastfulDbContext.Recipes.FirstOrDefaultAsync(c => c.Id == recipe.Id);

            if (existingRecipe == null)
            {
                throw new KeyNotFoundException("Failed to update. Recipe was not found.");
            }
            
            _feastfulDbContext.Entry(existingRecipe).CurrentValues.SetValues(recipe);
            await _feastfulDbContext.SaveChangesAsync();

            return existingRecipe;
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "Recipe with ID {RecipeId} not found while updating.", recipe.Id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating recipe with ID {RecipeId}", recipe.Id);
            throw new Exception("An unexpected error occured. Recipe was not updated.");
        }
    }

    public async Task<bool> DeleteRecipe(int id)
    {
        try
        {
            var recipeToDelete = await _feastfulDbContext.Recipes.FirstOrDefaultAsync(c => c.Id == id);

            if (recipeToDelete == null)
            {
                throw new KeyNotFoundException("Failed to delete. Recipe was not found.");
            }
            
            _feastfulDbContext.Recipes.Remove(recipeToDelete);
            await _feastfulDbContext.SaveChangesAsync();
            return true;
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "Recipe with ID {RecipeId} not found while deleting.", id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting recipe with ID {RecipeId}", id);
            throw new Exception("Recipe was not deleted. An unexpected error occured.");
        }
    
    }
}