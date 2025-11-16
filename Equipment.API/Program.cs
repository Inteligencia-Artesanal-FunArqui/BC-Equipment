using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OsitoPolar.EquipmentService.Domain.Repositories;
using OsitoPolar.EquipmentService.Domain.Services;
using OsitoPolar.EquipmentService.Application.Internal.CommandServices;
using OsitoPolar.EquipmentService.Application.Internal.QueryServices;
using OsitoPolar.EquipmentService.Infrastructure.Persistence.EFC.Configuration;
using OsitoPolar.EquipmentService.Infrastructure.Persistence.EFC.Repositories;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Interfaces.ASP.Configuration;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EquipmentDbContext>(options =>
{
    if (connectionString != null)
    {
        options.UseMySQL(connectionString)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
});

// ⚠️ CRÍTICO: Register DbContext as base class for UnitOfWork and BaseRepository
// Sin esto, obtendrás error: "Unable to resolve service for type 'Microsoft.EntityFrameworkCore.DbContext'"
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<EquipmentDbContext>());

// Dependency Injection - Repositories
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<OsitoPolar.EquipmentService.Shared.Domain.Repositories.IUnitOfWork, OsitoPolar.EquipmentService.Shared.Infrastructure.Persistence.EFC.Repositories.UnitOfWork>();

// Dependency Injection - Services
builder.Services.AddScoped<IEquipmentCommandService, EquipmentCommandService>();
builder.Services.AddScoped<IEquipmentQueryService, EquipmentQueryService>();

// ✅ FASE 2: HTTP Facades for Microservices Communication

// 1. EquipmentContextFacade - For OTHER services to call Equipment Service
builder.Services.AddScoped<OsitoPolar.EquipmentService.Interfaces.ACL.IEquipmentContextFacade, OsitoPolar.EquipmentService.Application.ACL.EquipmentContextFacade>();

// 2. ProfilesHttpFacade - For Equipment Service to call Profiles Service
builder.Services.AddHttpClient<OsitoPolar.EquipmentService.Shared.Interfaces.ACL.IProfilesContextFacade, OsitoPolar.EquipmentService.Application.ACL.Services.ProfilesHttpFacade>(client =>
{
    var profilesUrl = builder.Configuration["ServiceUrls:ProfilesService"]
        ?? throw new InvalidOperationException("ProfilesService URL not configured");

    client.BaseAddress = new Uri(profilesUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Equipment-Service/1.0");
});

// 3. NotificationsHttpFacade - For Equipment Service to call Notifications Service
builder.Services.AddHttpClient<OsitoPolar.EquipmentService.Shared.Interfaces.ACL.INotificationContextFacade, OsitoPolar.EquipmentService.Application.ACL.Services.NotificationsHttpFacade>(client =>
{
    var notificationsUrl = builder.Configuration["ServiceUrls:NotificationsService"]
        ?? throw new InvalidOperationException("NotificationsService URL not configured");

    client.BaseAddress = new Uri(notificationsUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Equipment-Service/1.0");
});

// ===========================
// MassTransit + RabbitMQ Configuration
// ===========================
builder.Services.AddMassTransit(x =>
{
    // Configure RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqPort = builder.Configuration["RabbitMQ:Port"] ?? "5672";
        var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host($"rabbitmq://{rabbitMqHost}:{rabbitMqPort}", h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        // Configure message retry policy
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

        // Auto-configure all consumers
        cfg.ConfigureEndpoints(context);
    });
});

Console.WriteLine("✅ MassTransit + RabbitMQ configured for Equipment Service");

// Controllers
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OsitoPolar Equipment Service API",
        Version = "v1",
        Description = "Equipment Microservice - Equipment Management & Rental"
    });
    options.EnableAnnotations();
});

var app = builder.Build();

// Verify database connection on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<EquipmentDbContext>();
    try
    {
        context.Database.CanConnect();
        Console.WriteLine("✅ Database connection successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database connection failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Equipment Service running on port 5003");

app.Run();
