using System.Collections.Generic;

namespace levitas.PoContract;
public class PoDynamicMetaData
{
    public int Version { get; set; }
    public string Title { get; set; }
     public List<PoDynamicField> Fields { get; set; } = new List<PoDynamicField>();
     public List<PoDynamicAction> Actions { get; set; } = new List<PoDynamicAction>();
     public bool KeepFilters { get; set; }  = true;
}