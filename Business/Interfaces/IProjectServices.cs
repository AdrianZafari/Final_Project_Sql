using Business.DTOs;
using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IProjectServices
{
    Task<Project> CreateProjectAsync(ProjectRegistrationForm form);
    Task<bool> DeleteProjectAsync(int userId);
    Task<IEnumerable<Project>> GetAllProjects();
    Task<Project> GetProjectByIdAsync(int Id);
    Task<Project> UpdateProjectAsync(int projectId, ProjectRegistrationForm updatedForm);
}