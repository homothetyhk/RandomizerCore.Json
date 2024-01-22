using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class MockStateModifier : StateModifier
    {
        public override string Name { get; }

        public MockStateModifier(string name)
        {
            Name = name;
        }

        public override IEnumerable<Term> GetTerms()
        {
            yield break;
        }

        public override IEnumerable<LazyStateBuilder> ModifyState(object? sender, ProgressionManager pm, LazyStateBuilder state)
        {
            throw new NotImplementedException();
        }
    }
}
