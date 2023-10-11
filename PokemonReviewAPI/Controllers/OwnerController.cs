using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;
using PokemonReviewAPI.Repository;

namespace PokemonReviewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owner = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            return Ok(owner);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        public IActionResult GetOwner(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);
        }

        [HttpGet("{pokeId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwnersByPokemon(int pokeId)
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwnersByPokemon(pokeId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owners);
        }

        [HttpGet("{ownerId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonsByOwner(ownerId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateOwner([FromQuery]int countryId, [FromBody] OwnerDto ownerCreate)
        {
            //Check reached data is null nor not
            if(ownerCreate == null)
                return BadRequest(ModelState);

            //Check reached data alredy exists in database or not
            var owner = _ownerRepository.GetOwners().Where(o => o.FirstName.Trim().ToUpper() == ownerCreate.FirstName.Trim().ToUpper() && o.LastName.Trim().ToUpper() == ownerCreate.LastName.Trim().ToUpper()).FirstOrDefault();
            
            if(owner != null)
            {
                ModelState.AddModelError("", "Owner already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Convert ownerDto to owner variable
            var ownerMap = _mapper.Map<Owner>(ownerCreate);

            //Send data to save it and check
            if(!_ownerRepository.CreateOwner(countryId,ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Owner Created Succesfully");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if(updatedOwner == null) 
                return BadRequest(ModelState);

            if(ownerId != updatedOwner.Id)
                return BadRequest(ModelState);

            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            if (!ModelState.IsValid) 
                return BadRequest();

            Owner ownerMap = _mapper.Map<Owner>(updatedOwner);

            if(!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wron while updating!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            //Check that category exists or not:
            if (!_ownerRepository.OwnerExists(ownerId))
                return BadRequest(ModelState);

            //I got category which i will delete
            var deletedOwner = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Finally i send it to function:
            if (!_ownerRepository.DeleteOwner(deletedOwner))
            {
                ModelState.AddModelError("", "Something went wrong while deleting operation!");
                return StatusCode(500, ModelState);
            }

            return Ok($"The {deletedOwner.FirstName} named owner deleted Succesfully!");
        }
    }
}
