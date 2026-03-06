using Claims.Domain.Entities;

namespace Claims.Application.Interfaces
{
    public interface IClaimService
    {
        Task<Claim> CreateAsync(Claim claim);
        Task<bool> DeleteAsync(Guid id);
        Task<Claim> GetAsync(Guid id);
        Task<IEnumerable<Claim>> GetAllAsync();
    }
}