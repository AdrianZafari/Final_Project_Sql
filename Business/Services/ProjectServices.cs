

using Business.DTOs;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using System.Diagnostics;

namespace Business.Services;

public class ProjectServices(IProjectRepository projectRepository, ICustomerRepository customerRepository, ICustomer_ContactPersonRepository contactPersonRepository, IProjectFactory projectFactory, IEmployeeRepository employeeRepository) : IProjectServices
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICustomer_ContactPersonRepository _contactPersonRepository = contactPersonRepository;
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IProjectFactory _projectFactory = projectFactory;

    public async Task<Project> CreateProjectAsync(ProjectRegistrationForm form) // This thing is a mammoth and references three FKs...
    {
        await _projectRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            // 1. Check if Customer already exists
            var existingCustomer = await _customerRepository.GetAsync(c => c.Customer_Name == form.Customer_Name);

            // 2. Create new Customer if not found
            if (existingCustomer == null)
            {
                var newCustomer = new CustomerEntity
                {
                    Customer_Name = form.Customer_Name
                };

                existingCustomer = await _customerRepository.CreateAsync(newCustomer);
            }

            // 3. Check if Contact Person exists (based on unique Email)
            var existingContactPerson = await _contactPersonRepository.GetAsync(cp => cp.Email == form.Email);

            // 4. Create new Contact Person if not found
            if (existingContactPerson == null)
            {
                var newContactPerson = new Customer_ContactPersonEntity
                {
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    Email = form.Email,
                    Phone = form.Phone,
                    Customer_Id = existingCustomer.Customer_Id // Foreign Key
                };

                existingContactPerson = await _contactPersonRepository.CreateAsync(newContactPerson);
            }

            // Assigns the Id after creating or finding Contact Person
            form.Customer_ContactPerson_Id = existingContactPerson.Customer_ContactPerson_Id;


            // 5. Create Project
            var projectEntity = await _projectFactory.CreateAsync(form);

            projectEntity = await _projectRepository.CreateAsync(projectEntity); // Add to DB

            // 6. Commit Transaction if everything is successful
            await _projectRepository.CommitTransactionAsync();

            var project = _projectFactory.Create(projectEntity);

            return project;
        }
        catch (Exception ex)
        {
            await _projectRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error creating project :: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<Project>> GetAllProjects()
    {

        try
        {
            var entities = await _projectRepository.GetAllAsync();
            var projects = new List<Project>();

            foreach (var entity in entities)
            {
                var project = _projectFactory.Create(entity);
                projects.Add(project);
            }

            return projects;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching employees:: {ex.Message}");
            return null!; ;
        }
    }

    public async Task<Project> GetProjectByIdAsync(int Id)
    {
        try
        {
            var entity = await _projectRepository.GetAsync(e => e.Project_Id == Id);

            if (entity == null)
            {
                Debug.WriteLine($"Project with Id:: {Id} not found.");
                return null!;
            }

            Project project = _projectFactory.Create(entity);
            return project;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching project:: {ex.Message}");
            return null!; ;
        }
    }

    // Limited functionality because this is even bigger than the create and time is not on my side.
    public async Task<Project> UpdateProjectAsync(int projectId, ProjectRegistrationForm updatedForm)
    {
        await _projectRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            // MAKE SURE ProjectId IS VALID
            var targetProjectEntity = await _projectRepository.GetAsync(p => p.Project_Id == projectId);
            if (targetProjectEntity == null)
            {
                Debug.WriteLine($"Project with Id: {projectId} was not found.");
                throw new InvalidOperationException($"Project with Id {projectId} does not exist.");
            }

            // Fetch the current customer
            var targetCustomer = await _customerRepository.GetAsync(c => c.Customer_Id == targetProjectEntity.Customer_Id);
            if (targetCustomer == null)
            {
                Debug.WriteLine($"Customer with Id: {targetProjectEntity.Customer_Id} was not found.");
                throw new InvalidOperationException($"Customer with Id {targetProjectEntity.Customer_Id} does not exist.");
            }

            // Fetch the current contact person
            var targetContactPerson = await _contactPersonRepository.GetAsync(cp => cp.Customer_ContactPerson_Id == targetProjectEntity.Customer_ContactPerson_Id);

            // **Prevent Customer Switching**
            if (!string.IsNullOrWhiteSpace(updatedForm.Customer_Name) && updatedForm.Customer_Name != targetCustomer.Customer_Name)
            {
                Debug.WriteLine($"Cannot change customer. You must create a new project instead.");
                throw new InvalidOperationException("Cannot change customer for an existing project. Please create a new project instead.");
            }

            // **Update Contact Person (Details Allowed, Email Must Stay the Same)**
            if (targetContactPerson != null)
            {
                if (updatedForm.Email != targetContactPerson.Email)
                {
                    Debug.WriteLine($"Cannot change contact person email. You must create a new project instead.");
                    throw new InvalidOperationException("Cannot change contact person email for an existing project. Please create a new project instead.");
                }

                // Update only allowed fields
                targetContactPerson.FirstName = updatedForm.FirstName;
                targetContactPerson.LastName = updatedForm.LastName;
                targetContactPerson.Phone = updatedForm.Phone;

                await _contactPersonRepository.UpdateAsync(cp => cp.Customer_ContactPerson_Id == targetContactPerson.Customer_ContactPerson_Id, targetContactPerson);
            }

            // **Update Project Leader if changed**
            if (updatedForm.ProjectLeader_Id > 0 && updatedForm.ProjectLeader_Id != targetProjectEntity.ProjectLeader_Id)
            {
                var newProjectLeader = await _employeeRepository.GetAsync(e => e.Employee_Id == updatedForm.ProjectLeader_Id);
                if (newProjectLeader != null)
                {
                    targetProjectEntity.ProjectLeader_Id = newProjectLeader.Employee_Id;
                }
                else
                {
                    Debug.WriteLine($"New Project Leader with Id: {updatedForm.ProjectLeader_Id} was not found.");
                    throw new InvalidOperationException($"New Project Leader with Id {updatedForm.ProjectLeader_Id} does not exist.");
                }
            }

            // **Update Project Details**
            targetProjectEntity.Deadline = updatedForm.Deadline;
            targetProjectEntity.Status = updatedForm.Status;

            // Ensure the project entity is tracked and updated
            await _projectRepository.UpdateAsync(p => p.Project_Id == projectId, targetProjectEntity);

            // Commit transaction
            await _projectRepository.CommitTransactionAsync();

            // Return Project instead of Entity
            var targetProject = _projectFactory.Create(targetProjectEntity);

            return targetProject;
        }
        catch (Exception ex)
        {
            await _projectRepository.RollbackTransactionAsync();
            Debug.WriteLine($"Error updating project with Id {projectId}:: {ex.Message}");
            throw;
        }
    }



    public async Task<bool> DeleteProjectAsync(int userId)
    {
        await _projectRepository.BeginTransactionAsync(); // TRANSACTION START

        try
        {
            var targetProject = await _projectRepository.GetAsync(e => e.Project_Id == userId);

            if (targetProject == null)
            {
                Debug.WriteLine($"Project Id: {userId} not found.");
                return false;
            }

            var deleted = await _projectRepository.DeleteAsync(e => e.Project_Id == userId);

            if (!deleted)
            {
                Debug.WriteLine($"Failed to delete Project with Id: {userId}");
                return false;
            }

            await _projectRepository.CommitTransactionAsync(); // TRANSACTION COMMIT

            return true;
        }
        catch (Exception ex)
        {
            await _projectRepository.RollbackTransactionAsync(); // TRANSACTION ABORT
            Debug.WriteLine($"Error deleting Project with Id {userId}:: {ex.Message}");
            return false;
        }
    }




}
