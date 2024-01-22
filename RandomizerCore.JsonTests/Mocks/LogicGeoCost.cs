using RandomizerCore.Logic;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class LogicGeoCost : LogicCost
    {
        public Term CanReplenishGeoWaypoint;
        public int GeoAmount;

        public override bool CanGet(ProgressionManager pm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Term> GetTerms()
        {
            throw new NotImplementedException();
        }
    }
}
