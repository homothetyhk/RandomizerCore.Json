using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RandomizerCore.Logic;

namespace RandomizerCore.Json.Converters
{
    public class RandoContextConverter : JsonConverter<RandoContext>
    {
        public override RandoContext ReadJson(JsonReader reader, Type objectType, RandoContext existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            LogicManager lm = jo[nameof(RandoContext.LM)].ToObject<LogicManager>(serializer);
            RandoContext ctx = (RandoContext)Activator.CreateInstance(objectType, lm);
            jo.Remove(nameof(RandoContext.LM));

            serializer.AddLogicManager(lm);
            JsonReader jr = jo.CreateReader();
            serializer.Populate(jr, ctx);
            serializer.RemoveLogicManager();

            return ctx;
        }

        public bool skipNextWrite;
        public override bool CanWrite => !skipNextWrite || (skipNextWrite = false);

        public override void WriteJson(JsonWriter writer, RandoContext value, JsonSerializer serializer)
        {
            serializer.AddLogicManager(value.LM);
            skipNextWrite = true;
            serializer.Serialize(writer, value);
            serializer.RemoveLogicManager();
        }
    }
}
