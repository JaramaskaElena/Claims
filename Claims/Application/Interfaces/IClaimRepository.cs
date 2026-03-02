using Claims.Domain.Entities;

namespace Claims.Application.Interfaces
{
    public interface IClaimRepository
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> GetClaimAsync(Guid id);
        Task AddAsync(Claim claim);
        Task DeleteAsync(Guid id);
    }
}