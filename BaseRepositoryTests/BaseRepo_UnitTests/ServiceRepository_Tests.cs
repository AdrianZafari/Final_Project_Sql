using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.BaseRepo_UnitTests
{
    public class ServiceRepository_Tests : IDisposable
    {
        private readonly DataContext _context;
        private readonly IServiceRepository _serviceRepository;

        public ServiceRepository_Tests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;

            _context = new DataContext(options);
            _serviceRepository = new ServiceRepository(_context);
        }

        // Given the high amount of refactoring, I let ChatGPT Arrange the values for many of the Entities, barring particular cases where particular business logic was being tested.

        [Fact]
        public async Task CreateAsync_ShouldReturnServiceEntityFromDB()
        {
            // Arrange
            var service = new ServiceEntity
            {
                Service_Id = 1,
                Project_Id = 1,
                Service_Name = "Web Development",
                Service_Description = "Full-stack web development service",
                Service_Price = 5000.00m
            };

            // Act
            var result = await _serviceRepository.CreateAsync(service);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Service_Id);
            Assert.Equal(service.Service_Name, result.Service_Name);
            Assert.Equal(service.Service_Price, result.Service_Price);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnServiceEntity_WhenItExists()
        {
            // Arrange
            var service = new ServiceEntity
            {
                Service_Id = 2,
                Project_Id = 1,
                Service_Name = "SEO Optimization",
                Service_Description = "Improve search engine rankings",
                Service_Price = 1200.00m
            };

            await _serviceRepository.CreateAsync(service);

            // Act
            var result = await _serviceRepository.GetAsync(s => s.Service_Id == 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SEO Optimization", result.Service_Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _serviceRepository.GetAsync(s => s.Service_Id == 99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllServices()
        {
            // Arrange
            var service1 = new ServiceEntity
            {
                Service_Id = 3,
                Project_Id = 2,
                Service_Name = "Graphic Design",
                Service_Description = "UI/UX and branding design",
                Service_Price = 3000.00m
            };

            var service2 = new ServiceEntity
            {
                Service_Id = 4,
                Project_Id = 2,
                Service_Name = "Content Writing",
                Service_Description = "SEO-optimized content writing",
                Service_Price = 800.00m
            };

            await _serviceRepository.CreateAsync(service1);
            await _serviceRepository.CreateAsync(service2);

            // Act
            var result = await _serviceRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingService()
        {
            // Arrange
            var service = new ServiceEntity
            {
                Service_Id = 5,
                Project_Id = 3,
                Service_Name = "Mobile App Development",
                Service_Description = "iOS and Android app creation",
                Service_Price = 7000.00m
            };

            await _serviceRepository.CreateAsync(service);

            var updatedService = new ServiceEntity
            {
                Service_Id = 5,
                Project_Id = 3,
                Service_Name = "Updated Mobile App Development",
                Service_Description = "Updated iOS and Android app services",
                Service_Price = 7500.00m
            };

            // Act
            var result = await _serviceRepository.UpdateAsync(s => s.Service_Id == 5, updatedService);
            var updatedEntity = await _serviceRepository.GetAsync(s => s.Service_Id == 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Mobile App Development", updatedEntity.Service_Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var updatedService = new ServiceEntity
            {
                Service_Id = 99,
                Project_Id = 3,
                Service_Name = "Ghost Service",
                Service_Description = "Should not exist",
                Service_Price = 0.00m
            };

            // Act
            var result = await _serviceRepository.UpdateAsync(s => s.Service_Id == 99, updatedService);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity_WhenItExists()
        {
            // Arrange
            var service = new ServiceEntity
            {
                Service_Id = 6,
                Project_Id = 4,
                Service_Name = "Testing Service",
                Service_Description = "Unit and integration testing",
                Service_Price = 2000.00m
            };

            await _serviceRepository.CreateAsync(service);

            // Act
            var result = await _serviceRepository.DeleteAsync(s => s.Service_Id == 6);
            var deletedEntity = await _serviceRepository.GetAsync(s => s.Service_Id == 6);

            // Assert
            Assert.True(result);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _serviceRepository.DeleteAsync(s => s.Service_Id == 99);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var service = new ServiceEntity
            {
                Service_Id = 7,
                Project_Id = 5,
                Service_Name = "Hosting Service",
                Service_Description = "Web hosting and domain registration",
                Service_Price = 150.00m
            };

            await _serviceRepository.CreateAsync(service);

            // Act
            var result = await _serviceRepository.ExistsAsync(s => s.Service_Id == 7);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _serviceRepository.ExistsAsync(s => s.Service_Id == 99);

            // Assert
            Assert.False(result);
        }

        #region **Cleanup**
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Cleanup DB after tests
            _context.Dispose();
        }
        #endregion
    }
}
