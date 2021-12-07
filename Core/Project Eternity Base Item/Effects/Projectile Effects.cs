using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public abstract class ProjectileEffect : BaseEffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        protected readonly ProjectileParams Params;

        public ProjectileEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        public ProjectileEffect(string EffectTypeName, bool IsPassive, ProjectileParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new ProjectileParams(Params);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected internal override void ReactivateEffect()
        {
        }
    }

    public sealed class ChangeAttackSpeedEffect : ProjectileEffect
    {
        public static string Name = "Change Attack Speed";

        private float _Speed;
        private Operators.NumberTypes _NumberType;
        private Operators.SignOperators _SignOperator;

        public ChangeAttackSpeedEffect()
            : base(Name, false)
        {
            _Speed = 0;
            _NumberType = Operators.NumberTypes.Absolute;
            _SignOperator = Operators.SignOperators.Equal;
        }

        public ChangeAttackSpeedEffect(ProjectileParams Params)
            : base(Name, false, Params)
        {
            _Speed = 0;
            _NumberType = Operators.NumberTypes.Absolute;
            _SignOperator = Operators.SignOperators.Equal;
        }

        protected override void Load(BinaryReader BR)
        {
            _Speed = BR.ReadSingle();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _SignOperator = (Operators.SignOperators)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Speed);
            BW.Write((byte)_NumberType);
            BW.Write((byte)_SignOperator);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            float CurrentAngle = (float)Math.Atan2(Params.LocalContext.OwnerProjectile.Speed.Y, Params.LocalContext.OwnerProjectile.Speed.X);
            float CurrentSpeed = Params.LocalContext.OwnerProjectile.Speed.Length();

            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                switch (_SignOperator)
                {
                    case Operators.SignOperators.Equal:
                        CurrentSpeed = _Speed;
                        break;

                    case Operators.SignOperators.PlusEqual:
                        CurrentSpeed += _Speed;
                        break;

                    case Operators.SignOperators.MinusEqual:
                        CurrentSpeed -= _Speed;
                        break;

                    case Operators.SignOperators.MultiplicatedEqual:
                        CurrentSpeed *= _Speed;
                        break;

                    case Operators.SignOperators.DividedEqual:
                        CurrentSpeed /= _Speed;
                        break;

                    case Operators.SignOperators.ModuloEqual:
                        CurrentSpeed %= _Speed;
                        break;
                }
            }
            else if (_NumberType == Operators.NumberTypes.Relative)
            {
                switch (_SignOperator)
                {
                    case Operators.SignOperators.Equal:
                        CurrentSpeed = _Speed * CurrentSpeed;
                        break;

                    case Operators.SignOperators.PlusEqual:
                        CurrentSpeed += _Speed * CurrentSpeed;
                        break;

                    case Operators.SignOperators.MinusEqual:
                        CurrentSpeed -= _Speed * CurrentSpeed;
                        break;

                    case Operators.SignOperators.MultiplicatedEqual:
                        CurrentSpeed *= _Speed * CurrentSpeed;
                        break;

                    case Operators.SignOperators.DividedEqual:
                        if (CurrentSpeed != 0)
                            CurrentSpeed /= _Speed * CurrentSpeed;
                        break;

                    case Operators.SignOperators.ModuloEqual:
                        CurrentSpeed %= _Speed * CurrentSpeed;
                        break;
                }
            }

            Params.LocalContext.OwnerProjectile.Speed = new Vector2((float)Math.Cos(CurrentAngle) * CurrentSpeed, (float)Math.Sin(CurrentAngle) * CurrentSpeed);

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeAttackSpeedEffect NewEffect = new ChangeAttackSpeedEffect(Params);

            NewEffect._Speed = _Speed;
            NewEffect._NumberType = _NumberType;
            NewEffect._SignOperator = _SignOperator;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeAttackSpeedEffect NewEffect = (ChangeAttackSpeedEffect)Copy;

            _Speed = NewEffect._Speed;
            _NumberType = NewEffect._NumberType;
            _SignOperator = NewEffect._SignOperator;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Speed to add.")]
        public float Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.NumberTypes NumberType
        {
            get { return _NumberType; }
            set { _NumberType = value; }
        }

        [TypeConverter(typeof(SignOperatorConverter)),
        CategoryAttribute(""),
        DescriptionAttribute(".")]
        public Operators.SignOperators SignOperator
        {
            get { return _SignOperator; }
            set { _SignOperator = value; }
        }

        #endregion
    }

    public sealed class BounceAttackOffGroundEffect : ProjectileEffect
    {
        public static string Name = "Bounce Attack Off Ground";

        public BounceAttackOffGroundEffect()
            : base(Name, false)
        {
        }

        public BounceAttackOffGroundEffect(ProjectileParams Params)
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
            Vector2 v = Params.LocalContext.OwnerProjectile.Speed;
            Vector2 n = new Vector2((float)Math.Cos(Params.SharedParams.OwnerAngle), -(float)Math.Sin(Params.SharedParams.OwnerAngle));
            Vector2 u = (Vector2.Dot(v, n) / Vector2.Dot(n, n)) * n;
            Vector2 w = v - u;

            Params.LocalContext.OwnerProjectile.Speed = w - u;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            BounceAttackOffGroundEffect NewEffect = new BounceAttackOffGroundEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }

    public sealed class RotateAttackEffect : ProjectileEffect
    {
        public static string Name = "Rotate Attack";

        private float _Angle;

        public RotateAttackEffect()
            : base(Name, false)
        {
        }

        public RotateAttackEffect(ProjectileParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _Angle = BR.ReadSingle();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Angle);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            float CurrentAngle = (float)Math.Atan2(Params.LocalContext.OwnerProjectile.Speed.Y, Params.LocalContext.OwnerProjectile.Speed.X);
            float CurrentSpeed = Params.LocalContext.OwnerProjectile.Speed.Length();
            CurrentAngle -= MathHelper.ToRadians(_Angle);

            Params.LocalContext.OwnerProjectile.Speed = new Vector2((float)Math.Cos(CurrentAngle) * CurrentSpeed, (float)Math.Sin(CurrentAngle) * CurrentSpeed);

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            RotateAttackEffect NewEffect = new RotateAttackEffect(Params);

            NewEffect._Angle = _Angle;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            RotateAttackEffect NewEffect = (RotateAttackEffect)Copy;

            _Angle = NewEffect._Angle;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Angle in degrees.")]
        public float Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        #endregion
    }

    public sealed class MatchTerrainTiltWithAttackEffect : ProjectileEffect
    {
        public static string Name = "Match Terrain Tilt With Attack";

        public MatchTerrainTiltWithAttackEffect()
            : base(Name, false)
        {
        }

        public MatchTerrainTiltWithAttackEffect(ProjectileParams Params)
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
            Params.LocalContext.OwnerProjectile.SetAngle(Params.SharedParams.OwnerAngle);

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            MatchTerrainTiltWithAttackEffect NewEffect = new MatchTerrainTiltWithAttackEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }

    public sealed class ReviveAttackEffect : ProjectileEffect
    {
        public static string Name = "Revive Attack";

        int _MaxExecutions;

        public ReviveAttackEffect()
            : base(Name, false)
        {
            _MaxExecutions = -1;
        }

        public ReviveAttackEffect(ProjectileParams Params)
            : base(Name, false, Params)
        {
            _MaxExecutions = -1;
        }

        protected override void Load(BinaryReader BR)
        {
            _MaxExecutions = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_MaxExecutions);
        }

        public override bool CanActivate()
        {
            if (_MaxExecutions == -1 || _MaxExecutions > 0)
            {
                return true;
            }

            return false;
        }

        protected override string DoExecuteEffect()
        {
            if (_MaxExecutions > 0)
            {
                --_MaxExecutions;
            }

            Params.LocalContext.OwnerProjectile.IsAlive = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            ReviveAttackEffect NewEffect = new ReviveAttackEffect(Params);

            NewEffect._MaxExecutions = _MaxExecutions;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ReviveAttackEffect NewEffect = (ReviveAttackEffect)Copy;

            _MaxExecutions = NewEffect._MaxExecutions;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Maximum number of times this effect can work. Use -1 for infinite.")]
        public int MaxExecutions
        {
            get { return _MaxExecutions; }
            set { _MaxExecutions = value; }
        }

        #endregion
    }

    public sealed class DestroyAttackEffect : ProjectileEffect
    {
        public static string Name = "Destroy Attack";

        public DestroyAttackEffect()
            : base(Name, false)
        {
        }

        public DestroyAttackEffect(ProjectileParams Params)
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
            Params.LocalContext.OwnerProjectile.IsAlive = false;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            DestroyAttackEffect NewEffect = new DestroyAttackEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }

    public sealed class ToggleAttackGravityEffect : ProjectileEffect
    {
        public static string Name = "Toggle Attack Gravity";

        private bool _UseGravity;

        public ToggleAttackGravityEffect()
            : base(Name, false)
        {
            _UseGravity = false;
        }

        public ToggleAttackGravityEffect(ProjectileParams Params)
            : base(Name, false, Params)
        {
            _UseGravity = false;
        }

        protected override void Load(BinaryReader BR)
        {
            _UseGravity = BR.ReadBoolean();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_UseGravity);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.OwnerProjectile.ToggleGravity(_UseGravity);

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            ToggleAttackGravityEffect NewEffect = new ToggleAttackGravityEffect(Params);

            NewEffect._UseGravity = _UseGravity;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ToggleAttackGravityEffect NewEffect = (ToggleAttackGravityEffect)Copy;

            _UseGravity = NewEffect._UseGravity;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool UseGravity
        {
            get { return _UseGravity; }
            set { _UseGravity = value; }
        }

        #endregion
    }
}
