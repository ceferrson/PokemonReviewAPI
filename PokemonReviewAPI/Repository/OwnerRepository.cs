using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;
        public OwnerRepository(DataContext context) 
        {
            _context = context;
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Find(ownerId);
        }

        public ICollection<Owner> GetOwnersByPokemon(int pokeId) 
        {
            return _context.PokemonOwners.Where(po => po.PokemonId == pokeId).Select(po => po.Owner).ToList();
        }

        public ICollection<Pokemon> GetPokemonsByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where( po => po.OwnerId == ownerId).Select(po => po.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId) 
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool CreateOwner(int countryId, Owner owner)
        {
            //we should get country becuase Owner is dependent entity for Country Entity and it has CountryId Foreign Key.
            //So i got it firstly
            var country = _context.Countries.Find(countryId);
            //and give it as country to my owner
            owner.Country = country;
            //And add owner to database
            _context.Owners.Add(owner);

            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Owners.Update(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Owners.Remove(owner);
            return Save();
        }
    }
}
