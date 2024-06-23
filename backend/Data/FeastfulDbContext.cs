namespace backend.Data;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;


public class FeastfulDbContext : DbContext
{
    public FeastfulDbContext(DbContextOptions<FeastfulDbContext> options) : base(options)
    {
    }
    
    public DbSet<Recipe> Recipes { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<List<string>, string>(
            v => JsonConvert.SerializeObject(v), 
            v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>() 
        );
       

        modelBuilder.Entity<Recipe>()
            .ToTable("recipes")
            .Property(e => e.Ingredients)
            .HasConversion(converter)
            .HasColumnName("ingredients")
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => new HashSet<string>(c1!).SetEquals(new HashSet<string>(c2!)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new List<string>(c) 
            ));
        
        modelBuilder.Entity<Recipe>()
            .ToTable("recipes")
            .Property(e => e.Directions)
            .HasConversion(converter)
            .HasColumnName("directions")
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => new HashSet<string>(c1!).SetEquals(new HashSet<string>(c2!)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new List<string>(c)
            ));
        
        modelBuilder.Entity<Recipe>()
            .ToTable("recipes")
            .Property(e => e.Image)
            .HasColumnName("image"); 
        
        modelBuilder.Entity<Recipe>()
            .ToTable("recipes")
            .Property(e => e.Id)
            .HasColumnName("id"); 

        modelBuilder.Entity<Recipe>()
            .ToTable("recipes")
            .Property(e => e.Title)
            .HasColumnName("title"); 
    }
}