using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class RandoVariableResolver : VariableResolver
    {
        private static string[] stateProviderPrefixes =
        [
            "$StartLocation"
        ];

        private static string[] stateModifierPrefixes =
        [
            "$BENCHRESET",
            "$CASTSPELL",
            "$SLOPEBALL",
            "$SHRIEKPOGO",
            "$SHRIEKPOGOS",
            "$SPENDSOUL",
            "$REGAINSOUL",
            "$EQUIPPEDCHARM",
            "$HOTSPRINGRESET",
            "$SHADESKIP",
            "$TAKEDAMAGE",
            "$STAGSTATEMODIFIER",
            "$FLOWERGET",
            "$SAVEQUITRESET",
            "$STARTRESPAWN",
            "$WARPTOSTART",
            "$WARPTOBENCH",
        ];

        private static string[] logicIntPrefixes =
        [
            "$NotchCost",
            "$SafeNotchCost"
        ];


        public override bool TryMatch(LogicManager lm, string term, out LogicVariable variable)
        {
            foreach (string s in stateProviderPrefixes)
            {
                if (TryMatchPrefix(term, s, out _))
                {
                    variable = new MockStateProvider(term);
                    return true;
                }
            }
            foreach (string s in stateModifierPrefixes)
            {
                if (TryMatchPrefix(term, s, out _))
                {
                    variable = new MockStateModifier(term);
                    return true;
                }
            }
            foreach (string s in logicIntPrefixes)
            {
                if (TryMatchPrefix(term, s, out _))
                {
                    variable = new MockLogicInt(term);
                    return true;
                }
            }

            return base.TryMatch(lm, term, out variable);
        }
    }
}
