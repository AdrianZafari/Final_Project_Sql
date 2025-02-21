
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class ServiceRegistrationForm
{

    // To be assigned on creation, frontend handler => CreateProjectAsync -> CreateServiceAsync, so this should never cause conflicts
    public int Project_Id { get; set; } // FK comes from an associated

    // Service info (editable)

    [Required]
    [RegularExpression(@"^[\p{L}0-9\s\-&,\.]{2,100}$", ErrorMessage = "Service name contains invalid characters.")]
    public string Service_Name { get; set; } = null!;
    
    public string? Service_Description { get; set; }
    
    [Required]
    [Range(0, 99999999.99, ErrorMessage = "Price must be a valid positive number.")]
    public decimal Service_Price { get; set; }
    



}
