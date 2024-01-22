using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RandomizerCore.Logic;
using System.Reflection;

namespace RandomizerCore.Json.Converters
{
    public class LogicContractResolver : DefaultContractResolver
    {
        public LogicManager LM { get; }

        public IContractResolver Inner { get; }

        private readonly HashSet<JsonObjectContract> modifiedContracts = new(RandomizerCore.Collections.ReferenceEqualityComparer<JsonObjectContract>.Instance);

        public LogicContractResolver(LogicManager lm, IContractResolver cr)
        {
            LM = lm;
            Inner = cr;
        }

        public override JsonContract ResolveContract(Type type)
        {
            JsonContract c = Inner.ResolveContract(type);

            if (c is JsonObjectContract o && modifiedContracts.Add(o))
            {
                ModifyObjectContract(type, o);
            }

            return c;
        }

        public void ModifyObjectContract(Type objectType, JsonObjectContract c)
        {
            // for abstract types, the contract is only used when deserializing a JObject annotated with exactly the abstract type.
            // here, we use this to denote that the contents of the logic object are serialized elsewhere with the LM.
            if (objectType == typeof(LogicDef)) 
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]);
                c.OverrideCreator = (args) => LM.GetLogicDefStrict((string)args[0]);
            }
            else if (objectType == typeof(LogicItem))
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]);
                c.OverrideCreator = (args) => LM.GetItemStrict((string)args[0]); // safe since we are modifying handling for the abstract type, which is only annotated by the custom converter
            }
            else if (objectType == typeof(LogicTransition))
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]); // if LogicObjectWriter was used
                c.CreatorParameters.Add(c.Properties["term"]); // for previous version compatibility, since Name used to be JsonIgnored
                c.Properties[nameof(LogicTransition.logic)].Ignored = true;
                c.OverrideCreator = (args) => LM.GetTransitionStrict(args[0] as string ?? args[1]?.ToString());
            }
            else if (objectType == typeof(LogicWaypoint))
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]);
                c.OverrideCreator = (args) => LM.Waypoints.First(w => w.Name == (string)args[0]);
            }
            else if (objectType == typeof(DNFLogicDef))
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]);
                c.CreatorParameters.Add(c.Properties["Logic"]);
                c.OverrideCreator = (args) => LM.CreateDNFLogicDef(new((string)args[0], (string)args[1]));
            }
            else if (objectType == typeof(RPNLogicDef))
            {
                c.CreatorParameters.Clear();
                c.CreatorParameters.Add(c.Properties["Name"]);
                c.CreatorParameters.Add(c.Properties["Logic"]);
                c.OverrideCreator = (args) => LM.CreateRPNLogicDef(new((string)args[0], (string)args[1]));
            }
        }
    }
}
