using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Events;

namespace Claims.Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepo;
        private readonly ICoverRepository _coverRepo;
        private readonly IEventDispatcher _dispatcher;

        public ClaimService(IClaimRepository claimRepo, ICoverRepository coverRepo, IEventDispatcher dispatcher)
        {
            _claimRepo = claimRepo;
            _coverRepo = coverRepo;
            _dispatcher = dispatcher;
        }

        public async Task<Claim> CreateAsync(Claim claim)
        {
            if (claim.DamageCost > 100_000)
                throw new ArgumentException("DamageCost cannot exceed 100,000");

            // Validate related Cover exists
            var cover = await _coverRepo.GetCoverAsync(claim.CoverId);
            if (cover == null)
                throw new ArgumentException("Related cover does not exist");

            // Validate Claim.Created within Cover period
            var now = DateTime.UtcNow;
            if (now < cover.StartDate || now > cover.EndDate)
                throw new ArgumentException("Claim date must be within cover period");


            claim.Id = Guid.NewGuid();
            claim.Created = DateTime.UtcNow;

            await _claimRepo.AddAsync(claim);
            await _dispatcher.DispatchAsync(new ClaimCreatedEvent(claim));

            return claim;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _claimRepo.DeleteAsync(id);
            await _dispatcher.DispatchAsync(new ClaimDeletedEvent(id));
        }

        public async Task<Claim> GetAsync(Guid id)
        {
           return await _claimRepo.GetClaimAsync(id);
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
           return await _claimRepo.GetClaimsAsync();
        }
    }
}