using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdminServico {

    Admin? Login(LoginDTO loginDTO);
    void Incluir(Admin admin);
    void Alterar(Admin admin);
    void Apagar(Admin admin);
    List<Admin> GetAll();
    Admin GetById(int id);
}