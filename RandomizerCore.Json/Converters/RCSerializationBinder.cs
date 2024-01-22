using Newtonsoft.Json.Serialization;
using RandomizerCore.Logic;

namespace RandomizerCore.Json.Converters
{
    public class RCSerializationBinder : DefaultSerializationBinder
    {
        public override Type BindToType(string? assemblyName, string typeName)
        {
            if (typeName == "RandomizerCore.Logic.OptimizedLogicDef") return typeof(DNFLogicDef);
            return base.BindToType(assemblyName, typeName);
        }
    }
}
