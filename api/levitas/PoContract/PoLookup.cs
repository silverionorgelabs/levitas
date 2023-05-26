using System.Linq;
using levitas.CadastroDeAlunos;
using Microsoft.AspNetCore.Http;

namespace levitas.PoContract;
public class PoLookup
{
    public string order { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    public string search { get; set; }
    public string nome { get; set; }
    public string nomeDoResponsavel { get; set; }

    public static PoLookup Create(HttpRequest req)
    => new PoLookup
    {
        page = req.Query.ContainsKey("page") ? int.Parse(req.Query["page"]) : 1,
        pageSize = req.Query.ContainsKey("pageSize") ? int.Parse(req.Query["pageSize"]) : 10,
        order = req.Query.ContainsKey("order") ? req.Query["order"].ToString() : string.Empty,
        search = req.Query.ContainsKey("search") ? req.Query["search"].ToString() : string.Empty,
        nome = req.Query.ContainsKey("nome") ? req.Query["nome"].ToString() : string.Empty,
        nomeDoResponsavel = req.Query.ContainsKey("nomeDoResponsavel") ? req.Query["nomeDoResponsavel"].ToString() : string.Empty
    };

    public IQueryable<Aluno> BuildQuery(IQueryable<Aluno> query)
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

        var skip = (page - 1) * pageSize;
        var take = pageSize;
        query = query.Skip(skip).Take(take);

        return query;
    }
}
