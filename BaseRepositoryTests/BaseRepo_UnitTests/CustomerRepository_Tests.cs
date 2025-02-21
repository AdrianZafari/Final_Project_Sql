
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.BaseRepo_UnitTests;

public class CustomerRepository_Tests : IDisposable
{
    private readonly DataContext _context;
    private readonly ICustomerRepository _customerRepository;

    public CustomerRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"{Guid.NewGuid}")
            .Options;

        _context = new DataContext(options);
        _customerRepository = new CustomerRepository(_context);
    }

    // Given the high amount of refactoring, I let ChatGPT Arrange the values for many of the Entities, barring particular cases where particular business logic was being tested.

    [Fact]
    public async Task CreateAsync_ShouldReturnCustomerEntityFromDB()
    {
        // Arrange
        var entity = new CustomerEntity { Customer_Id = 1, Customer_Name = "Company Inc." };

        // Act
        var result = await _customerRepository.CreateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Customer_Id);
        Assert.Equal(entity.Customer_Name, result.Customer_Name);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCustomerEntity_WhenItExists()
    {
        // Arrange
        var entity = new CustomerEntity { Customer_Id = 1, Customer_Name = "Test Customer" };
        await _customerRepository.CreateAsync(entity);

        // Act
        var result = await _customerRepository.GetAsync(c => c.Customer_Id == 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Customer_Name, result.Customer_Name);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _customerRepository.GetAsync(c => c.Customer_Id == 99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        var customer1 = new CustomerEntity { Customer_Id = 1, Customer_Name = "Customer One" };
        var customer2 = new CustomerEntity { Customer_Id = 2, Customer_Name = "Customer Two" };

        await _customerRepository.CreateAsync(customer1);
        await _customerRepository.CreateAsync(customer2);

        // Act
        var result = await _customerRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingCustomer()
    {
        // Arrange
        var entity = new CustomerEntity { Customer_Id = 1, Customer_Name = "Old Name" };
        await _customerRepository.CreateAsync(entity);

        var updatedEntity = new CustomerEntity { Customer_Id = 1, Customer_Name = "Updated Name" };

        // Act
        var result = await _customerRepository.UpdateAsync(c => c.Customer_Id == 1, updatedEntity);
        var updatedCustomer = await _customerRepository.GetAsync(c => c.Customer_Id == 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", updatedCustomer.Customer_Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var updatedEntity = new CustomerEntity { Customer_Id = 99, Customer_Name = "Non-existent" };

        // Act
        var result = await _customerRepository.UpdateAsync(c => c.Customer_Id == 99, updatedEntity);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity_WhenItExists()
    {
        // Arrange
        var entity = new CustomerEntity { Customer_Id = 1, Customer_Name = "To be deleted" };
        await _customerRepository.CreateAsync(entity);

        // Act
        var result = await _customerRepository.DeleteAsync(c => c.Customer_Id == 1);
        var deletedEntity = await _customerRepository.GetAsync(c => c.Customer_Id == 1);

        // Assert
        Assert.True(result);
        
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _customerRepository.DeleteAsync(c => c.Customer_Id == 99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var entity = new CustomerEntity { Customer_Id = 1, Customer_Name = "Existing Customer" };
        await _customerRepository.CreateAsync(entity);

        // Act
        var result = await _customerRepository.ExistsAsync(c => c.Customer_Id == 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _customerRepository.ExistsAsync(c => c.Customer_Id == 99);

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
