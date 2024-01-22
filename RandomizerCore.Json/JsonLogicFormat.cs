using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using RandomizerCore.LogicItems;
using RandomizerCore.LogicItems.Templates;
using RandomizerCore.StringItems;

namespace RandomizerCore.Json
{
    public class JsonLogicFormat : ILogicFormat
    {
        public JsonSerializer js;
        /// <summary>
        /// False by default. If false, the JsonSerializer attached to the JsonLogicFormat will be used to deserialize JsonItemTemplates during LogicManager construction.
        /// <br/>If true, a different generic JsonSerializer will be used.
        /// </summary>
        public bool UseDefaultSerializerForJsonItems { get; set; } = false;

        public JsonLogicFormat()
        {
            js = JsonUtil.GetNonLogicSerializer();
        }

        public JsonLogicFormat(JsonSerializer js)
        {
            this.js = js;
        }

        public IEnumerable<ILogicItemTemplate> LoadItems(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (ILogicItemTemplate template in LoadItems(jtr)) yield return template;
        }

        public IEnumerable<ILogicItemTemplate> LoadItems(JsonReader jr)
        {
            foreach (JToken jt in JArray.Load(jr))
            {
                yield return new JsonItemTemplate(jt, UseDefaultSerializerForJsonItems ? null : js);
            }
        }

        public IEnumerable<StringItemTemplate> LoadItemStrings(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (StringItemTemplate t in LoadItemStrings(jtr)) yield return t;
        }

        public IEnumerable<StringItemTemplate> LoadItemStrings(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<StringItemTemplate>>(jr) ?? [];
        }

        public IEnumerable<ILogicItemTemplate> LoadItemTemplates(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (ILogicItemTemplate t in LoadItemTemplates(jtr)) yield return t;
        }

        public IEnumerable<ILogicItemTemplate> LoadItemTemplates(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<ILogicItemTemplate>>(jr) ?? [];
        }

        public IEnumerable<RawLogicDef> LoadLocations(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (RawLogicDef def in LoadLocations(jtr)) yield return def;
        }

        public IEnumerable<RawLogicDef> LoadLocations(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<RawLogicDef>>(jr) ?? [];
        }

        public IEnumerable<RawLogicDef> LoadLogicEdits(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (RawLogicDef def in LoadLogicEdits(jtr)) yield return def;
        }

        public IEnumerable<RawLogicDef> LoadLogicEdits(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<RawLogicDef>>(jr) ?? [];
        }

        public IEnumerable<RawSubstDef> LoadLogicSubstitutions(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (RawSubstDef def in LoadLogicSubstitutions(jtr)) yield return def;
        }

        public IEnumerable<RawSubstDef> LoadLogicSubstitutions(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<RawSubstDef>>(jr) ?? [];
        }

        public Dictionary<string, string> LoadMacroEdits(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            return LoadMacroEdits(jtr);
        }

        public Dictionary<string, string> LoadMacroEdits(JsonReader jr)
        {
            return js.Deserialize<Dictionary<string, string>>(jr) ?? [];
        }

        public Dictionary<string, string> LoadMacros(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            return LoadMacros(jtr);
        }

        public Dictionary<string, string> LoadMacros(JsonReader jr)
        {
            return js.Deserialize<Dictionary<string, string>>(jr) ?? [];
        }

        public RawStateData LoadStateData(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            return LoadStateData(jtr);
        }

        public RawStateData LoadStateData(JsonReader jr)
        {
            return js.Deserialize<RawStateData>(jr) ?? new();
        }

        public IEnumerable<(string, TermType)> LoadTerms(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach ((string, TermType) t in LoadTerms(jtr)) yield return t;
        }

        public IEnumerable<(string, TermType)> LoadTerms(JsonReader jr)
        {
            JToken t = JToken.Load(jr);
            if (t is JObject o)
            {
                foreach (JProperty p in o.Properties())
                {
                    if (!Enum.TryParse(p.Name, true, out TermType termType))
                    {
                        Log($"Unable to parse {p.Name} as TermType, assuming Int...");
                        termType = TermType.Int;
                    }

                    foreach (JValue v in p.Value.Cast<JValue>())
                    {
                        yield return ((string)v.Value, termType);
                    }
                }
            }
            else if (t.Type == JTokenType.Array) // old format predating state logic
            {
                foreach (string term in t.ToObject<List<string>>())
                {
                    yield return (term, TermType.Int);
                }
            }
            else throw new NotSupportedException();
        }

        public IEnumerable<RawLogicDef> LoadTransitions(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (RawLogicDef def in LoadTransitions(jtr)) yield return def;
        }

        public IEnumerable<RawLogicDef> LoadTransitions(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<RawLogicDef>>(jr) ?? [];
        }

        public IEnumerable<RawWaypointDef> LoadWaypoints(Stream s)
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            foreach (RawWaypointDef def in LoadWaypoints(jtr)) yield return def;
        }

        public IEnumerable<RawWaypointDef> LoadWaypoints(JsonReader jr)
        {
            return js.Deserialize<IEnumerable<RawWaypointDef>>(jr) ?? [];
        }
    }
}
