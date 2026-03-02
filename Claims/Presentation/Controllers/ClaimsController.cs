using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAll()
        {
            return await _claimService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<Claim> Get(Guid id)
        {
            return await _claimService.GetAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Claim>> Create(Claim claim)
        {
            var created = await _claimService.CreateAsync(claim);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _claimService.DeleteAsync(id);
            return NoContent();
        }
    }
}