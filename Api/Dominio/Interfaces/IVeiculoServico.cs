using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico {

    List<Veiculo> GetAll(int pagina, string nome, string marca);
    Veiculo GetById(int id);
    void Incluir(Veiculo veiculo);
    void Alterar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);
}