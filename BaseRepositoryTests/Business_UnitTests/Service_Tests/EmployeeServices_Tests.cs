using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Services;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Business_UnitTests.Service_Tests;

public class EmployeeServicesTests : IDisposable
{
    private readonly DataContext _context;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployee_RoleRepository _employeeRoleRepository;
    private readonly IEmployeeFactory _employeeFactory;
    private readonly IEmployeeServices _employeeServices;

    public EmployeeServicesTests()
    {
        // InMemoryDatabase does not have Rollback support. In hindsight, using SqlServer instead with IDisposable to achieve same results and something more practical. 
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30")
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureCreated();

        // Setup repositories for Act/Assertion checks through getters mainly
        _employeeRepository = new EmployeeRepository(_context);
        _employeeRoleRepository = new Employee_RoleRepository(_context);
        _employeeFactory = new EmployeeFactory(_employeeRoleRepository);

        // Setup services
        _employeeServices = new EmployeeServices(_employeeRepository, _employeeRoleRepository, _employeeFactory);

    }

    // Given the high amount of refactoring, I let ChatGPT Arrange the values for many of the Entities, barring the Act and Asserts being made.



    [Fact]
    public async Task CreateEmployeeAsync_ShouldCreateEmployee_WhenValidData()
    {
        // Arrange
        var registrationForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Manager"
        };

        // Act
        var result = await _employeeServices.CreateEmployeeAsync(registrationForm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john.doe@example.com", result.Employee_Email);
        Assert.Equal("Manager", result.Employee_Role_Name);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateEmployee_WhenValidData()
    {
        // Arrange
        var registrationForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Manager"
        };

        var employee = await _employeeServices.CreateEmployeeAsync(registrationForm); // Create an employee first
        var updatedForm = new EmployeeRegistrationForm
        {
            FirstName = "Jane",
            LastName = "Smith",
            Employee_Email = "jane.smith@example.com",
            PhoneNumber = "987654321",
            Employee_Role_Name = "Director"
        };

        // Act
        var updatedEmployee = await _employeeServices.UpdateEmployeeAsync(employee.Employee_Id, updatedForm);

        // Assert
        Assert.NotNull(updatedEmployee);
        Assert.Equal("Jane", updatedEmployee.FirstName);
        Assert.Equal("Smith", updatedEmployee.LastName);
        Assert.Equal("jane.smith@example.com", updatedEmployee.Employee_Email);
        Assert.Equal("Director", updatedEmployee.Employee_Role_Name);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidEmployeeId = 999;
        var updateForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            Employee_Role_Name = "Cool Role"
        };

        // Act
        var result = await _employeeServices.UpdateEmployeeAsync(invalidEmployeeId, updateForm);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        var registrationForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Manager"
        };

        var employee = await _employeeServices.CreateEmployeeAsync(registrationForm); // Create employee

        // Act
        var result = await _employeeServices.GetEmployeeByIdAsync(employee.Employee_Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Employee_Id, result.Employee_Id);
    }

    [Fact]
    public async Task DeleteEmployee_ShouldReturnTrue_WhenEmployeeExists()
    {
        // Arrange
        var registrationForm = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Manager"
        };

        var employee = await _employeeServices.CreateEmployeeAsync(registrationForm); // Create employee

        // Act
        var result = await _employeeServices.DeleteEmployeeAsync(employee.Employee_Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_InvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidEmployeeId = 999; // An ID that does not exist in the database

        // Act
        var result = await _employeeServices.DeleteEmployeeAsync(invalidEmployeeId);

        // Assert
        Assert.False(result); // The method should return false if no employee was found
    }


    [Fact]
    public async Task GetAllEmployees_ShouldReturnListOfEmployees_WhenEmployeesExist()
    {
        // Arrange
        var registrationForm1 = new EmployeeRegistrationForm
        {
            FirstName = "John",
            LastName = "Doe",
            Employee_Email = "john.doe@example.com",
            PhoneNumber = "123456789",
            Employee_Role_Name = "Manager"
        };
        var registrationForm2 = new EmployeeRegistrationForm
        {
            FirstName = "Jane",
            LastName = "Smith",
            Employee_Email = "jane.smith@example.com",
            PhoneNumber = "987654321",
            Employee_Role_Name = "Director"
        };

        await _employeeServices.CreateEmployeeAsync(registrationForm1);
        await _employeeServices.CreateEmployeeAsync(registrationForm2);

        // Act
        var employees = await _employeeServices.GetAllEmployees();

        // Assert
        Assert.NotEmpty(employees);
        Assert.Equal(2, employees.Count());
    }


    #region **Cleanup of DB**
    public void Dispose()
    {
        // Cleanup test data if necessary
        _context.Database.ExecuteSqlRaw("DELETE FROM Employees");
        _context.Database.ExecuteSqlRaw("DELETE FROM Employee_Roles");

        _context.Dispose();
    }
    #endregion

}
