
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    // References to other main tables

    [Key]
    public int Project_Id { get; set; } // Primary Key

    [Required]
    public int ProjectLeader_Id { get; set; } // Foreign Key to EmployeeEntit, Auto assigned referenced

    [Required]
    public int Customer_Id { get; set; } // Auto assigned referenced

    [Required]
    public int Customer_ContactPerson_Id { get; set; } // Auto assigned referenced

    // Date related info

    [Required]
    public DateTime StartDate { get; set; } // Auto assigned

    public DateTime? EndDate { get; set; } // Assigned when Status = "completed"

    public DateTime? Deadline { get; set; } // Nullable

    [Required]
    [EnumDataType(typeof(ProjectStatus))]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active; // Auto assigned with default variable



    [Column(TypeName = "varchar(10)")]
    public string? ProjectNumber { get; set; } // Auto-generated as "P-1", "P-2", etc.

    // Other Foreign Keys
    public CustomerEntity Customer { get; set; } = null!;

    public Customer_ContactPersonEntity ContactPerson { get; set; } = null!;

    public EmployeeEntity ProjectLeader { get; set; } = null!;

    public virtual ICollection<ServiceEntity> Services { get; set; } = new List<ServiceEntity>();
}

public enum ProjectStatus
{
    Active,
    Completed,
    Inactive
}
