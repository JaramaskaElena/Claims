using Claims.Domain.Entities;

namespace Claims.Application.Interfaces
{
    public interface ICoverRepository
    {
        Task<IEnumerable<Cover>> GetCoversAsync();
        Task<Cover> GetCoverAsync(Guid id);
        Task AddAsync(Cover cover);
        Task DeleteAsync(Guid id);
    }
}