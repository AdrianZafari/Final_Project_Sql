using Business.Factories;
using Business.Interfaces;
using Business.Services;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace GUI_project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Load Configuration from appsettings.json in wwwroot
            var appSettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "appsettings.json");
            if (File.Exists(appSettingsFilePath))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(appSettingsFilePath, optional: false, reloadOnChange: true)
                    .Build();

                builder.Configuration.AddConfiguration(config);
            }
            else
            {
                throw new FileNotFoundException("appsettings.json not found");
            }

            builder.Services.AddMauiBlazorWebView();

            // Register Repositories
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployee_RoleRepository, Employee_RoleRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomer_ContactPersonRepository, Customer_ContactPersonRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

            // Register Factories
            builder.Services.AddScoped<IEmployeeFactory, EmployeeFactory>();
            builder.Services.AddScoped<IProjectFactory, ProjectFactory>();

            // Register Services
            builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
            builder.Services.AddScoped<IProjectServices, ProjectServices>();
            builder.Services.AddScoped<IServiceServices, ServiceServices>();

            // Register Database Context (Assuming You Have DbContext)
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}