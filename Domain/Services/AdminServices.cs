using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infrastrucuture.Db;

namespace MinimalApi.Domain.Services;

public class AdminServices : IAdminServices
{
    private readonly ApplicationDbContext _dbContext;
    public AdminServices(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Admin? Login(LoginDTO loginDTO)
    {
        var result = _dbContext.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

        return result;
    }
}