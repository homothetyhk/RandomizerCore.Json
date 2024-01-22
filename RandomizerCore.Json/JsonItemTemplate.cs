using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomizerCore.Json;
using RandomizerCore.Json.Converters;
using RandomizerCore.Logic;

namespace RandomizerCore.LogicItems.Templates
{
    /// <summary>
    /// Item template wrapper for a json representation of an item.
    /// </summary>
    public record JsonItemTemplate : ILogicItemTemplate
    {
        public JsonItemTemplate(string json, JsonSerializer? js) : this(JToken.Parse(json), js) { }

        public JsonItemTemplate(JToken t, JsonSerializer? js)
        {
            Name = t.Value<string>("Name");
            JToken = t;
            Serializer = js;
        }

        public string Name { get; }
        public JToken JToken { get; }
        public JsonSerializer? Serializer { get; }

        [ThreadStatic] private static JsonSerializer? sharedSerializer;

        public LogicItem Create(LogicManager lm)
        {
            JsonSerializer js = Serializer ?? (sharedSerializer ??= new());
            if (js.ContractResolver is not LogicContractResolver lcr)
            {
                js.AddLogicManager(lm);
            }
            else if (!ReferenceEquals(lm, lcr.LM))
            {
                js.RemoveLogicManager();
                js.AddLogicManager(lm);
            }
            return js.DeserializeFromToken<LogicItem>(JToken);
        }
    }
}
