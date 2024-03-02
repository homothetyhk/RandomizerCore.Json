using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RandomizerCore.Logic;
using RandomizerCore.StringItems;
using System.Reflection;

namespace RandomizerCore.Json.Converters
{
    public class RCContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty p = base.CreateProperty(member, memberSerialization);
            if (typeof(LogicDef).IsAssignableFrom(member.DeclaringType) && p.PropertyName == nameof(LogicDef.InfixSource))
            {
                p.PropertyName = "Logic";
            }
            return p;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract c = base.CreateObjectContract(objectType);

            if (objectType == typeof(RawWaypointDef))
            {
                c.Properties[nameof(RawWaypointDef.stateless)].DefaultValueHandling = DefaultValueHandling.Ignore;
            }
            else if (typeof(RandoLocation).IsAssignableFrom(objectType))
            {
                c.Properties[nameof(RandoLocation.Name)].Ignored = true;
            }
            else if (objectType == typeof(FirstOfEffect) || objectType == typeof(AllOfEffect))
            {
                JsonProperty p = base.CreateProperty(objectType.GetField("Effects", BindingFlags.Instance | BindingFlags.NonPublic), MemberSerialization.Fields);
                c.Properties.AddProperty(p);
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(p);
            }

            return c;
        }
    }
}
