using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class WeaponPickedUpMapEvent : BattleEvent
    {
        private readonly BattleMap Map;

        private string _WeaponName;

        public WeaponPickedUpMapEvent()
            : this(null)
        {
        }

        public WeaponPickedUpMapEvent(BattleMap Map)
            : base(BattleMap.WeaponPickedUpMap, new string[] { "Weapon Picked Up" })
        {
            this.Map = Map;

            _WeaponName = string.Empty;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_WeaponName);
        }

        public override void Load(BinaryReader BR)
        {
            _WeaponName = BR.ReadString();
        }

        public override bool IsValid()
        {
            foreach (string AttackPickedUp in Map.Params.GlobalContext.ListAttackPickedUp)
            {
                if (AttackPickedUp == _WeaponName)
                {
                    return true;
                }
            }

            return false;
        }

        public override MapScript CopyScript()
        {
            return new WeaponPickedUpMapEvent(Map);
        }

        #region Properties

        [Editor(typeof(AttackSelector), typeof(UITypeEditor)),
        CategoryAttribute("Event Attributes"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string WeaponName
        {
            get
            {
                return _WeaponName;
            }
            set
            {
                _WeaponName = value;
            }
        }

        #endregion
    }
}
