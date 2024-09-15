using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infra.Db;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto) {
        _contexto = contexto;
    }

    public void Alterar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo> GetAll(int pagina = 1, string nome = "", string marca = "")
    {
        var query = _contexto.Veiculos;

        query.Where(x => x.Nome.Contains(nome) || x.Marca.Contains(marca));

        int itensPorPagina = 10;
        query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public Veiculo GetById(int id)
    {
        return _contexto.Veiculos.Where(x => x.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }
}