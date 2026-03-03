using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Events;
using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Services
{
    public class CoverService : ICoverService
    {
        private readonly ICoverRepository _coverRepo;
        private readonly IEventDispatcher _dispatcher;

        public CoverService(ICoverRepository coverRepo, IEventDispatcher dispatcher)
        {
            _coverRepo = coverRepo;
            _dispatcher = dispatcher;
        }

        public async Task<Cover> CreateAsync(Cover cover)
        {
            Validate(cover);
            cover.Id = Guid.NewGuid();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            await _coverRepo.AddAsync(cover);
            await _dispatcher.DispatchAsync(new CoverCreatedEvent(cover));

            return cover;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _coverRepo.DeleteAsync(id);
            await _dispatcher.DispatchAsync(new CoverDeletedEvent(id));
        }

        public async Task<Cover> GetAsync(Guid id)
        {
           return await _coverRepo.GetCoverAsync(id);
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
           return await _coverRepo.GetCoversAsync();
        }

        private decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            const decimal baseRate = 1250m;

            decimal typeMultiplier;
            switch (coverType)
            {
                case CoverType.Yacht:
                    typeMultiplier = 1.10m;
                    break;
                case CoverType.PassengerShip:
                    typeMultiplier = 1.20m;
                    break;
                case CoverType.Tanker:
                    typeMultiplier = 1.50m;
                    break;
                default:
                    typeMultiplier = 1.30m;
                    break;
            }

            // Calculate total days
            int totalDays = (int)(endDate - startDate).TotalDays;
            totalDays = Math.Max(totalDays, 0);

            // First period: up to 30 days
            int firstPeriod = Math.Min(totalDays, 30);
            decimal premiumFirst = firstPeriod * baseRate * typeMultiplier;

            // Second period: days 31 to 180 (max 150 days)
            int secondPeriod = Math.Min(Math.Max(totalDays - 30, 0), 150);
            decimal secondDiscount;
            if (coverType == CoverType.Yacht)
            {
                secondDiscount = 0.95m;
            }
            else
            {
                secondDiscount = 0.98m;
            }
            decimal premiumSecond = secondPeriod * baseRate * typeMultiplier * secondDiscount;

            // Third period: days after 180
            int thirdPeriod = Math.Max(totalDays - 180, 0);
            decimal thirdDiscount;
            if (coverType == CoverType.Yacht)
            {
                thirdDiscount = 0.97m;
            }
            else
            {
                thirdDiscount = 0.99m;
            }
            decimal premiumThird = thirdPeriod * baseRate * typeMultiplier * thirdDiscount;

            // Total premium
            return premiumFirst + premiumSecond + premiumThird;
        }
        public void Validate(Cover cover)
        {
            // StartDate cannot be in the past
            if (cover.StartDate < DateTime.UtcNow)
                throw new ValidationException("Cover StartDate cannot be in the past");

            // Total insurance period cannot exceed 1 year
            if ((cover.EndDate - cover.StartDate).TotalDays > 365)
                throw new ValidationException("Total insurance period cannot exceed 1 year");
        }
    }
}