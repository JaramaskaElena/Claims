using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoversController : ControllerBase
    {
        private readonly ICoverService _coverService;

        public CoversController(ICoverService coverService)
        {
            _coverService = coverService;
        }

        [HttpGet]
        public async Task<IEnumerable<Cover>> GetAll()
        {
            return await _coverService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cover>> Get(Guid id)
        {
            var cover = await _coverService.GetAsync(id);

            if(cover == null)
            {
                return NotFound();
            }
            return Ok(cover);
        }

        [HttpPost]
        public async Task<ActionResult<Cover>> Create(Cover cover)
        {
            var created = await _coverService.CreateAsync(cover);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _coverService.DeleteAsync(id);
            return NoContent();
        }
    }
}