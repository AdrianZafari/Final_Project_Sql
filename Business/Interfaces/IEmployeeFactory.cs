using Business.DTOs;
using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IEmployeeFactory
{
    Task<Employee> CreateAsync(EmployeeEntity entity);
    Task<EmployeeEntity> CreateAsync(EmployeeRegistrationForm form);
}