using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Events;
using Moq;
using System.ComponentModel.DataAnnotations;

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
            var future = DateTime.UtcNow.AddMinutes(1);

            var cover = new Cover
            {
                StartDate = future, // future date
                EndDate = future.AddDays(10),
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
        public async Task DeleteAsync_Should_Delete_Cover_And_ReturnTrue_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Setup repository за да враќа постоечки cover
            _repo.Setup(r => r.GetCoverAsync(id))
                 .ReturnsAsync(new Cover { Id = id });

            var service = CreateService();

            // Act
            var result = await service.DeleteAsync(id);

            // Assert
            Assert.True(result);

            _repo.Verify(r => r.DeleteAsync(id), Times.Once);
            _dispatcher.Verify(d => d.DispatchAsync(It.Is<CoverDeletedEvent>(e => e.CoverId == id)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_StartDate_In_Past()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-1), // past
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            var service = CreateService();

            var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(cover));
            Assert.Equal("Cover StartDate cannot be in the past", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Period_TooLong()
        {
            var future = DateTime.UtcNow.AddMinutes(1);

            var cover = new Cover
            {
                StartDate = future, // future date
                EndDate = future.AddDays(366), // > 1 year
                Type = CoverType.Yacht
            };

            var service = CreateService();

            var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(cover));
            Assert.Equal("Total insurance period cannot exceed 1 year", ex.Message);
        }
        [Fact]
        public async Task GetAsync_Should_Return_Cover()
        {
            // Arrange
            var id = Guid.NewGuid();

            var cover = new Cover
            {
                Id = id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            _repo.Setup(r => r.GetCoverAsync(id))
                 .ReturnsAsync(cover);

            var service = CreateService();

            // Act
            var result = await service.GetAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            _repo.Verify(r => r.GetCoverAsync(id), Times.Once);
        }
        [Fact]
        public async Task GetAllAsync_Should_Return_All_Covers()
        {
            // Arrange
            var covers = new List<Cover>
    {
        new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            Type = CoverType.Yacht
        },
        new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(20),
            Type = CoverType.Tanker
        }
    };

            _repo.Setup(r => r.GetCoversAsync())
                 .ReturnsAsync(covers);

            var service = CreateService();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _repo.Verify(r => r.GetCoversAsync(), Times.Once);
        }
    }
}