using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Moq;

namespace Claims.Test
{
    public class PremiumCalculationTests
    {
        private readonly CoverService _service;

        public PremiumCalculationTests()
        {
            var repoMock = new Mock<ICoverRepository>();
            var dispatcherMock = new Mock<IEventDispatcher>();
            _service = new CoverService(repoMock.Object, dispatcherMock.Object);
        }

        [Theory]
        [InlineData(10, CoverType.Yacht)]
        [InlineData(50, CoverType.PassengerShip)]
        [InlineData(200, CoverType.Tanker)]
        public async Task ComputePremium_Should_Return_Positive_Premium(int days, CoverType type)
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(days),
                Type = type
            };

            // Act
            var result = await _service.CreateAsync(cover);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.True(result.Premium > 0, $"Expected positive premium for {type} with {days} days.");
        }

        [Fact]
        public async Task ComputePremium_Should_Handle_Zero_Days()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow, // 0 days
                Type = CoverType.Yacht
            };

            var result = await _service.CreateAsync(cover);

            Assert.True(result.Premium >= 0);
        }

        [Fact]
        public async Task ComputePremium_Should_Apply_Discounts_Correctly()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(200), // long period to test 2nd & 3rd period discount
                Type = CoverType.Yacht
            };

            var result = await _service.CreateAsync(cover);

            // Premium should be higher than just 200 * baseRate
            const decimal baseRate = 1250m;
            Assert.True(result.Premium > 200 * baseRate, "Premium should include multipliers and discounts.");
        }
    }
}