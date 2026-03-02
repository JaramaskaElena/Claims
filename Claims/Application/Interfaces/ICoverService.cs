using Claims.Domain.Entities;

namespace Claims.Application.Interfaces
{
    public interface ICoverService
    {
        Task<Cover> CreateAsync(Cover cover);
        Task DeleteAsync(Guid id);
        Task<Cover> GetAsync(Guid id);
        Task<IEnumerable<Cover>> GetAllAsync();
    }
}