using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GateCondition : BattleCondition
    {
        private bool _IsOpen;

        public GateCondition()
            : this(null)
        {
        }

        public GateCondition(BattleMap Map)
            : base(Map, 140, 70, "Gate", new string[] { "Open Gate and Check", "Close Gate and Check", "Toggle Gate and Check", "Check Gate" }, new string[] { "Gate is open", "Gate is closed" })
        {
            _IsOpen = false;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(IsOpen);
        }

        public override void Load(BinaryReader BR)
        {
            IsOpen = BR.ReadBoolean();
        }

        public override void Update(int Index)
        {
            switch (Index)
            {
                case 0:
                    IsOpen = true;
                    Map.ExecuteFollowingScripts(this, 0);
                    break;

                case 1:
                    IsOpen = false;
                    Map.ExecuteFollowingScripts(this, 1);
                    break;

                case 2:
                    IsOpen = !IsOpen;
                    if (IsOpen)
                        Map.ExecuteFollowingScripts(this, 0);
                    else
                        Map.ExecuteFollowingScripts(this, 1);
                    break;

                case 3:
                    if (IsOpen)
                        Map.ExecuteFollowingScripts(this, 0);
                    else
                        Map.ExecuteFollowingScripts(this, 1);
                    break;
            }
        }

        public override MapScript CopyScript()
        {
            return new GateCondition(Map);
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(false)]
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                _IsOpen = value;
            }
        }

        #endregion
    }
}
