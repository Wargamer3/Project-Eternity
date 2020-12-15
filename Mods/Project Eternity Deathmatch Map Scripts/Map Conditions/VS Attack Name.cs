using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VSAttackNameCondition : DeathmatchCondition
    {
        private string _AttackName;

        public VSAttackNameCondition()
            : base(140, 70, "VS Attack Name", new string[] { "Check Condition" }, new string[] { "Attack found", "Attacker found", "Defender found", "Nothing found" })
        {
            _AttackName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(AttackName);
        }

        public override void Load(BinaryReader BR)
        {
            AttackName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            bool VSAttackNameFound = false;
            if (Map.ActiveSquad != null && Map.ActiveSquad.CurrentLeader.CurrentAttack.FullName == AttackName)
            {
                Map.ExecuteFollowingScripts(this, 0);
                Map.ExecuteFollowingScripts(this, 1);

                VSAttackNameFound = true;
            }
            else if (Map.TargetSquad != null && Map.TargetSquad.CurrentLeader.BattleDefenseChoice == Core.Units.Unit.BattleDefenseChoices.Attack &&
                    Map.TargetSquad.CurrentLeader.CurrentAttack.FullName == AttackName)
            {
                if (!VSAttackNameFound)
                    Map.ExecuteFollowingScripts(this, 0);

                Map.ExecuteFollowingScripts(this, 2);

                VSAttackNameFound = true;
            }
            else if (!VSAttackNameFound)
                Map.ExecuteFollowingScripts(this, 3);
        }

        public override MapScript CopyScript()
        {
            return new VSAttackNameCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
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

        #endregion
    }
}
