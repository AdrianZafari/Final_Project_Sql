
using Data.Entities;
using Data.Interfaces;


namespace Data.Repositories;

public class Customer_ContactPersonRepository(DataContext context) : BaseRepository<Customer_ContactPersonEntity>(context), ICustomer_ContactPersonRepository
{
    private readonly DataContext _context = context;   
}
