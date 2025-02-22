using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;


// These tests were refactored using ChatGPT, then edited to be made compatible
namespace Tests.IntegrationTests;

public class ProjectFactoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly IProjectFactory _projectFactory;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomer_ContactPersonRepository _contactPersonRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ProjectFactoryTests()
    {
        // Use SQL Server for Integration Testing
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30")
            .Options;

        _context = new DataContext(options);
        _customerRepository = new CustomerRepository(_context);
        _contactPersonRepository = new Customer_ContactPersonRepository(_context);
        _employeeRepository = new EmployeeRepository(_context);

        // Instantiate ProjectFactory
        _projectFactory = new ProjectFactory(_customerRepository, _contactPersonRepository, _employeeRepository);
    }

   



    #region **Database Cleanup**
    public void Dispose()
    {
        _context.Database.ExecuteSqlRaw("DELETE FROM Projects");
        _context.Database.ExecuteSqlRaw("DELETE FROM Employees");
        _context.Database.ExecuteSqlRaw("DELETE FROM Customers");
        _context.Database.ExecuteSqlRaw("DELETE FROM Customer_Contact_Persons");
        _context.Dispose();
    }
    #endregion
}
