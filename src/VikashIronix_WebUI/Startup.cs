using VikashIronix_WebUI.AuthServices;
using VikashIronix_WebUI.Components;
using VikashIronix_WebUI.Services.AuthServices;
using VikashIronix_WebUI.Services.AuthServices.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using SharedKernel.Common.Interfaces;
using SharedKernel.Services;

namespace VikashIronix_WebUI;

public class WebUIStartup { }

public static class DependencyInjection
{
    #region # Add Services

    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Add Razor Components
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers();
        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // Add HTTP Client with JwtTokenHandler
        builder.Services.InjectHttpClient(builder.Configuration);

        // Add Authentication & Authorization
        builder.Services.InjectAuthentication();

        // Add Application Services
        builder.Services.Configure<SharedKernel.Settings.SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
        builder.Services.InjectApplicationServices();
    }

    #endregion

    #region # Use Services

    public static void UseServices(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapStaticAssets();


        app.MapGet("/perform-logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Redirect("/login");
        });

        app.MapControllers();



        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }

    #endregion

    #region # Private Helpers

    private static void InjectHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Register JwtTokenHandler as transient (required for DelegatingHandler)
        services.AddTransient<JwtTokenHandler>();

        // Add HTTP Client Factory with named client and JwtTokenHandler
        services.AddHttpClient("VikashIronixApi", client =>
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        }).AddHttpMessageHandler<JwtTokenHandler>();

    }

    private static void InjectAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "VikashIronix.AuthCookie";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/accessdenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

        // Add Authentication services
        services.AddScoped<TokenValidator>();
    }

    private static void InjectApplicationServices(this IServiceCollection services)
    {
        // Add Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<VikashIronix_WebUI.Services.Inventory.IInventoryService, VikashIronix_WebUI.Services.Inventory.InventoryService>();
        services.AddScoped<VikashIronix_WebUI.Services.Users.IUserService, VikashIronix_WebUI.Services.Users.UserService>();
        services.AddScoped<VikashIronix_WebUI.Services.Customers.ICustomerService, VikashIronix_WebUI.Services.Customers.CustomerService>();
        services.AddScoped<VikashIronix_WebUI.Services.Bills.IBillService, VikashIronix_WebUI.Services.Bills.BillService>();
        services.AddScoped<VikashIronix_WebUI.Services.Holidays.IHolidayService, VikashIronix_WebUI.Services.Holidays.HolidayService>();
        services.AddScoped<VikashIronix_WebUI.Services.Payroll.IPayrollService, VikashIronix_WebUI.Services.Payroll.PayrollService>();
        
        services.AddScoped<IEmailSender, EmailService>();
    }

    #endregion
}

