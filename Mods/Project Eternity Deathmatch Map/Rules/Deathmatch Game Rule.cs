using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchGameInfo : GameModeInfo
    {
        public const string ModeName = "Deathmatch";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public DeathmatchGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Gain points for kills and assists, respawn on death.", CategoryPVP, IsUnlocked, sprPreview)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new DeathmatchGameRule((DeathmatchMap)Map, this);
        }

        public override GameModeInfo Copy()
        {
            return new DeathmatchGameInfo(IsUnlocked, sprPreview);
        }

        [DisplayNameAttribute("Maximum Resapwn"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("How many points are allowed to respawn."),
        DefaultValueAttribute(3)]
        public int MaximumResapwn
        {
            get
            {
                return _MaximumResapwn;
            }
            set
            {
                _MaximumResapwn = value;
            }
        }
    }

    class DeathmatchGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;
        private readonly DeathmatchGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public DeathmatchGameRule(DeathmatchMap Owner, DeathmatchGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
        }
    }
}
