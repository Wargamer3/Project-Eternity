using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AttackActionEffect : ActionEffect
    {
        private readonly LifeSimParams Params;

        private Weapon ReferenceWeapon;

        public LifeSimManualSkillTargetType TargetType;
        public List<BaseEffect> ListEffect;

        public AttackActionEffect()
            : base("Attack")
        {
        }

        public AttackActionEffect(BinaryReader BR)
            : this()
        {
        }

        public override void DoWrite(BinaryWriter BW)
        {
        }

        public void UseAction()
        {
            TargetType.AddAndExecuteEffect(null, null, null);
        }

        public override void OnEquip()
        {/*
            foreach (Weapon ActiveWeapon in Params.Owner.ListWeapon)
            {
                if (ActiveWeapon.Name == Params.Owner.ActiveWeaponName)
                {
                    ReferenceWeapon = ActiveWeapon;
                    break;
                }
            }*/
        }

        public override ActionEffect LoadCopy(BinaryReader BR)
        {
            return new AttackActionEffect(BR);
        }
    }
}
