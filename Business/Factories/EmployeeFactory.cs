
using Business.DTOs;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Factories;

public class EmployeeFactory(IEmployee_RoleRepository employeeRoleRepository): IEmployeeFactory
{
    private readonly IEmployee_RoleRepository _employeeRole = employeeRoleRepository;

    public static EmployeeRegistrationForm Create()
    {
        return new EmployeeRegistrationForm();
    }

    public async Task<EmployeeEntity> CreateAsync(EmployeeRegistrationForm form)
    {
        // Get role from the database
        var existingRole = await _employeeRole.GetAsync(r => r.Employee_Role_Name == form.Employee_Role_Name);

        if (existingRole == null)
        {
            throw new ArgumentException("The specified role does not exist. Please create it first.");
        }

        return new EmployeeEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Employee_Email = form.Employee_Email,
            PhoneNumber = form.PhoneNumber,
            Employee_Role_Id = existingRole.Employee_Role_Id // Assign FK
        };
    }

    public async Task<Employee> CreateAsync(EmployeeEntity entity)
    {
        var role = await _employeeRole.GetAsync(r => r.Employee_Role_Id == entity.Employee_Role_Id);

        if (role == null)
        {
            throw new ArgumentException("The specified role does not exist.");
        }

        return new Employee
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Employee_Email = entity.Employee_Email,
            PhoneNumber = entity.PhoneNumber,
            Employee_Role_Name = role.Employee_Role_Name, 
            Employee_Id = entity.Employee_Id
        };
    }

}
