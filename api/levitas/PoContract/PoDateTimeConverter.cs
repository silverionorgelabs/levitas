using System;
using System.Globalization;
using Newtonsoft.Json;

namespace levitas.PoContract;

public class PoDateTimeConverter : JsonConverter
{
    private const string DateFormat = "yyyy-MM-dd";

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
        {
            return null;
        }

        if (reader.ValueType == typeof(DateTime))
        {
            return reader.Value;
        }

        if (reader.ValueType == typeof(string))
        {
            string dateString = (string)reader.Value;
            DateTime date;
            if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
        }

        throw new JsonSerializationException($"Cannot convert {reader.Value} to {objectType}");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(((DateTime)value).ToString(DateFormat));
    }
}