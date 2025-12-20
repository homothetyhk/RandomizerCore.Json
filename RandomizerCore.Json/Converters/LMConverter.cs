using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using RandomizerCore.StringItems;
using RandomizerCore.StringLogic;

namespace RandomizerCore.Json.Converters
{
    public class LMConverter : JsonConverter<LogicManager>
    {
        public override LogicManager ReadJson(JsonReader reader, Type objectType, LogicManager? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            LogicManagerBuilder lmb = new();
            JsonLogicFormat fmt = new(serializer);

            JObject lm = JObject.Load(reader);

            lmb.VariableResolver = serializer.DeserializeFromToken<VariableResolver>(lm[nameof(LogicManager.VariableResolver)]!)!;

            if (lm.ContainsKey("Macros"))
            {
                lmb.LoadData(LogicFileType.Macros, fmt.LoadMacros(lm["Macros"]!.CreateReader()));
            }
            else if (lm["LP"] is JObject lp && lp["macros"] is JObject lpdict)
            {
                lmb.LoadData(LogicFileType.Macros, fmt.LoadMacros(lpdict.CreateReader()));
            }

            lmb.LoadData(LogicFileType.Terms, fmt.LoadTerms(lm["Terms"]!.CreateReader()));
            lmb.LoadData(LogicFileType.Locations, fmt.LoadLocations(lm["Logic"]!.CreateReader()));

            try
            {
                lmb.Waypoints.UnionWith(serializer.DeserializeFromToken<List<string>>(lm["Waypoints"]!)!); // terms and logic are already defined
            }
            catch (Exception)
            {
                lmb.LoadData(LogicFileType.Waypoints, fmt.LoadWaypoints(lm["Waypoints"]!.CreateReader()));
            }
            try
            {
                lmb.Transitions.UnionWith(serializer.DeserializeFromToken<List<string>>(lm["Transitions"]!)!); // terms and logic are already defined
            }
            catch (Exception)
            {
                lmb.LoadData(LogicFileType.Transitions, fmt.LoadTransitions(lm["Transitions"]!.CreateReader()));
            }
            lmb.LoadData(LogicFileType.Items, fmt.LoadItems(lm["Items"]!.CreateReader()));
            if (lm["ItemStrings"] is JToken itemStringsList)
            {
                lmb.LoadData(LogicFileType.ItemStrings, fmt.LoadItemStrings(itemStringsList.CreateReader()));
            }

            return new(lmb);
        }

        public override void WriteJson(JsonWriter writer, LogicManager? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            List<JsonConverter> removeConverters = new();
            for (int i = 0; i < serializer.Converters.Count; i++)
            {
                if (serializer.Converters[i] is LogicObjectWriter)
                {
                    removeConverters.Add(serializer.Converters[i]);
                    serializer.Converters.RemoveAt(i);
                    i--;
                }
            }

            writer.WriteStartObject();

            TermConverter tc = new() { Terms = value.Terms };
            StateFieldConverter sfc = new() { SM = value.StateManager };

            serializer.Converters.Add(tc);
            serializer.Converters.Add(sfc);

            writer.WritePropertyName("Terms");
            serializer.Serialize(writer, value.Terms.Terms.Select((c, i) => (c, i)).ToDictionary(p => ((TermType)p.i).ToString(), p => p.c.Select(t => t.Name).ToList()));

            writer.WritePropertyName("Logic");
            serializer.Serialize(writer, value.LogicLookup.Values.Select(l => new RawLogicDef(l.Name, l.InfixSource)));

            writer.WritePropertyName("Items");
            serializer.Serialize(writer, value.ItemLookup.Values.Where(i => i is not StringItem));

            writer.WritePropertyName("ItemStrings");
            serializer.Serialize(writer, value.ItemLookup.Values.OfType<StringItem>().Select(s => new StringItemTemplate(s.Name, s.EffectString)));

            writer.WritePropertyName("Transitions");
            serializer.Serialize(writer, value.TransitionLookup.Keys);

            writer.WritePropertyName("Waypoints");
            serializer.Serialize(writer, value.Waypoints.Select(w => w.Name));

            writer.WritePropertyName("Macros");
            serializer.Serialize(writer, value.MacroLookup.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.InfixSource));

            writer.WritePropertyName(nameof(value.VariableResolver));
            serializer.Serialize(writer, value.VariableResolver, typeof(VariableResolver));

            writer.WritePropertyName(nameof(value.StateManager));
            serializer.Serialize(writer, new RawStateData(value.StateManager));

            writer.WriteEndObject();
            serializer.Converters.Remove(sfc);
            serializer.Converters.Remove(tc);
            foreach (JsonConverter c in removeConverters) serializer.Converters.Add(c);
        }
    }
}
