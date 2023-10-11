using Microsoft.EntityFrameworkCore;
using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context) 
        {
            _context = context; 
        }
        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public ICollection<Pokemon> GetPokemons(int id)
        {
           return _context.PokemonCategories.Where(pc => pc.CategoryId == id).Select(pc => pc.Pokemon).ToList();
        }
        public Category GetCategory(int id) 
        {
            return _context.Categories.Find(id);
        }
        public Category GetCategory(string name)
        {
            return _context.Categories.FirstOrDefault(c => c.Name == name);
        }
        public bool CategoryExists(int? id, string? name) 
        {
            //if user checks for id
            if (id != null)
                return _context.Categories.Any(c => c.Id == id);
            //user checks for name
            else
                return _context.Categories.Any(c => c.Name == name);
        }

        public bool CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            return Save();
        }
    }
}
