using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;
using PokemonReviewAPI.Repository;

namespace PokemonReviewAPI.Controllers
{
    [Route("api/[controller]")]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepostiory _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepostiory reviewerRepostiory, IMapper mapper)
        {
            _reviewerRepository = reviewerRepostiory;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviews= _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateCategory([FromBody] ReviewerDto reviewerCreate)
        {
            //if reached data is null return bad request
            if (reviewerCreate == null)
                return BadRequest(ModelState);
            //Check that is there data already exists or not
            var reviewer = _reviewerRepository.GetReviewers().Where(r => r.FirstName.Trim().ToUpper() == reviewerCreate.FirstName.Trim().ToUpper() && r.LastName.Trim().ToUpper() == reviewerCreate.LastName.Trim().ToUpper()).FirstOrDefault();

            if (reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //And We cahange our categorydto variable to category variable
            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);
            //And finally we send our data to save it and also we checking it with if
            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Reviewer saved succesfully!");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
        {
            if(updatedReviewer == null)
                return BadRequest(ModelState);

            if(reviewerId != updatedReviewer.Id) 
                return BadRequest(ModelState);

            if(!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            Reviewer reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);

            if(!_reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewerId)
        {
            //Check that category exists or not:
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return BadRequest(ModelState);

            //I got category which i will delete
            var deletedReviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Finally i send it to function:
            if (!_reviewerRepository.DeleteReviewer(deletedReviewer))
            {
                ModelState.AddModelError("", "Something went wrong while deleting operation!");
                return StatusCode(500, ModelState);
            }

            return Ok($"The {deletedReviewer.FirstName} named Reviewer deleted Succesfully!");
        }
    }
}
