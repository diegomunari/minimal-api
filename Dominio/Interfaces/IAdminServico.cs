using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdminServico {

    Admin? Login(LoginDTO loginDTO);
    void Incluir(Admin veiculo);
    void Alterar(Admin veiculo);
    void Apagar(Admin veiculo);
    List<Admin> GetAll();
    Admin GetById(int id);
}