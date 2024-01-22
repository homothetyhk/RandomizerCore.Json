using RandomizerCore.Json.Converters;

namespace RandomizerCore.JsonTests.Mocks
{
    public class MockSerializationBinder : RCSerializationBinder
    {
        public override Type? BindToType(string assemblyName, string typeName)
        {
            if (Type.GetType("RandomizerCore.JsonTests.Mocks." + typeName.Split('.')[^1]) is Type U) return U;
            if (typeName.EndsWith("BenchItemDef") || typeName.EndsWith("BenchLocationDef")) return typeof(Dictionary<string, object>);
            return base.BindToType(assemblyName, typeName);
        }
    }
}