using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext _context;
        public ClaimRepository(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await _context.Claims.ToListAsync();
        }

        public async Task<Claim> GetClaimAsync(Guid id)
        {
            return await _context.Claims.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var claim = await GetClaimAsync(id);
            if (claim != null)
            {
                _context.Claims.Remove(claim);
                await _context.SaveChangesAsync();
            }
        }
    }
}