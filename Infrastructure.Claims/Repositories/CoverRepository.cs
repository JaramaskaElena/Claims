using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories
{
    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext _context;
        public CoverRepository(ClaimsContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
           return await _context.Covers.ToListAsync();
        }

        public async Task<Cover> GetCoverAsync(Guid id)
        {
           return await _context.Covers.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Cover cover)
        {
            _context.Covers.Add(cover);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cover = await GetCoverAsync(id);
            if (cover != null)
            {
                _context.Covers.Remove(cover);
                await _context.SaveChangesAsync();
            }
        }
    }
}