

using Data.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Business.Models;

public class Project
{
    public int Project_Id { get; set; }

    public int ProjectLeader_Id { get; set; }

    public string Project_Leader_FirstName { get; set; } = null!;
    public string Project_Leader_LastName { get; set; } = null!;

    public int Customer_Id { get; set; }

    public string Customer_Name { get; set; } = null!;

    public int ContactPerson_Id { get; set; }

    public string ContactPerson_FirstName { get; set; } = null!;
    public string ContactPerson_LastName { get; set; } = null!;
    public string ContactPerson_Email { get; set; } = null!;
    public string ContactPerson_Phone { get; set; } = null!;

    public string ProjectNumber { get; set; } = null!;


    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? Deadline { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
}

public enum ProjectStatus
{
    Active,
    Completed,
    Inactive
}