using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class RelationshipRequirement : DeathmatchSkillRequirement
    {
        private int _DistanceToEnemy;
        private bool _IncludeDiagonals;
        private string _CharacterName;

        public RelationshipRequirement()
            : this(null)
        {
        }

        public RelationshipRequirement(DeathmatchParams Params)
            : base("Relationship Requirement", Params)
        {
            _DistanceToEnemy = 0;
            _IncludeDiagonals = false;
            _CharacterName = "";
        }

        protected override void Load(BinaryReader BR)
        {
            _DistanceToEnemy = BR.ReadInt32();
            _IncludeDiagonals = BR.ReadBoolean();
            _CharacterName = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_DistanceToEnemy);
            BW.Write(_IncludeDiagonals);
            BW.Write(_CharacterName);
        }

        public override bool CanActivatePassive()
        {
            if (Params.GlobalContext.EffectOwnerCharacter != Params.GlobalContext.EffectOwnerUnit.Pilot)
                return false;

            foreach (Player ActivePlayer in Params.Map.ListPlayer)
            {
                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    float X1 = Params.GlobalContext.EffectOwnerSquad.X;
                    float Y1 = Params.GlobalContext.EffectOwnerSquad.Y;
                    float X2 = ActiveSquad.X;
                    float Y2 = ActiveSquad.Y;

                    for(int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
                    {
                        Unit ActiveUnit = ActiveSquad.At(U);

                        foreach(Character ActiveCharacter in ActiveUnit.ArrayCharacterActive)
                        {
                            if (ActiveCharacter.Name == _CharacterName)
                            {
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
                    }
                }
            }
            return false;
        }
        
        public override BaseSkillRequirement Copy()
        {
            RelationshipRequirement NewSkillEffect = new RelationshipRequirement(Params);

            NewSkillEffect._DistanceToEnemy = _DistanceToEnemy;
            NewSkillEffect._IncludeDiagonals = _IncludeDiagonals;
            NewSkillEffect._CharacterName = _CharacterName;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            RelationshipRequirement NewRequirement = (RelationshipRequirement)Copy;

            _DistanceToEnemy = NewRequirement._DistanceToEnemy;
            _IncludeDiagonals = NewRequirement._IncludeDiagonals;
            _CharacterName = NewRequirement._CharacterName;
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

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute("")]
        public string CharacterName
        {
            get { return _CharacterName; }
            set { _CharacterName = value; }
        }
    }
}
