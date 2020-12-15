using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class SwitchTrigger : BattleTrigger
    {
        private bool _State;

        public SwitchTrigger()
            : this(null)
        {
        }

        public SwitchTrigger(BattleMap Map)
            : base(Map, 140, 70, "Switch", new string[] { "Turn On", "Turn Off", "Toggle" }, new string[] { "Switch to On", "Switch to Off", "Switch toggled" })
        {
            State = _State;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(State);
        }

        public override void Load(BinaryReader BR)
        {
            State = BR.ReadBoolean();
        }

        public override void Update(int Index)
        {
            switch (Index)
            {
                case 0:
                    if (!State)
                    {
                        State = true;
                        Map.ExecuteFollowingScripts(this, 0);
                        Map.ExecuteFollowingScripts(this, 2);
                    }
                    break;

                case 1:
                    if (State)
                    {
                        State = false;
                        Map.ExecuteFollowingScripts(this, 1);
                        Map.ExecuteFollowingScripts(this, 2);
                    }
                    break;

                case 2:
                    State = !State;

                    if (State)
                        Map.ExecuteFollowingScripts(this, 0);
                    else
                        Map.ExecuteFollowingScripts(this, 1);

                    Map.ExecuteFollowingScripts(this, 2);
                    break;
            }
        }

        public override MapScript CopyScript()
        {
            return new SwitchTrigger(Map);
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public bool State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }

        #endregion
    }
}
