using System.Collections.Generic;

namespace levitas.PoContract;
public class PoSuccessResponseCollections<T> : PoSuccessResponse
{
    public bool HasNext { get; set; }
    public List<T> Items { get; set; }
}
