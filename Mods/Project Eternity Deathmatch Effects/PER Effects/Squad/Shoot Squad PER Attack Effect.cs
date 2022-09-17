using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class ShootSquadPERAttackEffect : DeathmatchSquadPEREffect
    {
        public static string Name = "Shoot Squad PER Attack";

        public ShootSquadPERAttackEffect()
            : base(Name, false)
        {
        }

        public ShootSquadPERAttackEffect(SquadPERParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            ActionPanelAttackPER.CreateAttack(Params.LocalContext.Map, Params.LocalContext.Map.ActivePlayerIndex, Params.LocalContext.Target, Params.LocalContext.TargetWeapon,
                Params.LocalContext.TargetWeaponPosition,
                Params.LocalContext.Map.CursorPosition - Params.LocalContext.Target.Position,
                ListFollowingSkill);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ShootSquadPERAttackEffect NewEffect = new ShootSquadPERAttackEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
