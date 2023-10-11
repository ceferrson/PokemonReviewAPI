using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;
using PokemonReviewAPI.Repository;

namespace PokemonReviewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int id)
        {
            if (!_pokemonRepository.PokemonExists(id))
                return NotFound();
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemon);
        }


        [HttpGet("{id}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int id)
        {
            if (!_pokemonRepository.PokemonExists(id))
                return NotFound();
            var rating = _pokemonRepository.GetPokemonRating(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(rating);
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(string name)
        {
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(name));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int cateogryId, [FromBody] PokemonDto pokemonCreate)
        {
            if(pokemonCreate == null)
                return BadRequest(ModelState);

            //Checks pokemon doesn't exists in database already
            var pokemon = _pokemonRepository.GetPokemons().Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if(pokemon != null)
            {
                ModelState.AddModelError("", "Pokemon Already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            //And send data to create it and chekcs
            if(!_pokemonRepository.CreatePokemon(ownerId, cateogryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon created succuesfully!");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        public IActionResult UpdatePokemon(int pokeId, [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            Pokemon pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(pokemonMap))
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
        public IActionResult DeletePokemon(int pokeId)
        {
            //Check that category exists or not:
            if (!_pokemonRepository.PokemonExists(pokeId))
                return BadRequest(ModelState);

            //I got category which i will delete
            var deletedPokemon = _pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Finally i send it to function:
            if (!_pokemonRepository.DeletePokemon(deletedPokemon))
            {
                ModelState.AddModelError("", "Something went wrong while deleting operation!");
                return StatusCode(500, ModelState);
            }

            return Ok($"The {deletedPokemon.Name} named Pokemon deleted Succesfully!");
        }
    }
}
