using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetEffect : BaseEffect
    {
        protected readonly SorcererStreetBattleParams Params;

        protected SorcererStreetEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        /// <summary>
        /// Used for reflection
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="EffectContext"></param>
        protected SorcererStreetEffect(string EffectTypeName, bool IsPassive, SorcererStreetBattleParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new SorcererStreetBattleParams(Params);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }
    }
}
