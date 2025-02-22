

using Business.DTOs;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Diagnostics;

namespace Business.Services;

public class EmployeeServices(IEmployeeRepository employeeRepository, IEmployee_RoleRepository employeeRoleRepository, IEmployeeFactory employeeFactory) : IEmployeeServices
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IEmployee_RoleRepository _employeeRoleRepository = employeeRoleRepository;
    private readonly IEmployeeFactory _employeeFactory = employeeFactory;

    public async Task<Employee> CreateEmployeeAsync(EmployeeRegistrationForm form)
    {
        await _employeeRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            var existingRole = await _employeeRoleRepository.GetAsync(r => r.Employee_Role_Name == form.Employee_Role_Name);

            // Adds new roles to Database if there is no role of this name there already
            if (existingRole == null)
            {
                var new_role = new Employee_RoleEntity
                {
                    Employee_Role_Name = form.Employee_Role_Name
                };

                existingRole = await _employeeRoleRepository.CreateAsync(new_role); // Populates Employee_Roles table
            }

            var employeeEntity = await _employeeFactory.CreateAsync(form); // Creates an Entity through the factory

            await _employeeRepository.CreateAsync(employeeEntity); // Populates Employees table and generates the relevant PK

            var employee = await _employeeFactory.CreateAsync(employeeEntity); // Generates an Employee model for frontend usage

            await _employeeRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            return employee;
        }
        catch (Exception ex)
        {
            await _employeeRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            // Catch any unexpected errors
            Debug.WriteLine($"Unexpected error:: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {

        try
        {
            var entities = await _employeeRepository.GetAllAsync();
            var employees = new List<Employee>();

            foreach (var entity in entities)
            {
                var employee = await _employeeFactory.CreateAsync(entity);
                employees.Add(employee);
            }

            return employees;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching employees:: {ex.Message}");
            return null!;;
        }
    }

    public async Task<Employee> GetEmployeeByIdAsync(int Id)
    {
        try
        {
            var entity = await _employeeRepository.GetAsync(e => e.Employee_Id == Id);

            if (entity == null)
            {
                Debug.WriteLine($"Employee with Id:: {Id} not found.");
                return null!;
            }

            Employee employee = await _employeeFactory.CreateAsync(entity);
            return employee;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching employee:: {ex.Message}");
            return null!;;
        }
    }

    public async Task<Employee?> UpdateEmployeeAsync(int userId, EmployeeRegistrationForm updatedForm)
    {
        await _employeeRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            // Fetch employee entity
            var targetEmployeeEntity = await _employeeRepository.GetAsync(e => e.Employee_Id == userId);
            if (targetEmployeeEntity == null)
            {
                Debug.WriteLine($"Employee with Id: {userId} was not found.");
                return null;
            }

            // Fetch current role entity
            var targetEmployee_RoleEntity = await _employeeRoleRepository.GetAsync(e => e.Employee_Role_Id == targetEmployeeEntity.Employee_Role_Id);
            if (targetEmployee_RoleEntity == null)
            {
                Debug.WriteLine($"Employee role with Id: {targetEmployeeEntity.Employee_Role_Id} was not found.");
                return null;
            }

            // Update Employee Entity Fields only if provided
            if (!string.IsNullOrWhiteSpace(updatedForm.FirstName))
                targetEmployeeEntity.FirstName = updatedForm.FirstName;

            if (!string.IsNullOrWhiteSpace(updatedForm.LastName))
                targetEmployeeEntity.LastName = updatedForm.LastName;

            if (!string.IsNullOrWhiteSpace(updatedForm.Employee_Email))
                targetEmployeeEntity.Employee_Email = updatedForm.Employee_Email;

            if (!string.IsNullOrWhiteSpace(updatedForm.PhoneNumber))
                targetEmployeeEntity.PhoneNumber = updatedForm.PhoneNumber;

            // Handling Employee Role Entity update
            if (!string.IsNullOrWhiteSpace(updatedForm.Employee_Role_Name) && updatedForm.Employee_Role_Name != targetEmployee_RoleEntity.Employee_Role_Name)
            {
                var existingRole = await _employeeRoleRepository.GetAsync(r => r.Employee_Role_Name == updatedForm.Employee_Role_Name);

                if (existingRole == null) // Create a new role if it doesn't exist
                {

                    existingRole = await _employeeRoleRepository.CreateAsync(new Employee_RoleEntity
                    {
                        Employee_Role_Name = updatedForm.Employee_Role_Name
                    });
                }

                // Assign the new role
                targetEmployeeEntity.Employee_Role_Id = existingRole.Employee_Role_Id;
            }

            // Commit changes to Entity and update DB
            await _employeeRepository.UpdateAsync(e => e.Employee_Id == userId, targetEmployeeEntity);

            await _employeeRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            // Convert back to `Employee` model
            return await _employeeFactory.CreateAsync(targetEmployeeEntity);
        }
        catch (Exception ex)
        {
            await _employeeRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error updating employee with Id {userId}:: {ex.Message}");
            return null!;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int userId)
    {
        await _employeeRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            var targetEmployee = await _employeeRepository.GetAsync(e => e.Employee_Id == userId);

            if (targetEmployee == null)
            {
                Debug.WriteLine($"Employee Id: {userId} not found.");
                return false;
            }

            var deleted = await _employeeRepository.DeleteAsync(e => e.Employee_Id == userId);

            if (!deleted)
            {
                Debug.WriteLine($"Failed to delete Employee with Id: {userId}");
                return false;
            }

            await _employeeRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            return true;
        }
        catch (Exception ex)
        {
            await _employeeRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error deleting employee with Id {userId}:: {ex.Message}");
            return false;
        }
    }



}
