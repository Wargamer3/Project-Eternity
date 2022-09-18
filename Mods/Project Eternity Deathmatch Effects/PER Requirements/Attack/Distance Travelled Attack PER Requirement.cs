using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class DistanceTravelledPERAttackRequirement : AttackPERRequirement
    {
        private float _DistanceToTravel;
        private double DistanceTravelled;
        private double LastDistanceTravelled;

        public DistanceTravelledPERAttackRequirement()
            : this(null)
        {
        }

        public DistanceTravelledPERAttackRequirement(AttackPERParams Params)
            : base(OnDistanceTravelled, Params)
        {
            DistanceTravelled = 0;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_DistanceToTravel);
        }

        protected override void Load(BinaryReader BR)
        {
            _DistanceToTravel = BR.ReadSingle();
        }

        public override bool CanActicateManually(string ManualActivationName)
        {
            return false;
        }

        public override bool CanActivatePassive()
        {
            bool HasReachedDistance = false;

            DistanceTravelled += Params.GlobalContext.OwnerProjectile.DistanceTravelled - LastDistanceTravelled;

            if (DistanceTravelled >= _DistanceToTravel / 32)
            {
                DistanceTravelled -= _DistanceToTravel / 32;
                HasReachedDistance = true;
            }

            LastDistanceTravelled = Params.GlobalContext.OwnerProjectile.DistanceTravelled;

            return HasReachedDistance;
        }

        public override BaseSkillRequirement Copy()
        {
            DistanceTravelledPERAttackRequirement NewRequirement = new DistanceTravelledPERAttackRequirement(Params);

            NewRequirement._DistanceToTravel = _DistanceToTravel;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            DistanceTravelledPERAttackRequirement NewRequirement = (DistanceTravelledPERAttackRequirement)Copy;

            _DistanceToTravel = NewRequirement._DistanceToTravel;
        }

        #region Properties

        [CategoryAttribute("Requirement"),
        DescriptionAttribute("The Distance Travelled."),
        DefaultValueAttribute(0)]
        public float DistanceToTravel
        {
            get
            {
                return _DistanceToTravel;
            }
            set
            {
                _DistanceToTravel = value;
            }
        }

        #endregion
    }
}
