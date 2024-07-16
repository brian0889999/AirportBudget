using System.Text.Json.Serialization;
using System.Text.Json;


namespace AirportBudget.Server.Utilities;

public class JsonConverterUtility
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "yyyy-MM-ddTHH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime(); // 保持原有的反序列化行為
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
