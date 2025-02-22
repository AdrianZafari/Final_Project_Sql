using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.BaseRepo_UnitTests;

public class ProjectRepository_Tests : IDisposable // I wound up retroactively adding this to my other BaseRepo_UnitTests. IAsyncLifetime is also something I encountered but I opted for the default from Microsoft, the overhead is minimal anyway and I like not having to worry about anything the GC might miss between test runs.
{
    private readonly DataContext _context;
    private readonly IProjectRepository _projectRepository;

    public ProjectRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase("TestDB") // I'm using IDisposable anyway, 
            .EnableSensitiveDataLogging() // For testing purposes
            .Options;

        _context = new DataContext(options);
        _projectRepository = new ProjectRepository(_context);
    }

    // Given the high amount of refactoring, I let ChatGPT Arrange the values for many of the Entities, barring particular cases where particular business logic was being tested.

    [Fact]
    public async Task CreateAsync_ShouldReturnProjectEntityFromDB()
    {
        // Arrange
        var entity = new ProjectEntity
        {   
            ProjectLeader_Id = 10,
            Customer_Id = 20,
            Customer_ContactPerson_Id = 30,
            StartDate = DateTime.Now,
            Status = ProjectStatus.Ongoing,
        };

        // Act
        var result = await _projectRepository.CreateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Project_Id);
        Assert.Equal("P-1", result.ProjectNumber); // Sanity Check: Adrian 1 - C# 0; I jumped the gun on the testing being done here, this was the test that gave the most trouble (so far) and was the most important in studying DataContext and base repo was behaving as it should.
    }

    [Fact]
    public async Task GetAsync_ShouldReturnProjectEntity_WhenItExists()
    {
        // Arrange
        var entity = new ProjectEntity
        {
            Project_Id = 2,
            ProjectLeader_Id = 11,
            Customer_Id = 21,
            Customer_ContactPerson_Id = 31,
            StartDate = DateTime.Now,
            Status = ProjectStatus.Ongoing,
            ProjectNumber = "P-2"
        };

        await _projectRepository.CreateAsync(entity);

        // Act
        var result = await _projectRepository.GetAsync(p => p.Project_Id == 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("P-2", result.ProjectNumber);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var project1 = new ProjectEntity
        {
            Project_Id = 3,
            ProjectLeader_Id = 12,
            Customer_Id = 22,
            Customer_ContactPerson_Id = 32,
            StartDate = DateTime.Now,
            ProjectNumber = "P-3"
        };
        var project2 = new ProjectEntity
        {
            Project_Id = 4,
            ProjectLeader_Id = 13,
            Customer_Id = 23,
            Customer_ContactPerson_Id = 33,
            StartDate = DateTime.Now,
            ProjectNumber = "P-4"
        };

        await _projectRepository.CreateAsync(project1);
        await _projectRepository.CreateAsync(project2);

        // Act
        var result = await _projectRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingProject()
    {
        // Arrange
        var entity = new ProjectEntity
        {
            Project_Id = 5,
            ProjectLeader_Id = 14,
            Customer_Id = 24,
            Customer_ContactPerson_Id = 34,
            StartDate = DateTime.Now,
            ProjectNumber = "P-5"
        };
        await _projectRepository.CreateAsync(entity);

        var updatedEntity = new ProjectEntity
        {
            Project_Id = 5,
            ProjectLeader_Id = 15,
            Customer_Id = 25,
            Customer_ContactPerson_Id = 35,
            StartDate = DateTime.Now,
            ProjectNumber = "P-UPDATED"
        };

        // Act
        var result = await _projectRepository.UpdateAsync(p => p.Project_Id == 5, updatedEntity);
        var updatedProject = await _projectRepository.GetAsync(p => p.Project_Id == 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("P-UPDATED", updatedProject.ProjectNumber);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity_WhenItExists()
    {
        // Arrange
        var entity = new ProjectEntity
        {
            Project_Id = 6,
            ProjectLeader_Id = 16,
            Customer_Id = 26,
            Customer_ContactPerson_Id = 36,
            StartDate = DateTime.Now,
            ProjectNumber = "P-6"
        };
        await _projectRepository.CreateAsync(entity);

        // Act
        var result = await _projectRepository.DeleteAsync(p => p.Project_Id == 6);
        var deletedEntity = await _projectRepository.GetAsync(p => p.Project_Id == 6);

        // Assert
        Assert.True(result);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var entity = new ProjectEntity
        {
            Project_Id = 7,
            ProjectLeader_Id = 17,
            Customer_Id = 27,
            Customer_ContactPerson_Id = 37,
            StartDate = DateTime.Now,
            ProjectNumber = "P-7"
        };
        await _projectRepository.CreateAsync(entity);

        // Act
        var result = await _projectRepository.ExistsAsync(p => p.Project_Id == 7);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _projectRepository.ExistsAsync(p => p.Project_Id == 99);

        // Assert
        Assert.False(result);
    }

    #region **Business Logic Tests**

    [Fact]
    public async Task SettingStatusToCompleted_ShouldAutomaticallySetEndDate()
    {
        // Arrange
        var entity = new ProjectEntity
        {
            Project_Id = 8,
            ProjectLeader_Id = 18,
            Customer_Id = 28,
            Customer_ContactPerson_Id = 38,
            StartDate = DateTime.Now,
            Status = ProjectStatus.Ongoing,
            ProjectNumber = "P-8"
        };
        await _projectRepository.CreateAsync(entity);

        var updatedEntity = new ProjectEntity
        {
            Project_Id = 8,
            ProjectLeader_Id = 18,
            Customer_Id = 28,
            Customer_ContactPerson_Id = 38,
            StartDate = entity.StartDate,
            Status = ProjectStatus.Completed,  // Changing status to "Completed"
            ProjectNumber = "P-8"
        };

        // Act
        await _projectRepository.UpdateAsync(p => p.Project_Id == 8, updatedEntity);
        var completedProject = await _projectRepository.GetAsync(p => p.Project_Id == 8);

        // Assert
        Assert.NotNull(completedProject.EndDate);
    }

    [Fact]
    public async Task ProjectNumberAndStartDate_ShouldBeAutomaticallyAssignedCorrectly()
    {
        // Arrange
        var project1 = new ProjectEntity { ProjectLeader_Id = 21, Customer_Id = 31, Customer_ContactPerson_Id = 41 };
        var project2 = new ProjectEntity { ProjectLeader_Id = 22, Customer_Id = 32, Customer_ContactPerson_Id = 42 };

        // Act
        var createdProject1 = await _projectRepository.CreateAsync(project1);
        var createdProject2 = await _projectRepository.CreateAsync(project2);

        // Assert
        Assert.NotNull(createdProject1);
        Assert.NotNull(createdProject2);
        Assert.Equal("P-1", createdProject1.ProjectNumber);
        Assert.Equal("P-2", createdProject2.ProjectNumber);
    }

    #endregion


    #region **Cleanup**
    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Cleanup DB after tests
        _context.Dispose();
    }
    #endregion
}
