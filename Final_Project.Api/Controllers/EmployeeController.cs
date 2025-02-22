using Business.DTOs;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Api.Controllers;

public class EmployeeController : Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;

        public EmployeesController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeServices.GetAllEmployees();
            return Ok(employees); // Returns a 200 OK with the list of employees
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeServices.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound(); // 404 if not found
            return Ok(employee); // 200 OK with the employee data
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRegistrationForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // 400 Bad Request if validation fails
            var newEmployee = await _employeeServices.CreateEmployeeAsync(form);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Employee_Id }, newEmployee); // 201 Created
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRegistrationForm form)
        {
            var updatedEmployee = await _employeeServices.UpdateEmployeeAsync(id, form);
            if (updatedEmployee == null) return NotFound(); // 404 if not found
            return Ok(updatedEmployee); // 200 OK with updated employee
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeServices.DeleteEmployeeAsync(id);
            if (!success) return NotFound(); // 404 if not found
            return NoContent(); // 204 No Content
        }
    }
}
