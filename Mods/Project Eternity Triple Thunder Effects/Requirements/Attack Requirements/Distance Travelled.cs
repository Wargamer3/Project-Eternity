using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class DistanceTravelledRequirement : TripleThunderAttackRequirement
    {
        public static string Name = "Distance Travelled";

        private double _DistanceToTravel;
        private int _MaxExecutions;
        private double _Delay;
        private double CreateDistance;
        private bool IsInit;

        public DistanceTravelledRequirement()
            : this(null)
        {
        }

        public DistanceTravelledRequirement(TripleThunderAttackContext GlobalContext)
            : base(Name, GlobalContext)
        {
            IsInit = false;
            _DistanceToTravel = 0d;
            _MaxExecutions = 1;
            _Delay = 0d;
        }

        public override bool CanActivatePassive()
        {
            if (!IsInit)
            {
                IsInit = true;
                CreateDistance = GlobalContext.OwnerProjectile.DistanceTravelled;
            }

            if (_MaxExecutions == -1 || _MaxExecutions > 0)
            {
                bool CanActivate = GlobalContext.OwnerProjectile.DistanceTravelled - CreateDistance >= _DistanceToTravel;
                if (CanActivate)
                {
                    CreateDistance = GlobalContext.OwnerProjectile.DistanceTravelled + _Delay;
                    if (_MaxExecutions > 0)
                    {
                        --_MaxExecutions;
                    }
                }

                return CanActivate;
            }

            return false;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_DistanceToTravel);
            BW.Write(_MaxExecutions);
            BW.Write(_Delay);
        }

        protected override void Load(BinaryReader BR)
        {
            _DistanceToTravel = BR.ReadDouble();
            _MaxExecutions = BR.ReadInt32();
            _Delay = BR.ReadDouble();
        }
        
        public override BaseSkillRequirement Copy()
        {
            DistanceTravelledRequirement NewSkillEffect = new DistanceTravelledRequirement(GlobalContext);

            NewSkillEffect._DistanceToTravel = _DistanceToTravel;
            NewSkillEffect._MaxExecutions = _MaxExecutions;
            NewSkillEffect._Delay = _Delay;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            DistanceTravelledRequirement NewRequirement = (DistanceTravelledRequirement)Copy;

            _DistanceToTravel = NewRequirement._DistanceToTravel;
            _MaxExecutions = NewRequirement._MaxExecutions;
            _Delay = NewRequirement._Delay;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Distance to travel before activating skill in pixel.")]
        public double DistanceToTravel
        {
            get { return _DistanceToTravel; }
            set { _DistanceToTravel = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Maximum number of times this requirement can work. Use -1 for infinite.")]
        public int MaxExecutions
        {
            get { return _MaxExecutions; }
            set { _MaxExecutions = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Distance to travel before activating a following execution in pixel.")]
        public double Delay
        {
            get { return _Delay; }
            set { _Delay = value; }
        }

        #endregion
    }
}
