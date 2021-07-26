using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.Core.Effects
{
    public class UnitQuickLoadEffectContext
    {
        private Dictionary<uint, Squad> _ArrayEffectOwnerSquad;

        public Dictionary<uint, Squad> ArrayEffectOwnerSquad { get { return _ArrayEffectOwnerSquad; } }

        public void SetContext(Dictionary<uint, Squad> ArrayEffectOwnerSquad)
        {
            _ArrayEffectOwnerSquad = ArrayEffectOwnerSquad;
        }
    }
}
