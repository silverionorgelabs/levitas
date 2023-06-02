using System.Linq;
using levitas.CadastroDeAlunos;
using Microsoft.AspNetCore.Http;

namespace levitas.PoContract;
public abstract  class PoLookup<T>
{
    public string order { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    public string search { get; set; }

    public PoLookup(HttpRequest req)
    {
        page = req.Query.ContainsKey("page") ? int.Parse(req.Query["page"]) : 1;
        pageSize = req.Query.ContainsKey("pageSize") ? int.Parse(req.Query["pageSize"]) : 10;
        order = req.Query.ContainsKey("order") ? req.Query["order"].ToString() : string.Empty;
        search = req.Query.ContainsKey("search") ? req.Query["search"].ToString() : string.Empty;
    }



    public abstract IQueryable<T> BuildQuery(IQueryable<T> query);

    public IQueryable<T> SkipReturn(IQueryable<T> query)
    {
        
        var skip = (page - 1) * pageSize;
        var take = pageSize;
        query = query.Skip(skip).Take(take);

        return query;
    }
}
