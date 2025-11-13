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

// ⚠️ IMPORTANTE: Facades para comunicación con otros microservicios
// Estos facades ahora harán llamadas HTTP a otros servicios
// Por ahora están comentados - se implementarán después cuando se configure HTTP communication

// builder.Services.AddScoped<IProfilesContextFacade, ProfilesHttpFacade>();

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
