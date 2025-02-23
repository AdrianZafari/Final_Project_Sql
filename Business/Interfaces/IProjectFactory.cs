using Business.DTOs;
using Business.Models;
using Data.Entities;

namespace Business.Interfaces
{
    public interface IProjectFactory
    {
        Task<Project> CreateAsync(ProjectEntity entity);
        Task<ProjectEntity> CreateAsync(ProjectRegistrationForm form);
    }
}