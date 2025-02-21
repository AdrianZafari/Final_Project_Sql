
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ServiceEntity
{
    [Key]
    public int Service_Id { get; set; } // Primary Key

    [Required]
    public int Project_Id { get; set; } // Foreign Key to Project

    [Required]
    public string Service_Name { get; set; } = null!;

    public string? Service_Description { get; set; } 

    [Required]
    public decimal Service_Price { get; set; }


    // Navigation properties
    public ProjectEntity Project { get; set; } = null!;
}
