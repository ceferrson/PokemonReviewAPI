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
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository,IMapper mapper) 
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetCountry(int countryId)
        {
            if(!_countryRepository.CountryExists(countryId))
                return NotFound();
            var country = _mapper.Map<Country>(_countryRepository.GetCountry(countryId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpGet("{countryId}/owners")]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetOwnersByCountry(int countryId)
        {
            var owners = _mapper.Map<List<OwnerDto>>(_countryRepository.GetOwnersByCountry(countryId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            //check that reached data is not null
            if(countryCreate == null)
                return BadRequest(ModelState);
            //check that reached data already exists or not
            var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if(country != null)
            {
                ModelState.AddModelError("", "Country alrady exists!");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            //We send data to save and check it
            if(!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something wen wrong!");
                return StatusCode(500, ModelState);
            }
            return Ok("Country saved succesfully!");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if(updatedCountry == null)
                return BadRequest(ModelState);

            if(countryId!=updatedCountry.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            Country countryMap = _mapper.Map<Country>(updatedCountry);

            if(!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong with updating!");
                return StatusCode(500, ModelState);
            }
            
            return NoContent();

        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            //Check that category exists or not:
            if (!_countryRepository.CountryExists(countryId))
                return BadRequest(ModelState);

            //I got category which i will delete
            var deletedCountry = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Finally i send it to function:
            if (!_countryRepository.DeleteCountry(deletedCountry))
            {
                ModelState.AddModelError("", "Something went wrong while deleting operation!");
                return StatusCode(500, ModelState);
            }

            return Ok($"The {deletedCountry.Name} named country deleted Succesfully!");
        }
    }
}
