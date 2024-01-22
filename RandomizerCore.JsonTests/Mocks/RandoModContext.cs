using Newtonsoft.Json;
using RandomizerCore.Logic;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class RandoModContext : RandoContext
    {
        public RandoModContext(LogicManager LM) : base(LM)
        {
        }

        public override IEnumerable<GeneralizedPlacement> EnumerateExistingPlacements()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GenerationSettings { get; init; }
        public Dictionary<string, object> StartDef { get; init; }
        public List<GeneralizedPlacement> Vanilla = new();
        public List<ItemPlacement> itemPlacements = new();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TransitionPlacement> transitionPlacements = new();
        public List<int> notchCosts = new();
        public Dictionary<string, object> Properties { get; } = [];
    }
}
