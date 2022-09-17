using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class CreateProjectilePERAttackEffect : DeathmatchAttackPEREffect
    {
        public static string Name = "Create Projectile PER Attack";

        private string _AttackName;
        private Vector3 _Speed;
        private Attack WeaponToUse;

        public CreateProjectilePERAttackEffect()
            : base(Name, false)
        {
            _AttackName = string.Empty;
        }

        public CreateProjectilePERAttackEffect(AttackPERParams Params)
            : base(Name, false, Params)
        {
            _AttackName = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _AttackName = BR.ReadString();
            _Speed = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_AttackName);
            BW.Write(_Speed.X);
            BW.Write(_Speed.Y);
            BW.Write(_Speed.Z);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            ActionPanelAttackPER.CreateAttack(Params.LocalContext.Map, Params.LocalContext.Map.ActivePlayerIndex, Params.LocalContext.Owner, WeaponToUse, Params.SharedParams.OwnerPosition, _Speed, ListFollowingSkill);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            CreateProjectilePERAttackEffect NewEffect = new CreateProjectilePERAttackEffect(Params);

            NewEffect._AttackName = _AttackName;
            NewEffect._Speed = _Speed;
            if (Params != null && !string.IsNullOrEmpty(_AttackName))
            {
                NewEffect.WeaponToUse = new Attack(_AttackName, Params.SharedParams.Content, NewEffect.Params.LocalContext.Map.Params.DicRequirement, NewEffect.Params.LocalContext.Map.Params.DicEffect, NewEffect.Params.LocalContext.Map.Params.DicAutomaticSkillTarget);
            }

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            CreateProjectilePERAttackEffect NewEffect = (CreateProjectilePERAttackEffect)Copy;

            _AttackName = NewEffect._AttackName;
            _Speed = NewEffect._Speed;

            if (WeaponToUse == null && Params != null && !string.IsNullOrEmpty(_AttackName))
            {
                WeaponToUse = new Attack(_AttackName, Params.SharedParams.Content, Params.LocalContext.Map.Params.DicRequirement, Params.LocalContext.Map.Params.DicEffect, Params.LocalContext.Map.Params.DicAutomaticSkillTarget);
            }
        }

        #region Properties

        [Editor(typeof(AttackSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner"),
        DescriptionAttribute("The Weapon path."),
        DefaultValueAttribute(0)]
        public string AttackName
        {
            get
            {
                return _AttackName;
            }
            set
            {
                _AttackName = value;
            }
        }

        [CategoryAttribute("Spawner"),
        DescriptionAttribute("The Attack path."),
        DefaultValueAttribute(0)]
        public Vector3 Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                _Speed = value;
            }
        }

        #endregion
    }
}
