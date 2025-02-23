

using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tests.Business_UnitTests.Service_Tests;

public class ServiceServices_Tests : IDisposable
{
    private readonly DataContext _context;
    private readonly IServiceServices _serviceServices;
    private readonly IServiceRepository _serviceRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomer_ContactPersonRepository _contactPersonRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployee_RoleRepository _employeeRoleRepository;
    private readonly IEmployeeServices _employeeServices;
    private readonly IEmployeeFactory _employeeFactory;
    private readonly IProjectFactory _projectFactory;
    private readonly IProjectServices _projectServices;

    public ServiceServices_Tests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30")
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureCreated();

        _projectRepository = new ProjectRepository(_context);
        _customerRepository = new CustomerRepository(_context);
        _contactPersonRepository = new Customer_ContactPersonRepository(_context);
        _employeeRepository = new EmployeeRepository(_context);
        _employeeRoleRepository = new Employee_RoleRepository(_context);
        _projectFactory = new ProjectFactory(_customerRepository, _contactPersonRepository, _employeeRepository);
        _employeeFactory = new EmployeeFactory(_employeeRoleRepository);
        _employeeServices = new EmployeeServices(_employeeRepository, _employeeRoleRepository, _employeeFactory, _projectRepository);
        _serviceRepository = new ServiceRepository(_context);


        _projectServices = new ProjectServices(
                _projectRepository,
                _customerRepository,
                _contactPersonRepository,
                _projectFactory,
                _employeeRepository
            );
        _serviceServices = new ServiceServices(_serviceRepository);
    }

    [Fact]
    public async Task CreateServiceAsync_ShouldCreateService_WhenProjectExists()
    {
        // Arrange //

        // Employee
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Sleep",
            LastName = "Deprived",
            Employee_Email = "sleepless@domain.com",
            Employee_Role_Name = "Software Developer"
        };
        var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

        // Project Registration Form
        var projectform = new ProjectRegistrationForm
        {
            FirstName = "Dominic",
            LastName = "Toretto",
            Customer_Name = "Something Fast and Quite Furious",
            Email = "dominic@domain.com",
            ProjectLeader_Id = employeeEntity.Employee_Id,
        };

        var project = await _projectServices.CreateProjectAsync(projectform); // Adds to DB

        var form = new ServiceRegistrationForm
        {
            Service_Name = "Test Service",
            Service_Description = "This is a test service",
            Service_Price = 100.0m
        };

        // Act // 
        var service = await _serviceServices.CreateServiceAsync(project.Project_Id, form);

        // Assert //
        Assert.NotNull(service);
        Assert.Equal(project.Project_Id, service.Project_Id);
        Assert.Equal("Test Service", service.Service_Name);
    }

    [Fact]
    public async Task CreateServiceAsync_ShouldReturnNull_WhenWrongDataIsGiven()
    {
        // Arrange //

        // Employee
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Sleep",
            LastName = "Deprived",
            Employee_Email = "sleepless@domain.com",
            Employee_Role_Name = "Software Developer"
        };
        var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

        // Project Registration Form
        var projectform = new ProjectRegistrationForm
        {
            FirstName = "Dominic",
            LastName = "Toretto",
            Customer_Name = "Something Fast and Quite Furious",
            Email = "dominic@domain.com",
            ProjectLeader_Id = employeeEntity.Employee_Id,
        };

        var project = await _projectServices.CreateProjectAsync(projectform); // Adds to DB

        var form = new ServiceRegistrationForm
        {
            Service_Name = "Test Service",
            Service_Description = "This is a test service",
            Service_Price = 100.0m
        };

        // Act // 
        var service = await _serviceServices.CreateServiceAsync(project.Project_Id + 1, form);

        // Assert // 
        Assert.Null(service);
    }

    [Fact]
    public async Task UpdateServiceAsync_ShouldUpdateService_WhenCorrectDataGiven()
    {
        // Arrange //

        // Employee
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Sleep",
            LastName = "Deprived",
            Employee_Email = "sleepless@domain.com",
            Employee_Role_Name = "Software Developer"
        };
        var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

        // Project Registration Form
        var projectform = new ProjectRegistrationForm
        {
            FirstName = "Dominic",
            LastName = "Toretto",
            Customer_Name = "Something Fast and Quite Furious",
            Email = "dominic@domain.com",
            ProjectLeader_Id = employeeEntity.Employee_Id,
        };

        var project = await _projectServices.CreateProjectAsync(projectform); // Adds Project to DB
        var projectID = project.Project_Id;

        var form = new ServiceRegistrationForm
        {
            Service_Name = "Test Service 1",
            Service_Description = "This is a test service",
            Service_Price = 100.0m
        };
        var targetService = await _serviceServices.CreateServiceAsync(project.Project_Id, form); // Adds Service to DB

        var updatedform = new ServiceRegistrationForm
        {
            Service_Name = "Test Service 2",
            Service_Description = "This is a test service as well",
            Service_Price = 200.0m
        };

        // Act // 
        var updatedService = await _serviceServices.UpdateServiceAsync(targetService.Service_Id, updatedform);


        // Assert 
        Assert.NotNull(updatedService);
        Assert.Equal(projectID, updatedService.Project_Id);
        Assert.NotEqual(form.Service_Name, updatedService.Service_Name);
        Assert.NotEqual(form.Service_Description, updatedService.Service_Description);
        Assert.NotEqual(form.Service_Price, updatedService.Service_Price);
    }

    [Fact]
    public async Task GetAllServicesByProjectIdAsyncs_ShouldReturnAllServicesOfProject_WhenCorrectIdGiven()
    {
        // Arrange //

        // Employee
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Sleep",
            LastName = "Deprived",
            Employee_Email = "sleepless@domain.com",
            Employee_Role_Name = "Software Developer"
        };
        var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

        // Project Registration Form
        var projectform = new ProjectRegistrationForm
        {
            FirstName = "Dominic",
            LastName = "Toretto",
            Customer_Name = "Something Fast and Quite Furious",
            Email = "dominic@domain.com",
            ProjectLeader_Id = employeeEntity.Employee_Id,
        };

        var project = await _projectServices.CreateProjectAsync(projectform); // Adds Project to DB

        var form = new ServiceRegistrationForm
        {
            Service_Name = "Test Service 1",
            Service_Description = "This is a test service",
            Service_Price = 100.0m
        };
        await _serviceServices.CreateServiceAsync(project.Project_Id, form); // Adds Service to DB

        var form1 = new ServiceRegistrationForm
        {
            Service_Name = "Test Service 2",
            Service_Description = "This is a test service as well",
            Service_Price = 200.0m
        };
        await _serviceServices.CreateServiceAsync(project.Project_Id, form1); // Adds Service to DB

        // Act // 
        var projectServices = await _serviceServices.GetAllServicesByProjectIdAsync(project.Project_Id);


        // Assert // 
        Assert.NotNull(projectServices);
        Assert.Equal(2, projectServices.Count());
        Assert.NotEqual(projectServices[0].Service_Id, projectServices[1].Service_Id);
    }

    [Fact]
    public async Task DeleteServiceAsync_ShouldReturnTrue_WhenWrongRightDataGiven()
    {
        // Arrange //

        // Employee
        var employeeForm = new EmployeeRegistrationForm
        {
            FirstName = "Sleep",
            LastName = "Deprived",
            Employee_Email = "sleepless@domain.com",
            Employee_Role_Name = "Software Developer"
        };
        var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

        // Project Registration Form
        var projectform = new ProjectRegistrationForm
        {
            FirstName = "Dominic",
            LastName = "Toretto",
            Customer_Name = "Something Fast and Quite Furious",
            Email = "dominic@domain.com",
            ProjectLeader_Id = employeeEntity.Employee_Id,
        };

        var project = await _projectServices.CreateProjectAsync(projectform); // Adds to DB

        var form = new ServiceRegistrationForm
        {
            Service_Name = "Test Service",
            Service_Description = "This is a test service",
            Service_Price = 100.0m
        };
        var service = await _serviceServices.CreateServiceAsync(project.Project_Id, form);


        // Act // 
        var deletedService = await _serviceServices.DeleteServiceAsync(service.Service_Id);

        // Assert // 
        Assert.True(deletedService);
    }


    #region **Cleanup of DB**    
    public void Dispose()
    {
        _context.Database.ExecuteSqlRaw("DELETE FROM Services");

        _context.Dispose();
    }
    #endregion

}
