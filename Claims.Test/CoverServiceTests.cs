using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Events;
using Moq;

namespace Claims.Test
{
    public class CoverServiceTests
    {
        private readonly Mock<ICoverRepository> _repo = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private CoverService CreateService()
        {
            return new CoverService(_repo.Object, _dispatcher.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_SetId_ComputePremium_And_Dispatch_Event()
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            var service = CreateService();

            // Act
            var result = await service.CreateAsync(cover);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.True(result.Premium > 0);

            _repo.Verify(r => r.AddAsync(It.IsAny<Cover>()), Times.Once);
            _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<CoverCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_Delete_And_Dispatch_Event()
        {
            var id = Guid.NewGuid();
            var service = CreateService();

            await service.DeleteAsync(id);

            _repo.Verify(r => r.DeleteAsync(id), Times.Once);
            _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<CoverDeletedEvent>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_StartDate_In_Past()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            var service = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(cover));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Period_TooLong()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(366),
                Type = CoverType.Yacht
            };

            var service = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(cover));
        }
    }
}
