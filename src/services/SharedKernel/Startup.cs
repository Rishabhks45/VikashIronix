using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel.Common.Interfaces;
using SharedKernel.Services;
using SharedKernel.Settings;
using SharedKernel.Utilities.Helpers;
using System.Reflection;

namespace SharedKernel;

public class SharedKernelStartup { }

public static class DependencyInjection
{
    #region # Microservices - Complete API Setup (Recommended)

    public static void InjectGlobalConfigurations(this WebApplicationBuilder builder, Assembly moduleAssembly)
    {
        builder.Services.InjectControllers();
        builder.Services.InjectAuth(builder.Configuration);
        builder.Services.InjectSwagger();
        builder.Services.InjectSharedKernel(moduleAssembly);
        builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("Encryption"));
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettings>>().Value);
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EncryptionSettings>>().Value);
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static void UseGlobalConfigurations(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowBlazorApp");
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<UserContextMiddleware>();

        app.MapControllers();
    }

    #endregion

    #region # Microservices - Individual Components (For Custom Setup)

    public static void InjectControllers(this IServiceCollection services)
    {
        services
            .AddControllers(x => { })
            .ConfigureApiBehaviorOptions(x =>
            {
                x.SuppressModelStateInvalidFilter = true;
                x.SuppressMapClientErrors = true;
            });
    }

    public static void InjectAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<JwtHelper>(sp => new JwtHelper(configuration, sp.GetRequiredService<ILogger<JwtHelper>>()));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                var tempProvider = services.BuildServiceProvider();
                var jwtHelper = tempProvider.GetRequiredService<JwtHelper>();
                tempProvider.Dispose();

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = jwtHelper.GetTokenValidationParameters();

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new DomainUnauthorizedException("Token expired.");
                        }

                        throw new DomainUnauthorizedException("Invalid token.");
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        throw new DomainUnauthorizedException("Unauthorized access.");
                    }
                };
            });

        services.AddAuthorization();
    }

    public static void InjectSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WebApi v1",
                Version = "v1",
                Description = "Vikash Iron Ledger Suite API - Billing, Inventory, and Management"
            });

            x.EnableAnnotations();

            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token. Enter your token in the format: Bearer {your token}",
                Type = SecuritySchemeType.Http
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void InjectSharedKernel(this IServiceCollection services, Assembly moduleAssembly)
    {
        services.InjectRepositories(moduleAssembly);
        services.InjectServices(moduleAssembly);

        services.AddHttpContextAccessor();
        services.AddScoped<UserContextHelper>();
        services.AddSingleton<DbHelper>();
        services.AddScoped<EmailService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<EncryptionHelper>();
    }

    #endregion

    #region # Private Helpers

    private static void InjectRepositories(this IServiceCollection services, Assembly moduleAssembly)
    {
        InjectBySuffix(services, moduleAssembly, "Repository");
    }

    private static void InjectServices(this IServiceCollection services, Assembly moduleAssembly)
    {
        InjectBySuffix(services, moduleAssembly, "Service");
    }

    private static void InjectBySuffix(IServiceCollection services, Assembly moduleAssembly, string suffix)
    {
        var types = moduleAssembly
            .GetTypes()
            .Where(t => t.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) && !t.IsAbstract);

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();
            if (interfaces.Length > 0)
            {
                services.AddScoped(interfaces[0], type);
            }
            else
            {
                services.AddScoped(type);
            }
        }
    }

    #endregion
}

