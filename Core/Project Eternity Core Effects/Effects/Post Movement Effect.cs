﻿using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class PostMovementEffect : SkillEffect
    {
        public static string Name = "Post Movement Effect";

        private bool _Attack;
        private bool _Transform;
        private bool _Spirit;
        private bool _Move;

        public PostMovementEffect()
            : base(Name, true)
        {
            _Attack = false;
            _Transform = false;
            _Spirit = false;
            _Move = false;
        }

        public PostMovementEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(System.IO.BinaryReader BR)
        {
            _Attack = BR.ReadBoolean();
            _Transform = BR.ReadBoolean();
            _Spirit = BR.ReadBoolean();
            _Move = BR.ReadBoolean();
        }

        protected override void Save(System.IO.BinaryWriter BW)
        {
            BW.Write(_Attack);
            BW.Write(_Transform);
            BW.Write(_Spirit);
            BW.Write(_Move);
        }

        public override bool CanActivate()
        {
            bool CanActivateAttack = Params.GlobalContext.EffectTargetUnit.Boosts.PostMovementModifier.Attack != _Attack;
            bool CanActivateTransform = Params.GlobalContext.EffectTargetUnit.Boosts.PostMovementModifier.Transform != _Transform;
            bool CanActivateSpirit = Params.GlobalContext.EffectTargetUnit.Boosts.PostMovementModifier.Spirit != _Spirit;
            bool CanActivateMove = Params.GlobalContext.EffectTargetUnit.Boosts.PostMovementModifier.Move != _Move;

            return CanActivateAttack || CanActivateTransform || CanActivateSpirit || CanActivateMove;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.PostMovementModifier.Attack = _Attack;
            Params.LocalContext.EffectTargetUnit.Boosts.PostMovementModifier.Transform = _Transform;
            Params.LocalContext.EffectTargetUnit.Boosts.PostMovementModifier.Spirit = _Spirit;
            Params.LocalContext.EffectTargetUnit.Boosts.PostMovementModifier.Move = _Move;

            return "Attack: " + (_Attack ? "Yes" : "No") + " Transform: " + (_Transform ? "Yes" : "No") + " Spirit: " + (_Spirit ? "Yes" : "No") + " Move: " + (_Move ? "Yes" : "No");
        }

        protected override BaseEffect DoCopy()
        {
            PostMovementEffect NewEffect = new PostMovementEffect(Params);

            NewEffect._Attack = _Attack;
            NewEffect._Transform = _Transform;
            NewEffect._Spirit = _Spirit;
            NewEffect._Move = _Move;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            PostMovementEffect NewEffect = (PostMovementEffect)Copy;

            _Attack = NewEffect._Attack;
            _Transform = NewEffect._Transform;
            _Spirit = NewEffect._Spirit;
            _Move = NewEffect._Move;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool Attack
        {
            get { return _Attack; }
            set { _Attack = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool Transform
        {
            get { return _Transform; }
            set { _Transform = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool Spirit
        {
            get { return _Spirit; }
            set { _Spirit = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool Move
        {
            get { return _Move; }
            set { _Move = value; }
        }

        #endregion
    }
}
