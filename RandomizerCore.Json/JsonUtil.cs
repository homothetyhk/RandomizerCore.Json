using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RandomizerCore.Logic;
using RandomizerCore.Json.Converters;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace RandomizerCore.Json
{
    public static class JsonUtil
    {
        /// <summary>
        /// Creates a serializer with special handling for terms, logic defs, and so on, based on the contents of the provided LogicManager.
        /// </summary>
        public static JsonSerializer GetLogicSerializer(LogicManager lm)
        {
            JsonSerializer js = GetNonLogicSerializer();
            js.AddLogicManager(lm);
            return js;
        }

        /// <summary>
        /// Adds converters and contract resolver to give the serializer special handling for terms, logic defs, and so on, based on the contents of the provided LogicManager.
        /// <br/>Removes any components corresponding to another LogicManager.
        /// </summary>
        public static void AddLogicManager(this JsonSerializer js, LogicManager lm)
        {
            js.RemoveLogicManager();
            js.Converters.Add(new TermConverter() { Terms = lm.Terms });
            js.Converters.Add(new StateFieldConverter() { SM = lm.StateManager });
            js.Converters.Add(new LogicObjectWriter() { LM = lm });
            js.Converters.Add(new LogicDefReader() { LM = lm });
            js.ContractResolver = new LogicContractResolver(lm, js.ContractResolver);
        }

        /// <summary>
        /// Removes LogicManager-dependent converters and contract resolver.
        /// </summary>
        public static void RemoveLogicManager(this JsonSerializer js)
        {
            for (int i = js.Converters.Count - 1; i >= 0; i--)
            {
                switch (js.Converters[i])
                {
                    case TermConverter or StateFieldConverter or LogicObjectWriter or LogicDefReader:
                        js.Converters.RemoveAt(i);
                        break;
                }
            }
            if (js.ContractResolver is LogicContractResolver { Inner: IContractResolver cr }) js.ContractResolver = cr;
        }

        public static T? DeserializeFromEmbeddedResource<T>(Assembly a, string resourcePath) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromEmbeddedResource<T>(a, resourcePath);
        }

        public static T? DeserializeFromEmbeddedResource<T>(this JsonSerializer js, Assembly a, string resourcePath) where T : class
        {
            return js.DeserializeFromStream<T>(a.GetManifestResourceStream(resourcePath));
        }

        public static T? DeserializeFromFile<T>(string filepath) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromFile<T>(filepath);
        }

        public static T? DeserializeFromFile<T>(this JsonSerializer js, string filepath) where T : class
        {
            return js.DeserializeFromStream<T>(File.OpenRead(filepath));
        }

        public static T? DeserializeFromStream<T>(Stream s) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromStream<T>(s);
        }

        public static T? DeserializeFromStream<T>(this JsonSerializer js, Stream s) where T : class
        {
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);
            return js.DeserializeFromReader<T>(jtr);
        }

        public static T? DeserializeFromString<T>(string json) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromString<T>(json);
        }

        public static T? DeserializeFromString<T>(this JsonSerializer js, string json) where T : class
        {
            using StringReader sr = new(json);
            using JsonTextReader jtr = new(sr);
            return js.DeserializeFromReader<T>(jtr);
        }

        public static T? DeserializeFromReader<T>(JsonReader jr) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromReader<T>(jr);
        }

        public static T? DeserializeFromReader<T>(this JsonSerializer js, JsonReader jr) where T : class
        {
            return js.Deserialize<T>(jr);
        }

        public static T? DeserializeFromToken<T>(JToken t) where T : class
        {
            return GetNonLogicSerializer().DeserializeFromToken<T>(t);
        }

        public static T? DeserializeFromToken<T>(this JsonSerializer js, JToken t) where T : class
        {
            return t.ToObject<T>(js);
        }

        public static string SerializeToString(object o, Type? type = null)
        {
            return GetNonLogicSerializer().SerializeToString(o, type);
        }

        public static string SerializeToString(this JsonSerializer js, object o, Type? type = null)
        {
            using StringWriter sw = new();
            if (type is null)
            {
                js.Serialize(sw, o);
            }
            else
            {
                js.Serialize(sw, o, type);
            }
            return sw.ToString();
        }

        public static void SerializeToFile(string filepath, object o, Type? type = null)
        {
            GetNonLogicSerializer().SerializeToFile(filepath, o, type);
        }

        public static void SerializeToFile(this JsonSerializer js, string filepath, object o, Type? type = null)
        {
            using StreamWriter sw = new(File.OpenWrite(filepath));
            if (type is null)
            {
                js.Serialize(sw, o);
            }
            else
            {
                js.Serialize(sw, o, type);
            }
        }

        /// <summary>
        /// Returns a new general purpose serializer. Appropriate for handling LogicManager or RandoContext (de)serialization.
        /// </summary>
        public static JsonSerializer GetNonLogicSerializer()
        {
            JsonSerializer js = new()
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ContractResolver = new RCContractResolver(),
                SerializationBinder = new RCSerializationBinder(),
            };

            js.Converters.Add(new StringEnumConverter());
            js.Converters.Add(new LogicProcessorConverter());
            js.Converters.Add(new LMConverter());
            js.Converters.Add(new RandoContextConverter());
            return js;
        }
    }

}
