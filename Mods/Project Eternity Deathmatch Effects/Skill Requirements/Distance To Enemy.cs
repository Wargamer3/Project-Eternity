using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using System;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class DistanceToEnemyRequirement : DeathmatchSkillRequirement
    {
        private int _DistanceToEnemy;
        private bool _IncludeDiagonals;

        public DistanceToEnemyRequirement()
            : this(null)
        {
        }

        public DistanceToEnemyRequirement(DeathmatchContext Context)
            : base("Distance To Enemy Requirement", Context)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override bool CanActivatePassive()
        {
            foreach (Player ActivePlayer in Context.Map.ListPlayer)
            {
                if (ActivePlayer.Team != Context.Map.ListPlayer[Context.Map.ActivePlayerIndex].Team)
                {
                    continue;
                }

                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    float X1 = Context.EffectOwnerSquad.X;
                    float Y1 = Context.EffectOwnerSquad.Y;
                    float X2 = ActiveSquad.X;
                    float Y2 = ActiveSquad.Y;

                    int Distance = 0;

                    if (IncludeDiagonals)
                    {
                        Distance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2)));
                    }
                    else
                    {
                        Distance = (int)((X2 - X1) + (Y2 - Y1));
                    }

                    if (Distance < DistanceToEnemy)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public override BaseSkillRequirement Copy()
        {
            DistanceToEnemyRequirement NewSkillEffect = new DistanceToEnemyRequirement(Context);

            NewSkillEffect._DistanceToEnemy = _DistanceToEnemy;
            NewSkillEffect._IncludeDiagonals = _IncludeDiagonals;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            DistanceToEnemyRequirement NewRequirement = (DistanceToEnemyRequirement)Copy;

            _DistanceToEnemy = NewRequirement._DistanceToEnemy;
            _IncludeDiagonals = NewRequirement._IncludeDiagonals;
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute("")]
        public int DistanceToEnemy
        {
            get { return _DistanceToEnemy; }
            set { _DistanceToEnemy = value; }
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute("")]
        public bool IncludeDiagonals
        {
            get { return _IncludeDiagonals; }
            set { _IncludeDiagonals = value; }
        }
    }
}
