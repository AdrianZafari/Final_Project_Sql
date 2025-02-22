
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class EmployeeRegistrationForm
{
    [Required(ErrorMessage = "First Name cannot be left empty.")]
    [RegularExpression(@"^\s*[\p{L}\-']+\s*$", ErrorMessage = "First Name can only contain letters.")]
    public string FirstName { get; set; } = null!;

    [RegularExpression(@"^\s*[\p{L}\-']+\s*$", ErrorMessage = "Last Name can only contain letters.")]
    [Required(ErrorMessage = "Last Name cannot be left empty.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email cannot be left empty")]
    [RegularExpression(@"^\s*[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\s*$", ErrorMessage = "Please enter a valid email address.")]
    public string Employee_Email { get; set; } = null!;

    [RegularExpression(@"^\s*\+?[0-9]{1,3}?[-.\s]?([0-9]{2,4})?[-.\s]?[0-9]{3,4}[-.\s]?[0-9]{4}\s*$", ErrorMessage = "Please enter a valid phone number.")]
    public string? PhoneNumber { get; set; }

    [Required,RegularExpression(@"^[A-Z][a-z]+(?:\s[A-Z][a-z]+)*$", ErrorMessage = "Please enter a valid role name")]
    public string Employee_Role_Name { get; set; } = null!;
    
}
