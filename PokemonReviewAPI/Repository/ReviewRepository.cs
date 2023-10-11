using PokemonReviewAPI.Data;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Find(reviewId);
        }

        public ICollection<Review> GetReviewsByPokemon(int pokeId)
        {
            return _context.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }
        public bool CreateReview(int reviewerId, int pokemonId, Review review)
        {
            //Review is dependent entity with Reviewer and Pokemon therefore we should get them and give them to it.
            //Firstly i get reviewer
            var reviewer = _context.Reviewers.Find(reviewerId);
            //and give it to review
            review.Reviewer = reviewer;
            //Then i get pokemon
            var pokemon = _context.Pokemons.Find(pokemonId);
            //and give it to review
            review.Pokemon = pokemon;
            //and findally add it
            _context.Reviews.Add(review);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            return Save();
        }
    }
}
