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

var builder = WebApplication.CreateBuilder(args);

// ===== Database =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EmpreendedorismoSCDb"));

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

// ===== Middleware =====
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
