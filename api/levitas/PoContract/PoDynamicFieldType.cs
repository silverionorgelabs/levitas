using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace levitas.PoContract;
[JsonConverter(typeof(StringEnumConverter))]
public enum PoDynamicFieldType
{

    boolean,

    currency,

    date,

    dateTime,

    time,

    number,
    [EnumMember(Value = "string")]
    text,

    upload
}


