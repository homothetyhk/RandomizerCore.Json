using RandomizerCore.Logic;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class MockLogicInt : LogicInt
    {
        public override string Name { get; }

        public MockLogicInt(string name)
        {
            Name = name;
        }

        public override IEnumerable<Term> GetTerms()
        {
            yield break;
        }

        public override int GetValue(object? sender, ProgressionManager pm)
        {
            throw new NotImplementedException();
        }
    }
}
