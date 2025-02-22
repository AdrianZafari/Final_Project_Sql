using Data.Entities;
using Data.Interfaces;


namespace Data.Repositories;

public class Employee_RoleRepository(DataContext context) : BaseRepository<Employee_RoleEntity>(context), IEmployee_RoleRepository
{
    private readonly DataContext _context = context;
}