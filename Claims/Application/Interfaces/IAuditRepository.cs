using Claims.Domain.Entities;

namespace Claims.Application.Interfaces
{
    public interface IAuditRepository
    {
        Task AddClaimAuditAsync(ClaimAudit audit);
        Task AddCoverAuditAsync(CoverAudit audit);
    }
}