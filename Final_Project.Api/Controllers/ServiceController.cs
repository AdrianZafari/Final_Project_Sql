using Microsoft.AspNetCore.Mvc;
using Business.Interfaces; // Assuming your services are in the Business layer
using Data.Entities; // Assuming your entities (e.g., Service) are in the Data layer
using Business.DTOs;

namespace MyProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceServices _serviceServices;

        public ServicesController(IServiceServices serviceServices)
        {
            _serviceServices = serviceServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceServices.GetAllServices();
            return Ok(services); // Returns 200 OK with the list of services
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetAllServicesByProjectId(int projectId)
        {
            var services = await _serviceServices.GetAllServicesByProjectIdAsync(projectId);
            return Ok(services); // Returns 200 OK with the list of services for the specific project
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceServices.GetAllServicesByProjectIdAsync(id);
            if (service == null) return NotFound(); // Returns 404 if the service is not found
            return Ok(service); // Returns 200 OK with the service data
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceRegistrationForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // Returns 400 if the request is invalid
            var newService = await _serviceServices.CreateServiceAsync(form.Project_Id, form);
            return CreatedAtAction(nameof(GetServiceById), new { id = newService.Service_Id }, newService); // 201 Created
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceRegistrationForm form)
        {
            var updatedService = await _serviceServices.UpdateServiceAsync(id, form);
            if (updatedService == null) return NotFound(); // Returns 404 if the service is not found
            return Ok(updatedService); // Returns 200 OK with updated service
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var success = await _serviceServices.DeleteServiceAsync(id);
            if (!success) return NotFound(); // Returns 404 if the service was not found
            return NoContent(); // Returns 204 No Content after successful deletion
        }
    }
}
