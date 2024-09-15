using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infra.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdminServico : IAdminServico
{
    private readonly DbContexto _contexto;

    public AdminServico(DbContexto contexto) {
        _contexto = contexto;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        return _contexto.Admins.Where(
            x => x.Email == loginDTO.Email &&
            x.Senha == loginDTO.Senha).FirstOrDefault();
    }

    public void Alterar(Admin admin)
    {
        _contexto.Admins.Update(admin);
        _contexto.SaveChanges();
    }

    public void Apagar(Admin admin)
    {
        _contexto.Admins.Remove(admin);
        _contexto.SaveChanges();
    }

    public void Incluir(Admin admin)
    {
        _contexto.Admins.Add(admin);
        _contexto.SaveChanges();
    }

    public List<Admin> GetAll()
    {
        var query = _contexto.Admins;  
        return query.ToList();
    }

    public Admin GetById(int id)
    {
        var query = _contexto.Admins.Find(id);  
        return query;
    }
}