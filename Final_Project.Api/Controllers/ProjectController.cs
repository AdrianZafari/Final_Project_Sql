using Microsoft.AspNetCore.Mvc;
using Business.Interfaces; // Assuming your services are in the Business layer
using Data.Entities; // Assuming your entities (e.g., Project) are in the Data layer
using Business.DTOs;

namespace MyProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectServices _projectServices;

        public ProjectsController(IProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectServices.GetAllProjects();
            return Ok(projects); // Returns 200 OK with the list of projects
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectServices.GetProjectByIdAsync(id);
            if (project == null) return NotFound(); // Returns 404 if the project is not found
            return Ok(project); // Returns 200 OK with the project data
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRegistrationForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // Returns 400 if the request is invalid
            var newProject = await _projectServices.CreateProjectAsync(form);
            return CreatedAtAction(nameof(GetProjectById), new { id = newProject.Project_Id }, newProject); // 201 Created
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectRegistrationForm form)
        {
            var updatedProject = await _projectServices.UpdateProjectAsync(id, form);
            if (updatedProject == null) return NotFound(); // Returns 404 if project is not found
            return Ok(updatedProject); // Returns 200 OK with updated project
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var success = await _projectServices.DeleteProjectAsync(id);
            if (!success) return NotFound(); // Returns 404 if the project was not found
            return NoContent(); // Returns 204 No Content after successful deletion
        }
    }
}
