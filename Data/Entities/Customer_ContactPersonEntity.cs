

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Customer_ContactPersonEntity
{
    [Key]
    public int Customer_ContactPerson_Id { get; set; } 

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    public string? Phone { get; set; } 


    // Foreign Key to Customer
    public int Customer_Id { get; set; }
    public CustomerEntity Customer { get; set; } = null!;


    // A ContactPerson must NOT be linked to at least 1 project, this will otherwise cause the ProjectUpdateAsync to break into a state where you can't update a Project with a new Contact Person
    public virtual ICollection<ProjectEntity>? Projects { get; set; }
}

