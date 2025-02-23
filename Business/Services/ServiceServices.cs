
using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Data.Repositories;
using System.Diagnostics;

namespace Business.Services;

public class ServiceServices(IServiceRepository serviceRepository) : IServiceServices
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;

    /*
            ----- FRONTEND Handler Flow ------

        var newProject = await CreateProjectAsync(newProjectData);   <--- This should raise all errors
        
        var newService = await CreateServiceAsync(newProject.Project_Id, serviceData);    <--- This assigns the Id to form and adds it to the repository

        All checks are done with Regex, otherwise this should have no conlficts
    */

    public async Task<Service> CreateServiceAsync(int projectId, ServiceRegistrationForm form)
    {
        await _serviceRepository.BeginTransactionAsync(); // TRANSACTION START
        try
        {
            form.Project_Id = projectId;

            // Creates the Entity in the arguemnt and then adds the entity into DB
            var serviceEntity = await _serviceRepository.CreateAsync(ServiceFactory.Create(form));
            await _serviceRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            var service = ServiceFactory.Create(serviceEntity);

            return service;
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Unexpected Error encountered :: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<Service>> GetAllServices()
    {

        try
        {
            var entities = await _serviceRepository.GetAllAsync();
            var services = new List<Service>();

            foreach (var entity in entities)
            {
                var service = ServiceFactory.Create(entity);
                services.Add(service);
            }

            return services;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching services:: {ex.Message}");
            return null!; ;
        }
    }

    public async Task<List<Service>> GetAllServicesByProjectIdAsync(int ProjectId)
    {
        try
        {
            var entities = await _serviceRepository.GetAllAsync(s => s.Project_Id == ProjectId);

            if (entities == null)
            {
                Debug.WriteLine($"Services of Project with Id:: {ProjectId} not found.");
                return null!;
            }

            var services = new List<Service>();

            foreach (var entity in entities)
            {
                services.Add(ServiceFactory.Create(entity));
            }

            return services;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching services:: {ex.Message}");
            return null!; ;
        }
    }

    public async Task<Service> GetServiceByServiceIdAsync(int serviceId)
    {
        try
        {
            var entity = await _serviceRepository.GetAsync(s => s.Service_Id == serviceId);

            if (entity == null)
            {
                Debug.WriteLine($"Service with Id:: {serviceId} not found.");
                return null!;
            }

            var service = ServiceFactory.Create(entity);


            return service;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching service:: {ex.Message}");
            return null!; ;
        }
    }

    public async Task<Service> UpdateServiceAsync(int serviceId, ServiceRegistrationForm updatedForm)
    {
        await _serviceRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            var targetServiceEntity = await _serviceRepository.GetAsync(s => s.Service_Id == serviceId);

            if (targetServiceEntity == null)
            {
                Debug.WriteLine($"Service with Id {serviceId} was not found.");
                return null!;
            }

            // Allows all fields to be edited
            if (!string.IsNullOrWhiteSpace(updatedForm.Service_Name))
                targetServiceEntity.Service_Name = updatedForm.Service_Name;

            if (!string.IsNullOrWhiteSpace(updatedForm.Service_Description))
                targetServiceEntity.Service_Description = updatedForm.Service_Description;

            if (targetServiceEntity.Service_Price != updatedForm.Service_Price)
                targetServiceEntity.Service_Price = updatedForm.Service_Price;

            // Update in DB
            await _serviceRepository.UpdateAsync(s => s.Service_Id == serviceId, targetServiceEntity);
            await _serviceRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            // Convert to Service
            var service = ServiceFactory.Create(targetServiceEntity);

            return service;
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error updating service with Id {serviceId}:: {ex.Message}");
            return null!;
        }

    }

    public async Task<bool> DeleteServiceAsync(int serviceId)
    {
        await _serviceRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            var deleted = await _serviceRepository.DeleteAsync(s => s.Service_Id == serviceId);

            if (!deleted)
            {
                Debug.WriteLine($"Failed to delete Service with Id: {serviceId}");
                return false;
            }

            await _serviceRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            return true;
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error deleteing service with Id {serviceId}:: {ex.Message}");
            return false;
        }

    }


}
