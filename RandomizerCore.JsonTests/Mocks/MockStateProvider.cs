using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class MockStateProvider : StateProvider
    {
        public override string Name { get; }

        public MockStateProvider(string name)
        {
            Name = name;
        }

        public override StateUnion? GetInputState(object? sender, ProgressionManager pm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Term> GetTerms()
        {
            yield break;
        }
    }
}
