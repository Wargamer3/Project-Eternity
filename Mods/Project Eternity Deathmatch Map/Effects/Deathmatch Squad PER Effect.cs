using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchSquadPEREffect : SquadPEREffect
    {
        protected readonly SquadPERParams Params;

        public DeathmatchSquadPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        /// <summary>
        /// Used for reflection
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="EffectContext"></param>
        public DeathmatchSquadPEREffect(string EffectTypeName, bool IsPassive, SquadPERParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new SquadPERParams(Params);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }
}
