using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(int ownerId);
        ICollection<Owner> GetOwnersByPokemon(int pokeId);
        ICollection<Pokemon> GetPokemonsByOwner(int ownerId);
        bool OwnerExists(int ownerId);  
        bool CreateOwner(int countryId, Owner owner);
        bool UpdateOwner(Owner owner);
        bool DeleteOwner(Owner owner);
        bool Save();
    }
}
