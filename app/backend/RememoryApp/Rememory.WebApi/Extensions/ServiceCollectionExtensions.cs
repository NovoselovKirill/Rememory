using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rememory.Persistence;
using Rememory.Persistence.Client;
using Rememory.Persistence.Repositories.JourneyRepository;
using Rememory.Persistence.Repositories.NoteRepository;
using Rememory.Persistence.Repositories.RefreshSessionRepository;
using Rememory.Persistence.Repositories.UserRepository;
using Rememory.WebApi.Dtos;

namespace Rememory.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
    
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
    
            options.MapType<DeviceIdDto>(() => new OpenApiSchema
            {
                Type = "string",
                MinLength = 32, 
                MaxLength = 100,
                Example = new OpenApiString("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")
            });
        });
        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<DatabaseConfig>(builder.Configuration.GetSection("DatabaseConfig"));

        services.AddSingleton<IDatabaseClient, DatabaseClient>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IJourneyRepository, JourneyRepository>();
        services.AddSingleton<INoteRepository, NoteRepository>();
        services.AddSingleton<IRefreshSessionRepository, RefreshSessionRepository>();

        return services;
    }
}