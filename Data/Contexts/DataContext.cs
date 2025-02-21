using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;


public class DataContext : DbContext
{
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<Customer_ContactPersonEntity> Customer_Contact_Persons { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<Employee_RoleEntity> Employee_Roles { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relationship: One Customer -> One ContactPerson
        modelBuilder.Entity<CustomerEntity>()
            .HasOne(c => c.ContactPerson)
            .WithOne(cp => cp.Customer)
            .HasForeignKey<Customer_ContactPersonEntity>(cp => cp.Customer_Id);

        // Relationship: One Project -> One Customer
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.Customer)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.Customer_Id)
            .OnDelete(DeleteBehavior.Restrict); // Prevent Customer deletion affecting Project

        /* This makes the FK nullable on creation. Why? Because while we want ContactPersons assigned at all times to a Project, we want to be able to create a ContactPerson and then assign its Id, which is only generated upon creation. Without this the entity cannot be added because it is not attached to a Project upon creation. This is not an issue upon Project Creation, however it is an issue if you want to change (and not just update) the contact person upon updating the Project.

        Update: After several hours of beating my head against the wall, this amounted to nothing. It's a flaw in the design and if I had more time I would do something about it. If you want to change the Customer Name or Email of the Contact Person... Too bad! The contact person is a shame. I guess it throws you an Exception now though, which is kind but not very useful.
        */
        


        // Relationship: One Project -> One ContactPerson
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.ContactPerson)
            .WithMany(cp => cp.Projects)
            .HasForeignKey(p => p.Customer_ContactPerson_Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);  
        
        // Relationship: Project Leader is an Employee
        modelBuilder.Entity<ProjectEntity>()
            .HasOne(p => p.ProjectLeader)
            .WithMany(e => e.Projects)
            .HasForeignKey(p => p.ProjectLeader_Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: One Role -> Many Employees
        modelBuilder.Entity<EmployeeEntity>()
            .HasOne(e => e.EmployeeRole)
            .WithMany(r => r.Employees)
            .HasForeignKey(e => e.Employee_Role_Id);

        // Relationship: One Project -> Many Services
        modelBuilder.Entity<ServiceEntity>()
            .HasOne(s => s.Project)
            .WithMany(p => p.Services)
            .HasForeignKey(s => s.Project_Id);


    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Logic for setting StartDate and EndDate
        foreach (var entry in ChangeTracker.Entries<ProjectEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.StartDate = DateTime.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                var statusProperty = entry.Property(p => p.Status);

                // Check if the status was changed to "Completed"
                if (statusProperty.IsModified && statusProperty.CurrentValue == ProjectStatus.Completed)
                {
                    entry.Entity.EndDate = DateTime.Now;
                }
            }
        }
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }



}