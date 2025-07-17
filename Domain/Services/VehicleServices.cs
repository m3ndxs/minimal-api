using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastrucuture.Db;

namespace MinimalApi.Domain.Services;

public class VehicleServices : IVehicleServices
{
    private readonly ApplicationDbContext _dbContext;

    public VehicleServices(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Vehicle> GetAll(int? page = 1, string? name = null, string? brand = null)
    {
        var query = _dbContext.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name.ToLower()}%"));
        }

        int itemsPerPage = 10;

        if (page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return query.ToList();
    }

    public Vehicle? GetId(int id)
    {
        return _dbContext.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Post(Vehicle vehicle)
    {
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.SaveChanges();
    }

    public void Put(Vehicle vehicle)
    {
        _dbContext.Vehicles.Update(vehicle);
        _dbContext.SaveChanges();
    }
    public void Delete(Vehicle vehicle)
    {
        _dbContext.Vehicles.Remove(vehicle);
        _dbContext.SaveChanges();
    }
} 