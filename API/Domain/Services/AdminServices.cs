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
    public Admin Post(Admin admin)
    {
        _dbContext.Admins.Add(admin);

        _dbContext.SaveChanges();

        return admin;
    }
    public List<Admin> GetAll(int? page)
    {
        var query = _dbContext.Admins.AsQueryable();

        int itemsPerPage = 10;

        if (page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return query.ToList();
    }

    public Admin? GetId(int id)
    {
        return _dbContext.Admins.Where(a => a.Id == id).FirstOrDefault();
    }
}