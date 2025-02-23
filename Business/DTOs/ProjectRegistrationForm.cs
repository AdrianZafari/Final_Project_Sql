using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs;

public class ProjectRegistrationForm
{
    // -------- CUSTOMER ENTITY INFO --------

    [Required(ErrorMessage = "Customer Name cannot be left empty.")]
    [RegularExpression(@"^[\p{L}0-9&.,'’""()\- ]{2,100}$", ErrorMessage = "Please enter a valid company name.")]
    public string Customer_Name { get; set; } = null!;

    // -------- CONTACT PERSON INFO ---------

    [Required(ErrorMessage = "First Name cannot be left empty.")]
    [RegularExpression(@"^\s*[\p{L}\-']+\s*$", ErrorMessage = "First Name can only contain letters.")]
    public string FirstName { get; set; } = null!;
    
    public int Customer_ContactPerson_Id { get; set; }

    [RegularExpression(@"^\s*[\p{L}\-']+\s*$", ErrorMessage = "Last Name can only contain letters.")]
    [Required(ErrorMessage = "Last Name cannot be left empty.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email cannot be left empty")]
    [RegularExpression(@"^\s*[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\s*$", ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = null!;

    [RegularExpression(@"^\s*\+?[0-9]{1,3}?[-.\s]?([0-9]{2,4})?[-.\s]?[0-9]{3,4}[-.\s]?[0-9]{4}\s*$", ErrorMessage = "Please enter a valid phone number.")]
    public string? Phone { get; set; }

    // -------- PROJECT ENTITY INFO ---------

    [Required]
    public int ProjectLeader_Id { get; set; } // Selected from Employee dropdown

    // Date related info

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now; // Default to current local date

    public DateTime? EndDate { get; set; } // Assigned when Status = "Completed"

    public DateTime? Deadline { get; set; }

    [Required]
    [EnumDataType(typeof(ProjectStatus))]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
}
