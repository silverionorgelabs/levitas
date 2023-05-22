using System.Collections.Generic;

namespace levitas.PoContract;
public class PoMessage
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string DetailedMessage { get; set; }
    public List<PoMessage> Details { get; set; }
}
