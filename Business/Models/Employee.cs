
namespace Business.Models;

public class Employee
{
    public int Employee_Id { get; set; }
    public int Employee_Role_Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Employee_Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Employee_Role_Name { get; set; } = null!;
}
