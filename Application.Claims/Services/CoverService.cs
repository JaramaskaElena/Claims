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
        private const decimal BaseRate = 1250m;
        private const int FirstPeriodDays = 30;
        private const int SecondPeriodDays = 150;

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
            int totalDays = Math.Max((endDate - startDate).Days, 0);

            decimal multiplier = GetTypeMultiplier(coverType);

            int firstPeriod = Math.Min(totalDays, FirstPeriodDays);
            int secondPeriod = Math.Min(Math.Max(totalDays - FirstPeriodDays, 0), SecondPeriodDays);
            int thirdPeriod = Math.Max(totalDays - FirstPeriodDays - SecondPeriodDays, 0);

            decimal premiumFirst = firstPeriod * BaseRate * multiplier;
            decimal premiumSecond = secondPeriod * BaseRate * multiplier * GetSecondDiscount(coverType);
            decimal premiumThird = thirdPeriod * BaseRate * multiplier * GetThirdDiscount(coverType);

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
        private decimal GetTypeMultiplier(CoverType coverType)
        {
            decimal multiplier;

            switch (coverType)
            {
                case CoverType.Yacht:
                    multiplier = 1.10m;
                    break;

                case CoverType.PassengerShip:
                    multiplier = 1.20m;
                    break;

                case CoverType.Tanker:
                    multiplier = 1.50m;
                    break;

                default:
                    multiplier = 1.30m;
                    break;
            }

            return multiplier;
        }

        private decimal GetSecondDiscount(CoverType coverType)
        {
            return coverType == CoverType.Yacht ? 0.95m : 0.98m;
        }

        private decimal GetThirdDiscount(CoverType coverType)
        {
            return coverType == CoverType.Yacht ? 0.97m : 0.99m;
        }
    }
}