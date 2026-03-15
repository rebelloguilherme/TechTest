using System.Text.Json.Serialization;
using EmpreendedorismoSC.Application.Interfaces;
using EmpreendedorismoSC.Application.Services;
using EmpreendedorismoSC.Application.Validators;
using EmpreendedorismoSC.Domain.Interfaces;
using EmpreendedorismoSC.Infrastructure.Data;
using EmpreendedorismoSC.Infrastructure.Repositories;
using EmpreendedorismoSC.WebApi.Middleware;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

// ===== Serilog (early init) =====
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação...");

    var builder = WebApplication.CreateBuilder(args);

    // ===== Serilog =====
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

    // ===== Database (SQLite) =====
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));

    // ===== Dependency Injection =====
    builder.Services.AddScoped<IEmpreendimentoRepository, EmpreendimentoRepository>();
    builder.Services.AddScoped<IEmpreendimentoService, EmpreendimentoService>();

    // ===== FluentValidation =====
    builder.Services.AddValidatorsFromAssemblyContaining<CreateEmpreendimentoValidator>();

    // ===== Controllers =====
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // ===== Swagger =====
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Empreendedorismo SC API",
            Version = "v1",
            Description = "API para gerenciamento de empreendimentos em Santa Catarina"
        });

        // Inclui XML docs para exibir descrições no Swagger
        var webApiXml = Path.Combine(AppContext.BaseDirectory, "EmpreendedorismoSC.WebApi.xml");
        if (File.Exists(webApiXml)) options.IncludeXmlComments(webApiXml);

        var applicationXml = Path.Combine(AppContext.BaseDirectory, "EmpreendedorismoSC.Application.xml");
        if (File.Exists(applicationXml)) options.IncludeXmlComments(applicationXml);
    });

    // ===== CORS =====
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // ===== Auto-Migration (não roda em ambiente de teste) =====
    if (!app.Environment.IsEnvironment("Testing"))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Log.Information("Banco de dados migrado com sucesso.");
    }

    // ===== Middleware =====
    app.UseSerilogRequestLogging();
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // ===== Pipeline =====
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Empreendedorismo SC API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Aplicação encerrada com erro.");
}
finally
{
    Log.CloseAndFlush();
}

// Necessário para WebApplicationFactory<Program> nos testes de integração
public partial class Program { }
