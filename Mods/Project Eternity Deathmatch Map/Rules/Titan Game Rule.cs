using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class TitanGameInfo : GameModeInfo
    {
        public const string ModeName = "Titan";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public TitanGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Both teams have a flying base protected by a shield. Capture missile silos to bring the shield down. Destroy the core to win.", CategoryPVP, IsUnlocked, sprPreview)
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
            return new TitanGameRule((DeathmatchMap)Map, this);
        }

        public override GameModeInfo Copy()
        {
            return new TitanGameInfo(IsUnlocked, sprPreview);
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

    public class TitanGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;
        private readonly TitanGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public TitanGameRule(DeathmatchMap Owner, TitanGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
        }

        public override void Init()
        {
            base.Init();
        }

        public void GetPoint(int ActiveTeam)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }
    }
}
