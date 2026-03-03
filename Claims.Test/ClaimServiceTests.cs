using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Events;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Claims.Test
{
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _claimRepo;
        private readonly Mock<ICoverRepository> _coverRepo;
        private readonly Mock<IEventDispatcher> _dispatcher;

        public ClaimServiceTests()
        {
            _claimRepo = new Mock<IClaimRepository>();
            _coverRepo = new Mock<ICoverRepository>();
            _dispatcher = new Mock<IEventDispatcher>();
        }

        private ClaimService CreateService()
        {
            return new ClaimService(_claimRepo.Object, _coverRepo.Object, _dispatcher.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Save_Claim_And_Dispatch_Event()
        {
            // Arrange
            var cover = new Cover
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>())).ReturnsAsync(cover);

            var claim = new Claim
            {
                CoverId = cover.Id,
                DamageCost = 5000,
                Created = DateTime.UtcNow
            };

            var service = CreateService();

            // Act
            var result = await service.CreateAsync(claim);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.NotEqual(default, result.Created);

            _claimRepo.Verify(r => r.AddAsync(It.IsAny<Claim>()), Times.Once);
            _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<ClaimCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_DamageCost_Too_High()
        {
            // Arrange
            var claim = new Claim
            {
                CoverId = Guid.NewGuid(),
                DamageCost = 200_000,
                Created = DateTime.UtcNow
            };

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(claim));
            Assert.Equal("DamageCost cannot exceed 100,000", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Related_Cover_Does_Not_Exist()
        {
            // Arrange
            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>())).ReturnsAsync((Cover)null);

            var claim = new Claim
            {
                CoverId = Guid.NewGuid(),
                DamageCost = 5000,
                Created = DateTime.UtcNow
            };

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(claim));
            Assert.Equal("Related cover does not exist", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Claim_Out_Of_Cover_Period()
        {
            // Arrange
            var cover = new Cover
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>())).ReturnsAsync(cover);

            var claim = new Claim
            {
                CoverId = cover.Id,
                DamageCost = 5000,
                Created = DateTime.UtcNow // before cover start date
            };

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(claim));
            Assert.Equal("Claim date must be within cover period", ex.Message);
        }
    }
}