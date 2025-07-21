using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interfaces;

public interface IAdminServices
{
    Admin? Login(LoginDTO loginDTO);
    Admin Post(Admin admin);
    List<Admin> GetAll(int? page);
    Admin? GetId(int id);
}