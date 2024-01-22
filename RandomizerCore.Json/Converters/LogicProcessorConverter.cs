using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomizerCore.StringLogic;

namespace RandomizerCore.Json.Converters
{
    public class LogicProcessorConverter : JsonConverter<LogicProcessor>
    {
        public override LogicProcessor? ReadJson(JsonReader reader, Type objectType, LogicProcessor? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            JObject o = JObject.Load(reader);
            return new(o["macros"]?.ToObject<Dictionary<string, string>>() ?? []);
        }

        public override void WriteJson(JsonWriter writer, LogicProcessor? value, JsonSerializer serializer)
        {
            if (value is null) writer.WriteNull();
            else serializer.Serialize(writer, new { macros = value.Macros.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToInfix()) });
        }
    }
}
