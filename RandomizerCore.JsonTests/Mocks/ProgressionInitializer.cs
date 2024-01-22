using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class ProgressionInitializer : ILogicItem
    {
        public List<TermValue> Setters = new();
        public List<TermValue> Increments = new();
        public List<Term> StartStateLinkedTerms = new();
        public Term StartStateTerm;

        public string Name => "Progression Initializer";

        public void AddTo(ProgressionManager pm)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Term> GetAffectedTerms()
        {
            throw new NotImplementedException();
        }
    }
}
