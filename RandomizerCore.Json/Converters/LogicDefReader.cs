using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;

namespace RandomizerCore.Json.Converters
{
    public class LogicDefReader : JsonConverter<LogicDef>
    {
        public LogicManager? LM;

        public override LogicDef ReadJson(JsonReader reader, Type objectType, LogicDef? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken t = JToken.Load(reader);
            JObject o = (JObject)t;
            string? name = o.GetValue("name", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            string? logic = o.GetValue("logic", StringComparison.OrdinalIgnoreCase)?.Value<string>();

            if (name is null) throw new NullReferenceException($"Error deserializing logic def at {reader.Path}: null name.");
            if (LM is null) throw new NullReferenceException(nameof(LM));

            LogicDef? lDef = LM.GetLogicDef(name);

            if (logic is null || lDef is not null && lDef.InfixSource == logic && objectType.IsAssignableFrom(lDef.GetType()))
            {
                return lDef;
            }

            if (typeof(StateLogicDef).IsAssignableFrom(objectType) || objectType == typeof(LogicDef))
            {
                return LM.CreateDNFLogicDef(new RawLogicDef(name, logic));
            }
            else if (typeof(RPNLogicDef).IsAssignableFrom(objectType))
            {
                return LM.CreateRPNLogicDef(new RawLogicDef(name, logic));
            }
            else throw new NotSupportedException($"Cannot deserialize LogicDef at {reader.Path} to unexpected type {objectType.Name}");
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, LogicDef? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
