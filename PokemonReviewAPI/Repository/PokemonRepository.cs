using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository
{
	public class PokemonRepository : IPokemonRepository
	{
		private readonly DataContext _context;

		public PokemonRepository(DataContext context) 
		{
			_context = context;
		}
		public ICollection<Pokemon> GetPokemons()
		{
			return _context.Pokemons.OrderByDescending(p => p.Id).ToList();
		}

		public Pokemon GetPokemon(int id)
		{

			return _context.Pokemons.Find(id);
		}

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.FirstOrDefault(p => p.Name == name);
        }

        public decimal GetPokemonRating(int pokeId)
        {
			var reviews = _context.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
			if (reviews.Count <= 0)
				return 0;
            return reviews.Sum(r => r.Rating) / reviews.Count();
        }

        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemons.Any(p => p.Id == pokeId);
        }

		public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
		{
			//We'll get owner and category becuase pokemon has a many to many relationships with them.
			//Firstly i get owner
			var owner = _context.Owners.Find(ownerId);
			//And i create pokemonOwner and give pokemon and owner to it.
			PokemonOwner pokemonOwner = new PokemonOwner()
			{
				Owner = owner,
				Pokemon = pokemon
			};
			//And add it do data base
			_context.PokemonOwners.Add(pokemonOwner);

			//then i get category
			var category = _context.Categories.Find(categoryId);
			//And i create new pokemoncategory and give pokemon and category to it
			PokemonCategory pokemonCategory = new PokemonCategory()
			{
				Pokemon = pokemon,
				Category = category
			};
			//And add it do data base
			_context.PokemonCategories.Add(pokemonCategory);

			//And Finally i add my pokemon
			_context.Pokemons.Add(pokemon);
			return Save();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdatePokemon(Pokemon pokemon)
		{
			_context.Pokemons.Update(pokemon);
			return Save();
		}

		public bool DeletePokemon(Pokemon pokemon)
		{
			_context.Pokemons.Remove(pokemon);
			return Save();
		}
    }
}
