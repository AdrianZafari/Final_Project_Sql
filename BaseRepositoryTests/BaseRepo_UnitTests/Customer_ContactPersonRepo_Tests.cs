using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.BaseRepo_UnitTests;

public class Customer_ContactPersonRepository_Tests: IDisposable
{
    private readonly DataContext _context;
    private readonly ICustomer_ContactPersonRepository _contactPersonRepository;

    public Customer_ContactPersonRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"{Guid.NewGuid()}")
            .Options;

        _context = new DataContext(options);
        _contactPersonRepository = new Customer_ContactPersonRepository(_context);
    }

    // Given the high amount of refactoring, I let ChatGPT Arrange the values for many of the Entities, barring particular cases where particular business logic was being tested.

    [Fact]
    public async Task CreateAsync_ShouldReturnContactPersonEntityFromDB()
    {
        // Arrange
        var entity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Email = "john.Smith@example.com",
            Phone = "123456789",
            Customer_Id = 1
        };

        // Act
        var result = await _contactPersonRepository.CreateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Customer_ContactPerson_Id);
        Assert.Equal(entity.FirstName, result.FirstName);
        Assert.Equal(entity.Email, result.Email);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnContactPersonEntity_WhenItExists()
    {
        // Arrange
        var entity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.Smith@example.com",
            Customer_Id = 2
        };

        await _contactPersonRepository.CreateAsync(entity);

        // Act
        var result = await _contactPersonRepository.GetAsync(c => c.Customer_ContactPerson_Id == 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _contactPersonRepository.GetAsync(c => c.Customer_ContactPerson_Id == 99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllContactPersons()
    {
        // Arrange
        var contact1 = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Customer_Id = 1
        };
        var contact2 = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 2,
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob@example.com",
            Customer_Id = 2
        };

        await _contactPersonRepository.CreateAsync(contact1);
        await _contactPersonRepository.CreateAsync(contact2);

        // Act
        var result = await _contactPersonRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingContactPerson()
    {
        // Arrange
        var entity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Tom",
            LastName = "Hanks",
            Email = "tom@example.com",
            Customer_Id = 3
        };
        await _contactPersonRepository.CreateAsync(entity);

        var updatedEntity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Updated Tom",
            LastName = "Hanks",
            Email = "tom@example.com",
            Customer_Id = 3
        };

        // Act
        var result = await _contactPersonRepository.UpdateAsync(c => c.Customer_ContactPerson_Id == 1, updatedEntity);
        var updatedContact = await _contactPersonRepository.GetAsync(c => c.Customer_ContactPerson_Id == 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Tom", updatedContact.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var updatedEntity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 99,
            FirstName = "Ghost",
            LastName = "Person",
            Email = "ghost@example.com",
            Customer_Id = 4
        };

        // Act
        var result = await _contactPersonRepository.UpdateAsync(c => c.Customer_ContactPerson_Id == 99, updatedEntity);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity_WhenItExists()
    {
        // Arrange
        var entity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Delete Me",
            LastName = "Now",
            Email = "delete@example.com",
            Customer_Id = 5
        };
        await _contactPersonRepository.CreateAsync(entity);

        // Act
        var result = await _contactPersonRepository.DeleteAsync(c => c.Customer_ContactPerson_Id == 1);
        var deletedEntity = await _contactPersonRepository.GetAsync(c => c.Customer_ContactPerson_Id == 1);

        // Assert
        Assert.True(result);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _contactPersonRepository.DeleteAsync(c => c.Customer_ContactPerson_Id == 99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var entity = new Customer_ContactPersonEntity
        {
            Customer_ContactPerson_Id = 1,
            FirstName = "Existing",
            LastName = "Contact",
            Email = "exists@example.com",
            Customer_Id = 6
        };
        await _contactPersonRepository.CreateAsync(entity);

        // Act
        var result = await _contactPersonRepository.ExistsAsync(c => c.Customer_ContactPerson_Id == 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _contactPersonRepository.ExistsAsync(c => c.Customer_ContactPerson_Id == 99);

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
