using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CounterTrigger : BattleTrigger
    {
        public int Value;
        private int _StartingValue;
        private int _MaximumValue;

        public CounterTrigger()
            : this(null)
        {
        }

        public CounterTrigger(BattleMap Map)
            : base(Map, 140, 70, "Counter", new string[] { "Increment Counter", "Reset Counter" }, new string[] { "Counter changed", "Counter reset", "Counter maxed" })
        {
            _StartingValue = 0;
            _MaximumValue = 5;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(StartingValue);
            BW.Write(MaximumValue);
        }

        public override void Load(BinaryReader BR)
        {
            StartingValue = BR.ReadInt32();
            MaximumValue = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            switch (Index)
            {//Increment.
                case 0:
                    //Counter maxed.
                    if (Value++ >= MaximumValue)
                        Map.ExecuteFollowingScripts(this, 2);
                    else
                        Map.ExecuteFollowingScripts(this, 0);
                    break;

                //Reset.
                case 1:
                    if (Value > StartingValue)
                    {
                        Value = StartingValue;
                        Map.ExecuteFollowingScripts(this, 0);
                        Map.ExecuteFollowingScripts(this, 1);
                    }
                    break;
            }
        }

        public override MapScript CopyScript()
        {
            return new CounterTrigger(Map);
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public int StartingValue
        {
            get
            {
                return _StartingValue;
            }
            set
            {
                _StartingValue = value;
            }
        }

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public int MaximumValue
        {
            get
            {
                return _MaximumValue;
            }
            set
            {
                _MaximumValue = value;
            }
        }

        #endregion
    }
}
