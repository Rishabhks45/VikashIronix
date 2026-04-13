using SharedKernel.Common.Interfaces;
using SharedKernel.Services;


namespace WebApi;

public class WebApiStartup { }

public static class DependencyInject
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Add CORS
        builder.Services.InjectCors();

        // Add Global Configurations (Controllers, Auth, Swagger, SharedKernel)
        builder.InjectGlobalConfigurations(typeof(WebApiStartup).Assembly);

        // Add Encryption Services
        builder.Services.InjectEncryptionServices();

        // Add Email Services
        builder.Services.Configure<SharedKernel.Settings.SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
        builder.Services.AddScoped<IEmailSender, EmailService>();

        // Add Feature Services
        builder.Services.AddScoped<WebApi.Features.Inventory.Infrastructure.InventoryRepository>();
        builder.Services.AddScoped<WebApi.Features.Inventory.InventoryService>();
        builder.Services.AddScoped<WebApi.Features.Users.Infrastructure.UsersRepository>();
        builder.Services.AddScoped<WebApi.Features.Users.UsersService>();
        
        builder.Services.AddScoped<WebApi.Features.Customers.Infrastructure.CustomersRepository>();
        builder.Services.AddScoped<WebApi.Features.Customers.CustomersService>();
        
        builder.Services.AddScoped<WebApi.Features.Bills.Infrastructure.BillsRepository>();
        builder.Services.AddScoped<WebApi.Features.Bills.BillsService>();
        builder.Services.AddScoped<WebApi.Features.Bills.Infrastructure.IBillsRepository, WebApi.Features.Bills.Infrastructure.BillsRepository>();

        builder.Services.AddScoped<WebApi.Features.Holidays.Infrastructure.HolidayRepository>();
        builder.Services.AddScoped<WebApi.Features.Holidays.HolidayService>();

        builder.Services.AddScoped<WebApi.Features.Payroll.Infrastructure.PayrollRepository>();
        builder.Services.AddScoped<WebApi.Features.Payroll.PayrollService>();
    }

    public static void UseServices(this WebApplication app)
    {
        app.UseGlobalConfigurations();
    }

    #region # Private Helpers

    private static void InjectCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorApp", policy =>
            {
                policy.WithOrigins(
                    "https://localhost:7182",
                    "https://localhost:7256",
                    "http://localhost:5182"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });
    }

    private static void InjectEncryptionServices(this IServiceCollection services)
    {
        services.AddScoped<EncryptionService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
    }



    #endregion
}
