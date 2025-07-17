using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastrucuture.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<IVehicleServices, VehicleServices>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminServices adminServices) =>
{
    if (adminServices.Login(loginDTO) != null)
    {
        return Results.Ok("Login feito com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{
    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleServices.Post(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();