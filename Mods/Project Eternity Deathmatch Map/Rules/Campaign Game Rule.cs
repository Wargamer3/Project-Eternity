using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class CampaignGameInfo : GameModeInfo
    {
        public const string ModeName = "Campaign";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public CampaignGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Classic mission based mode, no respawn.", CategoryPVE, IsUnlocked, sprPreview)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
            _MaximumResapwn = 1000;
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new CampaignGameRule((DeathmatchMap)Map, this);
        }

        public override void OnBotChanged(RoomInformations Room)
        {
            Room.MaxNumberOfBots = 0;
        }

        public override GameModeInfo Copy()
        {
            return new CampaignGameInfo(IsUnlocked, sprPreview);
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

    class CampaignGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;
        private CampaignGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public CampaignGameRule(DeathmatchMap Owner, CampaignGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
            UseTeamsForSpawns = false;
        }

        public override void Init()
        {
            base.Init();

            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                ListRemainingResapwn.Add(GameInfo.MaximumResapwn);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(3, 3), 200, 24, Color.FromNonPremultiplied(0, 0, 0, 40));
            g.DrawString(Owner.fntArial8, "Campaign", new Vector2(28, 7), Color.White);
            g.DrawString(Owner.fntArial8, ListRemainingResapwn[0].ToString(), new Vector2(134, 9), Color.White);

            if (ShowRoomSummary)
            {
                ShowSummary(g);
            }
        }
    }
}
