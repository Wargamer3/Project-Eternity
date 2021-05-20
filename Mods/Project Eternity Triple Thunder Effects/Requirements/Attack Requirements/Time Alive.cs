using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class TimeAliveRequirement : TripleThunderAttackRequirement
    {
        public static string Name = "Time Alive";

        private double _TimeToWait;
        private int _MaxExecutions;
        private double _Delay;
        private double CreateTime;
        private bool IsInit;

        public TimeAliveRequirement()
            : this(null)
        {
        }

        public TimeAliveRequirement(TripleThunderAttackContext GlobalContext)
            : base(Name, GlobalContext)
        {
            IsInit = false;
            _TimeToWait = 0d;
            _MaxExecutions = 1;
            _Delay = 0d;
        }
        
        public override bool CanActivatePassive()
        {
            if (!IsInit)
            {
                IsInit = true;
                CreateTime = GlobalContext.OwnerProjectile.TimeAlive;
            }

            if (_MaxExecutions == -1 || _MaxExecutions > 0)
            {
                bool CanActivate = GlobalContext.OwnerProjectile.TimeAlive - CreateTime >= _TimeToWait;
                if (CanActivate)
                {
                    CreateTime = GlobalContext.OwnerProjectile.TimeAlive + _Delay;
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
            BW.Write(_TimeToWait);
            BW.Write(_MaxExecutions);
            BW.Write(_Delay);
        }

        protected override void Load(BinaryReader BR)
        {
            _TimeToWait = BR.ReadDouble();
            _MaxExecutions = BR.ReadInt32();
            _Delay = BR.ReadDouble();
        }
        
        public override BaseSkillRequirement Copy()
        {
            TimeAliveRequirement NewSkillEffect = new TimeAliveRequirement(GlobalContext);

            NewSkillEffect._TimeToWait = _TimeToWait;
            NewSkillEffect._MaxExecutions = _MaxExecutions;
            NewSkillEffect._Delay = _Delay;

            return NewSkillEffect;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Time to wait before activating skill in seconds.")]
        public double TimeToWait
        {
            get { return _TimeToWait; }
            set { _TimeToWait = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Maximum number of times this requirement can work. Use -1 for infinite.")]
        public int MaxExecutions
        {
            get { return _MaxExecutions; }
            set { _MaxExecutions = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Time to wait before activating a following execution in seconds.")]
        public double Delay
        {
            get { return _Delay; }
            set { _Delay = value; }
        }

        #endregion
    }
}
