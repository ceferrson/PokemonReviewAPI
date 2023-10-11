using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        ICollection<Pokemon> GetPokemons(int id);
        Category GetCategory(int id);
        Category GetCategory(string name);  
        bool CategoryExists(int? id, string? name);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
