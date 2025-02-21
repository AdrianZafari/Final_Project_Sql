

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class EmployeeEntity
{
    [Key]
    public int Employee_Id { get; set; } // Primary Key

    [Required]
    public int Employee_Role_Id { get; set; } // Foreign Key

    [Required]
    public string Employee_Email { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    // Navigation properties
    public Employee_RoleEntity EmployeeRole { get; set; } = null!;

    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
}

