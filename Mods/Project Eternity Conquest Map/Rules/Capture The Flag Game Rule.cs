using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class CaptureTheFlagGameInfo : GameModeInfo
    {
        public const string ModeName = "Capture The Flag";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public CaptureTheFlagGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Capture a flag in the enemy base and bring it back to your own flag to score a point.", CategoryPVP, IsUnlocked, sprPreview)
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
            return new CaptureTheFlagGameRule((ConquestMap)Map, this);
        }

        public override GameModeInfo Copy()
        {
            return new CaptureTheFlagGameInfo(IsUnlocked, sprPreview);
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

    public class CaptureTheFlagGameRule : LobbyMPGameRule
    {
        private readonly ConquestMap Owner;
        private readonly CaptureTheFlagGameInfo GameInfo;
        public Dictionary<int, int> DicPointsByTeam;

        public override string Name => GameInfo.Name;

        public CaptureTheFlagGameRule(ConquestMap Owner, CaptureTheFlagGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;

            DicPointsByTeam = new Dictionary<int, int>();
        }

        public override void Init()
        {
            base.Init();

            DicPointsByTeam.Add(0, 0);
            DicPointsByTeam.Add(1, 0);

            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (!DicPointsByTeam.ContainsKey(ActivePlayer.TeamIndex))
                {
                    DicPointsByTeam.Add(ActivePlayer.TeamIndex, 0);
                }
            }
        }

        protected override void InitBot(UnitConquest NewSquad)
        {
            NewSquad.SquadAI = new ConquestScripAIContainer(new ConquestAIInfo(Owner, NewSquad));
            NewSquad.SquadAI.Load("Multiplayer/Capture The Flag/Easy");
        }

        public void GetPoint(int ActiveTeam)
        {
            ++DicPointsByTeam[ActiveTeam];
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            g.DrawString(Owner.fntArial8, DicPointsByTeam[0].ToString(), new Vector2(Constants.Width / 2 - 50, 12), Color.White);
            g.DrawString(Owner.fntArial8, DicPointsByTeam[1].ToString(), new Vector2(Constants.Width / 2 + 50, 12), Color.White);
        }

        public override List<GameRuleError> Validate(RoomInformations Room)
        {
            return new List<GameRuleError>();
        }
    }
}
