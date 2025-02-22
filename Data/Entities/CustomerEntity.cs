

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class CustomerEntity
{
    [Key]
    public int Customer_Id { get; set; } // Primary Key

    [Required]
    public string Customer_Name { get; set; } = null!;


    // Navigation property
    public Customer_ContactPersonEntity ContactPerson { get; set; } = null!;


    // Same bullshit as with CustomerContactPerson. Make it nullable and let yourself be able to create a new Customer in UpdateProjectAsync 
    public virtual ICollection<ProjectEntity>? Projects { get; set; }
}

