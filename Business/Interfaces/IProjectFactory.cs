using Business.DTOs;
using Business.Models;
using Data.Entities;

namespace Business.Interfaces
{
    public interface IProjectFactory
    {
        Project Create(ProjectEntity entity);
        Task<ProjectEntity> CreateAsync(ProjectRegistrationForm form);
    }
}