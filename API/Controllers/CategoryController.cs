using Application.DTOs.Category;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return category == null 
                    ? NotFound(new { message = "القسم مش موجود" }) 
                    : Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("flat-categories")]
        public async Task<IActionResult> GetLookupCategories([FromQuery] bool? isMain = null)
        {
            try
            {
                var result = await _categoryService.GetFlatCategoriesAsync(isMain);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdCategory = await _categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCategoryDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "رقم القسم غير متطابق" });

                var updated = await _categoryService.UpdateCategoryAsync(dto);
                return updated == false
                    ? NotFound(new { message = "القسم مش موجود" })
                    : Ok(new { message = "تم تحديث القسم بنجاح" }); 
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userType = User.FindFirst("type")?.Value;
                if (userType != "admin") 
                    return Forbid(); 

                var result = await _categoryService.DeleteCategoryAsync(id);
                return result
                    ? Ok(new { message = "تم حذف القسم" })
                    : NotFound(new { message = "القسم مش موجود" });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}