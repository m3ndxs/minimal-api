using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IVehicleServices
{
    List<Vehicle> GetAll(int? page = 1, string? name = null, string? brand = null);
    Vehicle? GetId(int id);
    void Post(Vehicle vehicle);
    void Put(Vehicle vehicle);
    void Delete(Vehicle vehicle);
}