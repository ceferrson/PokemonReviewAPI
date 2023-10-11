using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewAPI.Dto;
using PokemonReviewAPI.Interfaces;
using PokemonReviewAPI.Models;

namespace PokemonReviewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public IActionResult GetCategory(int id)
        {
            if(!_categoryRepository.CategoryExists(id,null))
                return NotFound();
            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(category);
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public IActionResult GetCategory(string name)
        {
            if(!_categoryRepository.CategoryExists(null,name))
                return NotFound();
            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(name));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(category);
        }

        [HttpGet("pokemon/{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons(int id)
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemons(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            //if reached data is null return bad request
            if (categoryCreate == null)
                return BadRequest(ModelState);
            //Check that is there data already exists or not
            var category = _categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //And We cahange our categorydto variable to category variable
            var categoryMap = _mapper.Map<Category>(categoryCreate);
            //And finally we send our data to save it and also we checking it with if
            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return Ok("Category saved succesfully!");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            //Check that reached data is not null
            if (updatedCategory == null)
                return BadRequest(ModelState);

            //Check that categoryId is fit with updatedCategory's Id
            if (categoryId != updatedCategory.Id)
                return BadRequest(ModelState);

            //Check that there is category like this in database
            if (!_categoryRepository.CategoryExists(categoryId, null))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //Convert data from dto to original
            Category categoryMap = _mapper.Map<Category>(updatedCategory);

            //And send data to updating
            if(!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }

            return Ok("Category Updated Succesfully!");//Or You can return NoContent().
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            //Check that category exists or not:
            if (!_categoryRepository.CategoryExists(categoryId, null))
                return BadRequest(ModelState);

            //I got category which i will delete
            var deletedCategory = _categoryRepository.GetCategory(categoryId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //Finally i send it to function:
            if(!_categoryRepository.DeleteCategory(deletedCategory))
            {
                ModelState.AddModelError("", "Something went wrong while deleting operation!");
                return StatusCode(500, ModelState);
            }

            return Ok($"The {deletedCategory.Name} named category deleted Succesfully!");
        }
    }
}
