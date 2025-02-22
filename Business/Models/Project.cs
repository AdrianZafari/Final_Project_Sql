

using Data.Entities;

namespace Business.Models;

public class Project
{
    public int Project_Id { get; set; }

    public int ProjectLeader_Id { get; set; } 

    public int Customer_Id { get; set; }

    public int ContactPerson_Id { get; set; }


    public string ProjectNumber { get; set; } = null!;


    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? Deadline { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
}
