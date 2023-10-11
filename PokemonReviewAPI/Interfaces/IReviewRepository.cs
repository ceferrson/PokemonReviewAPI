using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);
        ICollection<Review> GetReviewsByPokemon(int pokeId);
        bool ReviewExists(int reviewId);
        bool CreateReview(int reviewerId, int pokemonId, Review review);
        bool UpdateReview(Review review);
        bool DeleteReview(Review review);
        bool Save();
    }
}
