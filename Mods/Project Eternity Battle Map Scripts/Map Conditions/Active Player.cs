using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ActivePlayerCondition : BattleCondition
    {
        private int _ActivePlayer;

        public ActivePlayerCondition()
            : this(null)
        {
        }

        public ActivePlayerCondition(BattleMap Map)
            : base(Map, 140, 70, "Active Player", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _ActivePlayer = 0;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(ActivePlayer);
        }

        public override void Load(BinaryReader BR)
        {
            ActivePlayer = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            if (Map.ActivePlayerIndex == ActivePlayer)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new ActivePlayerCondition(Map);
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(0)]
        public int ActivePlayer
        {
            get
            {
                return _ActivePlayer;
            }
            set
            {
                _ActivePlayer = value;
            }
        }

        #endregion
    }
}
