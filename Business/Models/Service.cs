

namespace Business.Models;

public class Service
{
    // Key info (auto assigned)
    public int Service_Id { get; set; } // PK
    public int Project_Id { get; set; } // FK 

    // Service info (editable)
    public string Service_Name { get; set; } = null!;
    public string? Service_Description { get; set; }
    public decimal Service_Price { get; set; }

}
