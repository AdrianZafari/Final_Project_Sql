using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Business_UnitTests.Service_Tests
{
    public class ProjectServicesTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly IProjectRepository _projectRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomer_ContactPersonRepository _contactPersonRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployee_RoleRepository _employeeRoleRepository;
        private readonly IEmployeeServices _employeeServices;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IProjectFactory _projectFactory;
        private readonly IProjectServices _projectServices;

        public ProjectServicesTests()
        {
            // Setup In-Memory DB for testing
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30").EnableSensitiveDataLogging()
                .Options;

            _context = new DataContext(options);
            _context.Database.EnsureCreated();

            // Setup repositories for Act/Assertion checks through getters mainly
            _projectRepository = new ProjectRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _contactPersonRepository = new Customer_ContactPersonRepository(_context);
            _employeeRepository = new EmployeeRepository(_context);
            _employeeRoleRepository = new Employee_RoleRepository(_context);
            _projectFactory = new ProjectFactory(_customerRepository, _contactPersonRepository, _employeeRepository);
            _employeeFactory = new EmployeeFactory(_employeeRoleRepository);
            _employeeServices = new EmployeeServices(_employeeRepository, _employeeRoleRepository, _employeeFactory);

            // Setup services
            _projectServices = new ProjectServices(
                _projectRepository,
                _customerRepository,
                _contactPersonRepository,
                _projectFactory,
                _employeeRepository
            );
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldCreateProjectEntityAndCreateContactPerson_WhenValidData()
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
                ProjectLeader_Id = employeeEntity.Employee_Id
            };

            // Act
            var project = await _projectServices.CreateProjectAsync(projectform);
            var contactPerson = await _contactPersonRepository.GetAsync(cp => cp.Customer_ContactPerson_Id == project.ContactPerson_Id);

            // Assert
            Assert.NotNull(employeeEntity); // Because this is an insanely long process, we have to test along the way.
            Assert.NotNull(project);
            Assert.Equal(employeeEntity.Employee_Id, project.ProjectLeader_Id);
            Assert.Equal("Dominic", contactPerson.FirstName);// Ensures the person was added to the DB, if this is here the other stuff is there too.
        }

        [Fact]
        public async Task GetAllProjectsAsync_ShouldReturnProjects()
        {
            // Arrange //
            var projectsBefore = await _projectServices.GetAllProjects();
            

            // Employee Role
            var employeeRole = new Employee_RoleEntity
            {
                Employee_Role_Name = "Software Developer"
            };

            employeeRole = await _employeeRoleRepository.CreateAsync(employeeRole); // Add to DB

            // Employee
            var employeeForm = new EmployeeRegistrationForm
            {
                FirstName = "Sleep",
                LastName = "Deprived",
                Employee_Email = "sleepless@domain.com",
                Employee_Role_Name = employeeRole.Employee_Role_Name
            };
            var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

            // Project Registration Form
            var projectform = new ProjectRegistrationForm
            {
                FirstName = "Dominic",
                LastName = "Toretto",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity.Employee_Id
            };

            var project = await _projectServices.CreateProjectAsync(projectform);

            // Act
            var projectsAfter = await _projectServices.GetAllProjects();

            // Assert
            Assert.NotNull(projectsAfter);
            Assert.True(projectsAfter.Count() > projectsBefore.Count());
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenProjectExists()
        {
            // Arrange //

            // Employee Role
            var employeeRole = new Employee_RoleEntity
            {
                Employee_Role_Name = "Software Developer"
            };

            employeeRole = await _employeeRoleRepository.CreateAsync(employeeRole); // Add to DB

            // Employee
            var employeeForm = new EmployeeRegistrationForm
            {
                FirstName = "Sleep",
                LastName = "Deprived",
                Employee_Email = "sleepless@domain.com",
                Employee_Role_Name = employeeRole.Employee_Role_Name
            };
            var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB


            // Project Registration Form
            var projectform = new ProjectRegistrationForm
            {
                FirstName = "Dominic",
                LastName = "Toretto",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity.Employee_Id
            };

            var project = await _projectServices.CreateProjectAsync(projectform);

            // Act
            var retrievedProject = await _projectServices.GetProjectByIdAsync(project.Project_Id);

            // Assert
            Assert.NotNull(retrievedProject);
            Assert.Equal(project.Project_Id, retrievedProject.Project_Id);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldNotUpdate_WhenGivenSameData()
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

            // Updated form

            var updatedForm = new ProjectRegistrationForm
            {
                FirstName = "Dominic",
                LastName = "Toretto",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity.Employee_Id,
            };

            // Act
            var updatedProject = await _projectServices.UpdateProjectAsync(project.Project_Id, updatedForm);

            // Assert
            Assert.Equal(project.StartDate, updatedProject.StartDate);
            Assert.Equal(project.ProjectNumber, updatedProject.ProjectNumber);
            Assert.Equal(project.ProjectStatus, updatedProject.ProjectStatus);
            Assert.Equal(project.ContactPerson_Id, updatedProject.ContactPerson_Id);
            Assert.Equal(project.Customer_Id, updatedProject.Customer_Id);
            Assert.Equal(project.Project_Id, updatedProject.Project_Id);

        }

        [Fact]
        public async Task UpdateProjectAsync_ValidData_ShouldUpdateSuccessfully()
        {
            // Arrange //

            // Employee 1
            var employeeForm = new EmployeeRegistrationForm
            {
                FirstName = "Sleep",
                LastName = "Deprived",
                Employee_Email = "sleepless@domain.com",
                Employee_Role_Name = "Software Developer"
            };
            var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB

            // Employee 2
            var employeeForm1 = new EmployeeRegistrationForm
            {
                FirstName = "Very",
                LastName = "Sleep-Deprived",
                Employee_Email = "sleepless@domain.com",
                Employee_Role_Name = "Software Developer"
            };
            var employeeEntity1 = await _employeeServices.CreateEmployeeAsync(employeeForm1); // Creates and adds to DB


            // Project Registration Form
            var projectform = new ProjectRegistrationForm
            {
                FirstName = "Dominic",
                LastName = "Toretto",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity.Employee_Id
            };
            var ContactsBefore = await _contactPersonRepository.GetAllAsync();

            var project = await _projectServices.CreateProjectAsync(projectform); // Adds to DB


            // Updated form

            var updatedForm = new ProjectRegistrationForm
            {
                FirstName = "Dominica",
                LastName = "Torettino",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity1.Employee_Id,
                Status = ProjectStatus.Completed
            };


            // Act
            var updatedProject = await _projectServices.UpdateProjectAsync(project.Project_Id, updatedForm);
            var updatedLeader = await _employeeRepository.GetAsync(e => e.Employee_Id == updatedProject.ProjectLeader_Id);
            var updatedContact = await _contactPersonRepository.GetAsync(cp => cp.Customer_ContactPerson_Id == updatedProject.ContactPerson_Id);

            // Assert
            Assert.NotNull(updatedProject);
            Assert.NotNull(updatedProject.EndDate);
            Assert.Equal(ProjectStatus.Completed, updatedProject.ProjectStatus);
            Assert.Equal("Very",updatedLeader.FirstName);
            Assert.Equal("Sleep-Deprived", updatedLeader.LastName);
            Assert.Equal("Dominica", updatedContact.FirstName);
            Assert.Equal("Torettino", updatedContact.LastName);
        }


        [Fact]
        public async Task DeleteProjectAsync_ShouldRemoveProject_WhenProjectExists()
        {

            // Arrange //

            // Employee Role
            var employeeRole = new Employee_RoleEntity
            {
                Employee_Role_Name = "Software Developer"
            };

            employeeRole = await _employeeRoleRepository.CreateAsync(employeeRole); // Add to DB

            // Employee
            var employeeForm = new EmployeeRegistrationForm
            {
                FirstName = "Sleep",
                LastName = "Deprived",
                Employee_Email = "sleepless@domain.com",
                Employee_Role_Name = employeeRole.Employee_Role_Name
            };
            var employeeEntity = await _employeeServices.CreateEmployeeAsync(employeeForm); // Creates and adds to DB


            // Project Registration Form
            var projectform = new ProjectRegistrationForm
            {
                FirstName = "Dominic",
                LastName = "Toretto",
                Customer_Name = "Something Fast and Quite Furious",
                Email = "dominic@domain.com",
                ProjectLeader_Id = employeeEntity.Employee_Id
            };

            var project = await _projectServices.CreateProjectAsync(projectform); ;

            // Act
            var deleted = await _projectServices.DeleteProjectAsync(project.Project_Id);
            var retrievedProject = await _projectServices.GetProjectByIdAsync(project.Project_Id);

            // Assert
            Assert.True(deleted);
            Assert.Null(retrievedProject);
        }

        #region Cleanup

        public void Dispose()
        {
            // Cleanup test data if necessary
            _context.Database.ExecuteSqlRaw("DELETE FROM Projects");
            _context.Database.ExecuteSqlRaw("DELETE FROM Customers");
            _context.Database.ExecuteSqlRaw("DELETE FROM Customer_Contact_Persons");
            _context.Database.ExecuteSqlRaw("DELETE FROM Employees");

            _context.Dispose();
        }

        #endregion
    }
}
