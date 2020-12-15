using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VSSquadIDCondition : DeathmatchCondition
    {
        private uint _SquadID;

        public VSSquadIDCondition()
            : base(140, 70, "VS Squad ID", new string[] { "Check Condition" }, new string[] { "Squad found", "Attacker found", "Defender found", "Nothing found" })
        {
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(SquadID);
        }

        public override void Load(BinaryReader BR)
        {
            SquadID = BR.ReadUInt32();
        }

        public override void Update(int Index)
        {
            bool VSSquadIDFound = false;
            if (Map.ActiveSquad != null && Map.ActiveSquad.ID == SquadID)
            {
                Map.ExecuteFollowingScripts(this, 0);
                Map.ExecuteFollowingScripts(this, 1);

                VSSquadIDFound = true;
            }
            else if (Map.TargetSquad != null && Map.TargetSquad.ID == SquadID)
            {
                if (!VSSquadIDFound)
                    Map.ExecuteFollowingScripts(this, 0);

                Map.ExecuteFollowingScripts(this, 1);

                VSSquadIDFound = true;
            }
            else if (!VSSquadIDFound)
                Map.ExecuteFollowingScripts(this, 3);
        }

        public override MapScript CopyScript()
        {
            return new VSSquadIDCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(0)]
        public UInt32 SquadID
        {
            get
            {
                return _SquadID;
            }
            set
            {
                _SquadID = value;
            }
        }

        #endregion
    }
}
