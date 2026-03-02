using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Events;
using Moq;

namespace Claims.Test
{
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _repo = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();
        private readonly Mock<ICoverRepository> _coverRepo = new();

        private ClaimService CreateService()
        {
            return new ClaimService(_repo.Object, _coverRepo.Object, _dispatcher.Object);
        }
        [Fact]
        public async Task CreateAsync_Should_Save_Claim_And_Dispatch_Event()
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(cover);

            var claim = new Claim
            {
                CoverId = Guid.NewGuid(),
                DamageCost = 5000
            };

            var service = CreateService();

            // Act
            var result = await service.CreateAsync(claim);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.NotEqual(default, result.Created);

            _repo.Verify(x => x.AddAsync(It.IsAny<Claim>()), Times.Once);
            _dispatcher.Verify(x =>
                x.DispatchAsync(It.IsAny<ClaimCreatedEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_DamageCost_TooHigh()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(cover);

            var claim = new Claim
            {
                CoverId = Guid.NewGuid(),
                DamageCost = 200_000
            };

            var service = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(claim));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Claim_Out_Of_CoverPeriod()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _coverRepo.Setup(c => c.GetCoverAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(cover);

            var claim = new Claim
            {
                CoverId = Guid.NewGuid(),
                DamageCost = 5000
            };

            var service = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(claim));
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_And_Dispatch_Event()
        {
            // Arrange
            var id = Guid.NewGuid();
            var service = CreateService();

            // Act
            await service.DeleteAsync(id);

            // Assert
            _repo.Verify(x => x.DeleteAsync(id), Times.Once);
            _dispatcher.Verify(x =>
                x.DispatchAsync(It.IsAny<ClaimDeletedEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Claim()
        {
            var id = Guid.NewGuid();
            var claim = new Claim { Id = id };

            _repo.Setup(x => x.GetClaimAsync(id))
                 .ReturnsAsync(claim);

            var service = CreateService();

            var result = await service.GetAsync(id);

            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Claims()
        {
            var claims = new List<Claim>
            {
                new Claim { Id = Guid.NewGuid() },
                new Claim { Id = Guid.NewGuid() }
            };

            _repo.Setup(x => x.GetClaimsAsync())
                 .ReturnsAsync(claims);

            var service = CreateService();

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }
    }
}