using System;
using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public sealed class CharacterLevelUnlockRequirementEvaluator : UnlockRequirementEvaluator
    {
        public const string UnlockTypeName = "Character Level";

        private byte _UnlockLevel;

        public CharacterLevelUnlockRequirementEvaluator()
            : base(UnlockTypeName)
        {
        }

        private CharacterLevelUnlockRequirementEvaluator(BinaryReader BR)
            : this()
        {
            _UnlockLevel = BR.ReadByte();
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(_UnlockLevel);
        }

        public override bool CanBeUnlocked()
        {
            return Params.Owner.Level >= _UnlockLevel;
        }

        public override UnlockRequirementEvaluator Copy()
        {
            return new CharacterLevelUnlockRequirementEvaluator();
        }

        public override UnlockRequirementEvaluator LoadCopy(BinaryReader BR)
        {
            return new CharacterLevelUnlockRequirementEvaluator(BR);
        }

        #region Properties

        [CategoryAttribute("Requirement"),
        DescriptionAttribute("Character Level required.")]
        public byte UnlockLevel
        {
            get { return _UnlockLevel; }
            set { _UnlockLevel = value; }
        }

        #endregion
    }
}
