using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Effects
{
    public class UnitEffectContext
    {
        private Squad _EffectOwnerSquad;
        private Unit _EffectOwnerUnit;
        private Character _EffectOwnerCharacter;

        private Squad _EffectTargetSquad;
        private Unit _EffectTargetUnit;
        private Character _EffectTargetCharacter;

        private FormulaParser _ActiveParser;

        public Squad EffectOwnerSquad { get { return _EffectOwnerSquad; } }
        public Unit EffectOwnerUnit { get { return _EffectOwnerUnit; } }
        public Character EffectOwnerCharacter { get { return _EffectOwnerCharacter; } }

        public Squad EffectTargetSquad { get { return _EffectTargetSquad; } }
        public Unit EffectTargetUnit { get { return _EffectTargetUnit; } }
        public Character EffectTargetCharacter { get { return _EffectTargetCharacter; } }
        public FormulaParser ActiveParser { get { return _ActiveParser; } }

        public void SetContext(Squad EffectOwnerSquad, Unit EffectOwnerUnit, Character EffectOwnerCharacter,
            Squad EffectTargetSquad, Unit EffectTargetUnit, Character EffectTargetCharacter, FormulaParser ActiveParser)
        {
            _EffectOwnerSquad = EffectOwnerSquad;
            _EffectOwnerUnit = EffectOwnerUnit;
            _EffectOwnerCharacter = EffectOwnerCharacter;

            _EffectTargetSquad = EffectTargetSquad;
            _EffectTargetUnit = EffectTargetUnit;
            _EffectTargetCharacter = EffectTargetCharacter;

            _ActiveParser = ActiveParser;
        }
    }
}
