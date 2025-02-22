

using Business.DTOs;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Factories;

public class ProjectFactory(ICustomerRepository customerRepository, ICustomer_ContactPersonRepository contactPersonRepository, IEmployeeRepository employeeRepository) : IProjectFactory
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICustomer_ContactPersonRepository _contactPersonRepository = contactPersonRepository;
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public static ProjectRegistrationForm Create()
    {
        return new ProjectRegistrationForm();
    }

    public async Task<ProjectEntity> CreateAsync(ProjectRegistrationForm form)
    {
        // It's redundant doing these checks as to whether they exist or not, if we made it this far then we should be good
        var existingEmployee = await _employeeRepository.GetAsync(emp => emp.Employee_Id == form.ProjectLeader_Id);
        var existingCustomer = await _customerRepository.GetAsync(c => c.Customer_Name == form.Customer_Name);
        var existingCustomerContact = await _contactPersonRepository.GetAsync(cp => cp.Customer_Id == existingCustomer.Customer_Id);

        // I suppose one little check doesn't hurt...

        if (existingCustomer == null! || existingCustomerContact == null! || existingEmployee == null!)
        {
            throw new ArgumentException("One of the specified entities does not exist. Make sure to have successfully created an Entity, Customer and, Customer Contact Person");
        }

        return new ProjectEntity
        {
            ProjectLeader_Id = form.ProjectLeader_Id,
            Customer_Id = existingCustomer.Customer_Id,
            Customer_ContactPerson_Id = existingCustomerContact.Customer_ContactPerson_Id,
            Deadline = form.Deadline,
            Status = form.Status
        };
    }

    public Project Create(ProjectEntity entity)
    {
        if (entity == null)
        {
            return null!;
        }

        return new Project
        {
            Project_Id = entity.Project_Id,
            ProjectLeader_Id = entity.ProjectLeader_Id,
            Customer_Id = entity.Customer_Id,
            ContactPerson_Id = entity.Customer_ContactPerson_Id,
            ProjectNumber = entity.ProjectNumber!,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Deadline = entity.Deadline,
            ProjectStatus = entity.Status
        };


    }



}
