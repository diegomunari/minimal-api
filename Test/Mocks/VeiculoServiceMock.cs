using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Test.Mocks;

public class VeiculoServiceMock : IVeiculoServico
{
    private static List<Veiculo> veiculos = new List<Veiculo>() {
        new Veiculo {
            Marca = "Jeep",
            Ano = 2024,
            Nome = "Renegade Longitude",
            Id = 1
        },
        new Veiculo {
            Marca = "Volkswagen",
            Ano = 2020,
            Nome = "Polo Highline",
            Id = 2
        }
    };

    public void Alterar(Veiculo veiculo)
    {
        throw new NotImplementedException();
    }

    public void Apagar(Veiculo veiculo)
    {
        veiculos.Remove(veiculo);
    }

    public List<Veiculo> GetAll(int pagina, string nome, string marca)
    {
        return veiculos;
    }

    public Veiculo GetById(int id)
    {
        return veiculos.Find(x => x.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculos.Add(veiculo);
    }
}