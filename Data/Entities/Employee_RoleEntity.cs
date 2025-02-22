
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Employee_RoleEntity
{
    [Key]
    public int Employee_Role_Id { get; set; }

    [Required]
    public string Employee_Role_Name { get; set; } = null!;

    // Navigation property
    public virtual ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
}
