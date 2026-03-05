using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Events;
using System.ComponentModel.DataAnnotations;

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
            var now = DateTime.UtcNow;
            await ValidateAsync(claim, now);

            claim.Id = Guid.NewGuid();
            claim.Created = now;

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
        private async Task ValidateAsync(Claim claim, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(claim.Name))
                throw new InvalidOperationException("Claim Name is required.");

            if (claim.DamageCost > 100_000)
                throw new ValidationException("DamageCost cannot exceed 100,000");

            var cover = await _coverRepo.GetCoverAsync(claim.CoverId);
            if (cover == null)
                throw new ValidationException("Related cover does not exist");

            if (now < cover.StartDate || now > cover.EndDate)
                throw new ValidationException("Claim date must be within cover period");
        }
    }
}