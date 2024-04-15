using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

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

        public DistanceToEnemyRequirement(DeathmatchParams Params)
            : base("Distance To Enemy Requirement", Params)
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
            foreach (Player ActivePlayer in Params.Map.ListPlayer)
            {
                if (ActivePlayer.TeamIndex != Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                {
                    continue;
                }

                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    float X1 = Params.GlobalContext.EffectOwnerSquad.X;
                    float Y1 = Params.GlobalContext.EffectOwnerSquad.Y;
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
            DistanceToEnemyRequirement NewSkillEffect = new DistanceToEnemyRequirement(Params);

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
