using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCore.JsonTests.Mocks
{
    internal record SplitCloakItem : LogicItem
    {
        public bool LeftBiased;
        public Term LeftDashTerm;
        public Term RightDashTerm;

        public SplitCloakItem(string Name) : base(Name)
        {
        }

        public override void AddTo(ProgressionManager pm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Term> GetAffectedTerms()
        {
            throw new NotImplementedException();
        }
    }
}
