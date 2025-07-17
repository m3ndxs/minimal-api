using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
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
#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Admin
app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminServices adminServices) =>
{
    if (adminServices.Login(loginDTO) != null)
    {
        return Results.Ok("Login feito com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Admins");

app.MapGet("/admins", ([FromQuery] int? page, IAdminServices adminServices) =>
{
    var adms = new List<AdminModelView>();
    var admins = adminServices.GetAll(page);

    foreach (var adm in admins)
    {
        adms.Add(new AdminModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(adms);
}).WithTags("Admins");

app.MapGet("/admins/{id}", ([FromRoute] int id, IAdminServices adminServices) =>
{
    var admin = adminServices.GetId(id);

    if (admin == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new AdminModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).WithTags("Admins");

app.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminServices adminServices) =>
{
    var validation = new ErrorValidation
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email) || string.IsNullOrEmpty(adminDTO.Senha) || adminDTO.Perfil == null)
    {
        validation.Messages.Add("O campo não pode ser vazio!");
    }

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var admin = new Admin
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    adminServices.Post(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).WithTags("Admins");

#endregion

#region Validation
ErrorValidation validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ErrorValidation
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name) || string.IsNullOrEmpty(vehicleDTO.Brand))
    {
        validation.Messages.Add("O campo não pode ser vazio!");
    }
    if (vehicleDTO.Year < 1950)
    {
        validation.Messages.Add("Ano deve ser superior a 1950!");
    }

    return validation;
}
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{
    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleServices.Post(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetAll(page);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{ 
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }


    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleServices.Put(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    vehicleServices.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.Run();