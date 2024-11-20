using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Test.Mocks;

public class AdminServiceMock : IAdminServico
{
    private static List<Admin> admins = new List<Admin>(){
        new Admin  {
            Email = "admin@teste.com",
            Id = 1,
            Senha = "1234",
            Perfil = "admin"
        },
        new Admin  {
            Email = "editor@teste.com",
            Id = 2,
            Senha = "1234",
            Perfil = "editor"
        }
    };

    public void Alterar(Admin admin)
    {
        throw new NotImplementedException();
    }

    public void Apagar(Admin admin)
    {
        admins.Remove(admin);
    }

    public List<Admin> GetAll()
    {
        return admins;
    }

    public Admin GetById(int id)
    {
        return admins.Find(x => x.Id == id);
    }

    public void Incluir(Admin admin)
    {
        admin.Id = admins.Count() + 1;
        admins.Add(admin);

    }

    public Admin? Login(LoginDTO loginDTO)
    {
        return admins.Find(x => x.Email == loginDTO.Email &&
                                x.Senha == loginDTO.Senha);
    }
}