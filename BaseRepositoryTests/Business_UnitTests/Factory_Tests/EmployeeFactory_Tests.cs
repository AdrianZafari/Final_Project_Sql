using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

// These tests were refactored using ChatGPT based on the prior testing scheme used for EmployeeServices

namespace Tests.IntegrationTests;

public class EmployeeFactoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly IEmployee_RoleRepository _employeeRoleRepository;
    private readonly IEmployeeFactory _employeeFactory;

    public EmployeeFactoryTests()
    {
        // Use SQL Server for Integration Testing
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30")
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureCreated();

        // Setup repositories
        _employeeRoleRepository = new Employee_RoleRepository(_context);
        _employeeFactory = new EmployeeFactory(_employeeRoleRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateEmployeeEntity_WhenRoleExists()
    {
        // Arrange: Create a Role first
        var role = new Employee_RoleEntity
        {
            Employee_Role_Name = "Software Engineer"
        };
        await _employeeRoleRepository.CreateAsync(role);

        var form = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Software Engineer"  // Matches the created role
        };

        // Act
        var employeeEntity = await _employeeFactory.CreateAsync(form);

        // Assert
        Assert.NotNull(employeeEntity);
        Assert.Equal("John", employeeEntity.FirstName);
        Assert.Equal("Doe", employeeEntity.LastName);
        Assert.Equal("john.doe@example.com", employeeEntity.Employee_Email);
        Assert.Equal(role.Employee_Role_Id, employeeEntity.Employee_Role_Id); // FK should match role ID
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenRoleDoesNotExist()
    {
        // Arrange: Employee role does NOT exist in DB
        var form = new EmployeeRegistrationForm
        {
            FirstName = "Jane",
            LastName = "Smith",
            Employee_Email = "jane.smith@example.com",
            PhoneNumber = "987654321",
            Employee_Role_Name = "Nonexistent Role"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeFactory.CreateAsync(form));
        Assert.Equal("The specified role does not exist. Please create it first.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnEmployeeModel_WhenValidEntity()
    {
        // Arrange: Create a Role
        var role = new Employee_RoleEntity
        {
            Employee_Role_Name = "Project Manager"
        };
        await _employeeRoleRepository.CreateAsync(role);

        var employeeEntity = new EmployeeEntity
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Employee_Email = "alice.johnson@example.com",
            PhoneNumber = "555123456",
            Employee_Role_Id = role.Employee_Role_Id
        };

        // Act
        var employeeModel = await _employeeFactory.CreateAsync(employeeEntity);

        // Assert
        Assert.NotNull(employeeModel);
        Assert.Equal("Alice", employeeModel.FirstName);
        Assert.Equal("Johnson", employeeModel.LastName);
        Assert.Equal("alice.johnson@example.com", employeeModel.Employee_Email);
        Assert.Equal("Project Manager", employeeModel.Employee_Role_Name); // Role name should match
    }

    #region **Database Cleanup**
    public void Dispose()
    {
        _context.Database.ExecuteSqlRaw("DELETE FROM Employees");
        _context.Database.ExecuteSqlRaw("DELETE FROM Employee_Roles");
        _context.Dispose();
    }
    #endregion
}
