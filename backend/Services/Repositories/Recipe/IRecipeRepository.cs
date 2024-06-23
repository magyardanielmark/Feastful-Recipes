namespace backend.Services.Repositories.Recipe;

public interface IRecipeRepository
{
    Task<Models.Recipe> CreateRecipe(Models.Recipe recipe); 
    
    Task<Models.Recipe?> GetRecipeById(int id);
    
    Task<Models.Recipe?> GetRecipeByTitle(string title);
    
    Task<IEnumerable<Models.Recipe>> GetAllRecipes();
    
    Task<Models.Recipe?> UpdateRecipe(Models.Recipe recipe);
    
    Task<bool> DeleteRecipe(int id); 
}