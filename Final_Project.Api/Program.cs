using Data.Contexts;
using Microsoft.EntityFrameworkCore; 

// API setup was done with the help of ChatGPT

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (EF Core) to the DI container
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\EC_Utbildning\\SQL\\Projects\\Final_Project_Sql\\Data\\Databases\\local_database_fp.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True")));

// Add controllers for API functionality
builder.Services.AddControllers();

// Enable CORS if needed (for development purposes)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Use CORS policy if needed
app.UseCors("AllowAll");

// Use routing and map controllers
app.UseRouting();
app.MapControllers();

app.Run();
