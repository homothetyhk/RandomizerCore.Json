using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCore.JsonTests.Mocks
{
    internal class ItemPlacement
    {
        public RandoModItem Item { get; init; }
        public RandoModLocation Location { get; init; }
        public int Index { get; init; } = -1;
    }
}
