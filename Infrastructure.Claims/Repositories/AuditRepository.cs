using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Infrastructure.Data;

namespace Claims.Infrastructure.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditContext _context;
        public AuditRepository(AuditContext context) => _context = context;

        public async Task AddClaimAuditAsync(ClaimAudit audit)
        {
            _context.ClaimAudits.Add(audit);
            await _context.SaveChangesAsync();
        }

        public async Task AddCoverAuditAsync(CoverAudit audit)
        {
            _context.CoverAudits.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}