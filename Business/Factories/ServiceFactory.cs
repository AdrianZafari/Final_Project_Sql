

using Business.DTOs;
using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class ServiceFactory
{
    
    public static ServiceRegistrationForm Create()
    {
        return new ServiceRegistrationForm();
    }

    public static ServiceEntity Create(ServiceRegistrationForm form)
    {
        return new ServiceEntity
        {
            Project_Id = form.Project_Id, // FK 
            Service_Name = form.Service_Name,
            Service_Description = form.Service_Description,
            Service_Price = form.Service_Price
        };
    }

    public static Service Create(ServiceEntity entity)
    {
        return new Service
        {
            Service_Id    = entity.Service_Id, // PK
            Project_Id    = entity.Project_Id, // FK
            Service_Name  = entity.Service_Name,
            Service_Description = entity.Service_Description,
            Service_Price = entity.Service_Price
        }; 
    }



}
