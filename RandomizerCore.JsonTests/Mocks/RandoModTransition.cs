using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class RandoModTransition : RandoTransition
    {
        public Dictionary<string, object> TransitionDef;

        public RandoModTransition(LogicTransition lt) : base(lt)
        {
        }
    }
}
