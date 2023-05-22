using System.Net.Mime;
using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace levitas.PoContract;
[JsonConverter(typeof(StringEnumConverter))]
public class PoDynamicField
{
    public bool Key { get; set; }
    public string Property { get; set; }
    public string Label { get; set; }
    public int? GridColumns { get; set; }
    public bool Visible { get; set; } = true;
    public string Divider { get; set; }
    public string booleanTrue { get; set; } = "Sim";
    public string booleanFalse { get; set; } = "Não";

    public PoDynamicFieldType Type { get; set; }
    public bool Image { get; set; }
    public string Alt { get; set; }
    public int Height { get; set; }
}
