using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TeamManageSystem.Models.Account;

namespace TeamManageSystem.Controllers.Account
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long unixTime = ((DateTime)value).Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTime /= TimeSpan.TicksPerMillisecond;
            writer.WriteValue(unixTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                // Handle null values by returning null
                return null;
            }

            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.String)
            {
                if (reader.Value is long unixTime)
                {
                    return new DateTime(1970, 1, 1).AddMilliseconds(unixTime);
                }
                else if (reader.Value is string unixTimeString && long.TryParse(unixTimeString, out long unixTimeParsed))
                {
                    return new DateTime(1970, 1, 1).AddMilliseconds(unixTimeParsed);
                }
            }

            throw new JsonSerializationException("Unexpected token type or format: " + reader.TokenType);
        }
    }
}
