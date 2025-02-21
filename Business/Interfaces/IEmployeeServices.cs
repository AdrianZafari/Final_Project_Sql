using Business.DTOs;
using Business.Models;

namespace Business.Interfaces;

public interface IEmployeeServices
{
    Task<Employee> CreateEmployeeAsync(EmployeeRegistrationForm form);
    Task<bool> DeleteEmployeeAsync(int userId);
    Task<IEnumerable<Employee>> GetAllEmployees();
    Task<Employee> GetEmployeeByIdAsync(int Id);
    Task<Employee?> UpdateEmployeeAsync(int userId, EmployeeRegistrationForm updatedForm);
}