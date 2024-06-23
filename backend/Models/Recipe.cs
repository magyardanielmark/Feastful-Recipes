namespace backend.Models;
using System.ComponentModel.DataAnnotations;


public class Recipe
{
    public int Id { get; }
    
    [StringLength(200)] 
    public string Title { get; init; }
    
    public List<string> Ingredients { get; init; }
    
    public List<string> Directions { get; init; }
    
    [StringLength(200)]
    public string? Image { get; init; }
    
    
    public Recipe(int id,string title, List<string> ingredients, List<string> directions, string image)
    {
        Id = id;
        Title = title;
        Ingredients = ingredients;
        Directions = directions;
        Image = image;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Recipe otherRecipe) return false;
        if (ReferenceEquals(this, otherRecipe)) return true;

        return otherRecipe.Id == this.Id && otherRecipe.Title == this.Title 
                                         && otherRecipe.Ingredients.SequenceEqual(this.Ingredients) 
                                         && otherRecipe.Directions.SequenceEqual(this.Directions);
    
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Title, Ingredients, Directions,Image);
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Ingredients: {string.Join(",",Ingredients)}, Directions: {string.Join(",",Directions)}, ImageURL: {Image}";
    }
}

