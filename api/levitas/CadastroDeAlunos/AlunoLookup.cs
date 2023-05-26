using System.Linq;
using levitas.PoContract;
using Microsoft.AspNetCore.Http;

namespace levitas.CadastroDeAlunos;
public class AlunoLookup : PoLookup<Aluno>
{

    public static AlunoLookup Create(HttpRequest req)
    {
        return new AlunoLookup(req);
    }
    public AlunoLookup(HttpRequest req) : base(req)
    {
        nome = req.Query.ContainsKey("nome") ? req.Query["nome"].ToString() : string.Empty;
        nomeDoResponsavel = req.Query.ContainsKey("nomeDoResponsavel") ? req.Query["nomeDoResponsavel"].ToString() : string.Empty;
    }

    public string nome { get; set; }
    public string nomeDoResponsavel { get; set; }

    public override IQueryable<Aluno> BuildQuery(IQueryable<Aluno> query)
    {
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Nome.ToLower().Contains(search.ToLower()));

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(x => x.Nome.ToLower().Contains(nome.ToLower()));

        if (!string.IsNullOrEmpty(nomeDoResponsavel))
            query = query.Where(x => x.NomeDoResponsavel.ToLower().Contains(nomeDoResponsavel.ToLower()));


        if (!string.IsNullOrEmpty(order))
        {
            if (order.StartsWith("-"))
            {

                query = query.OrderByDescending(x => x.Nome);
            }
            else
            {
                query = query.OrderBy(x => x.Nome);
            }
            query = query.OrderBy(x => x.Nome);
        }
        return query;
    }
}