using Business.DTOs;
using Business.Models;

namespace Business.Interfaces;

public interface IServiceServices
{
    Task<Service> CreateServiceAsync(int projectId, ServiceRegistrationForm form);
    Task<bool> DeleteServiceAsync(int serviceId);
    Task<IEnumerable<Service>> GetAllServices();
    Task<List<Service>> GetAllServicesByProjectIdAsync(int ProjectId);
    Task<Service> GetServiceByServiceIdAsync(int serviceId);
    Task<Service> UpdateServiceAsync(int serviceId, ServiceRegistrationForm updatedForm);
}