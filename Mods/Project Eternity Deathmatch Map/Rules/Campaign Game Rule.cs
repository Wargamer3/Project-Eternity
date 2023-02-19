using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
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
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new CampaignGameRule((DeathmatchMap)Map, this);
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
    }
}
