using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class CampaignGameInfo : GameModeInfo
    {
        public const string ModeName = "Campaign";

        private int _ResapwnLimit;
        private int _UnitValueLimit;

        private int _HPRegenPerTurnFixed;
        private int _ENRegenPerTurnFixed;
        private int _SPRegenPerTurnFixed;
        private int _AmmoRegenPerTurnFixed;
        private float _HPRegenPerTurnPercent;
        private float _ENRegenPerTurnPercent;
        private float _SPRegenPerTurnPercent;
        private float _AmmoRegenPerTurnPercent;

        public CampaignGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Classic mission based mode, no respawn.", CategoryPVE, IsUnlocked, sprPreview)
        {
            _ResapwnLimit = 3000;
            _UnitValueLimit = 1000;

            _HPRegenPerTurnFixed = 0;
            _ENRegenPerTurnFixed = 5;
            _SPRegenPerTurnFixed = 10;
            _AmmoRegenPerTurnFixed = 0;

            _HPRegenPerTurnPercent = 0;
            _ENRegenPerTurnPercent = 0;
            _SPRegenPerTurnPercent = 0;
            _AmmoRegenPerTurnPercent = 0;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_ResapwnLimit);
            BW.Write(_UnitValueLimit);

            BW.Write(_HPRegenPerTurnFixed);
            BW.Write(_ENRegenPerTurnFixed);
            BW.Write(_SPRegenPerTurnFixed);
            BW.Write(_AmmoRegenPerTurnFixed);

            BW.Write(_HPRegenPerTurnPercent);
            BW.Write(_ENRegenPerTurnPercent);
            BW.Write(_SPRegenPerTurnPercent);
            BW.Write(_AmmoRegenPerTurnPercent);
        }

        public override void Load(BinaryReader BR)
        {
            _ResapwnLimit = BR.ReadInt32();
            _UnitValueLimit = BR.ReadInt32();

            _HPRegenPerTurnFixed = BR.ReadInt32();
            _ENRegenPerTurnFixed = BR.ReadInt32();
            _SPRegenPerTurnFixed = BR.ReadInt32();
            _AmmoRegenPerTurnFixed = BR.ReadInt32();

            _HPRegenPerTurnPercent = BR.ReadSingle();
            _ENRegenPerTurnPercent = BR.ReadSingle();
            _SPRegenPerTurnPercent = BR.ReadSingle();
            _AmmoRegenPerTurnPercent = BR.ReadSingle();
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

        #region Properties

        [DisplayNameAttribute("Resapwn Limit"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("How many points are allowed to respawn."),
        EditableInGame(false)]
        public int ResapwnLimit
        {
            get
            {
                return _ResapwnLimit;
            }
            set
            {
                _ResapwnLimit = value;
            }
        }

        [DisplayNameAttribute("Unit Value Limit"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Maximum value of a unit allowed to spawn in the game."),
        EditableInGame(false)]
        public int UnitValueLimit
        {
            get
            {
                return _UnitValueLimit;
            }
            set
            {
                _UnitValueLimit = value;
            }
        }

        [DisplayNameAttribute("HP Regen Per Turn Fixed"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Fixed quantity of HP regeneated every turn."),
        DefaultValueAttribute(0),
        EditableInGame(false)]
        public int HPRegenPerTurnFixed
        {
            get
            {
                return _HPRegenPerTurnFixed;
            }
            set
            {
                _HPRegenPerTurnFixed = value;
            }
        }

        [DisplayNameAttribute("EN Regen Per Turn Fixed"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Fixed quantity of EN regeneated every turn."),
        DefaultValueAttribute(5),
        EditableInGame(false)]
        public int ENRegenPerTurnFixed
        {
            get
            {
                return _ENRegenPerTurnFixed;
            }
            set
            {
                _ENRegenPerTurnFixed = value;
            }
        }

        [DisplayNameAttribute("SP Regen Per Turn Fixed"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Fixed quantity of SP regeneated every turn."),
        DefaultValueAttribute(10),
        EditableInGame(false)]
        public int SPRegenPerTurnFixed
        {
            get
            {
                return _SPRegenPerTurnFixed;
            }
            set
            {
                _SPRegenPerTurnFixed = value;
            }
        }

        [DisplayNameAttribute("Ammo Regen Per Turn Fixed"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Fixed quantity of ammunition regeneated every turn."),
        DefaultValueAttribute(0),
        EditableInGame(false)]
        public int AmmoRegenPerTurnFixed
        {
            get
            {
                return _AmmoRegenPerTurnFixed;
            }
            set
            {
                _AmmoRegenPerTurnFixed = value;
            }
        }

        [DisplayNameAttribute("HP Regen Per Turn Percent"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Percentage quantity of HP regeneated every turn."),
        DefaultValueAttribute(0f),
        EditableInGame(false)]
        public float HPRegenPerTurnPercent
        {
            get
            {
                return _HPRegenPerTurnPercent;
            }
            set
            {
                _HPRegenPerTurnPercent = value;
            }
        }

        [DisplayNameAttribute("EN Regen Per Turn Percent"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Percentage quantity of EN regeneated every turn."),
        DefaultValueAttribute(0f),
        EditableInGame(false)]
        public float ENRegenPerTurnPercent
        {
            get
            {
                return _ENRegenPerTurnPercent;
            }
            set
            {
                _ENRegenPerTurnPercent = value;
            }
        }

        [DisplayNameAttribute("SP Regen Per Turn Percent"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Percentage quantity of SP regeneated every turn."),
        DefaultValueAttribute(0f),
        EditableInGame(false)]
        public float SPRegenPerTurnPercent
        {
            get
            {
                return _SPRegenPerTurnPercent;
            }
            set
            {
                _SPRegenPerTurnPercent = value;
            }
        }

        [DisplayNameAttribute("Ammo Regen Per Turn Percent"),
        CategoryAttribute("Regen"),
        DescriptionAttribute("Percentage quantity of ammunition regeneated every turn."),
        DefaultValueAttribute(0f),
        EditableInGame(false)]
        public float AmmoRegenPerTurnPercent
        {
            get
            {
                return _AmmoRegenPerTurnPercent;
            }
            set
            {
                _AmmoRegenPerTurnPercent = value;
            }
        }

        #endregion
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

            CheckForGameOver = false;
            UseTeamsForSpawns = false;

            HPRegenPerTurnFixed = GameInfo.HPRegenPerTurnFixed;
            ENRegenPerTurnFixed = GameInfo.ENRegenPerTurnFixed;
            SPRegenPerTurnFixed = GameInfo.SPRegenPerTurnFixed;
            AmmoRegenPerTurnFixed = GameInfo.AmmoRegenPerTurnFixed;

            HPRegenPerTurnPercent = GameInfo.HPRegenPerTurnPercent;
            ENRegenPerTurnPercent = GameInfo.ENRegenPerTurnPercent;
            SPRegenPerTurnPercent = GameInfo.SPRegenPerTurnPercent;
            AmmoRegenPerTurnPercent = GameInfo.AmmoRegenPerTurnPercent;
        }

        public override void Init()
        {
            base.Init();

            for (int P = 20; P >= 0; --P)
            {
                ListRemainingResapwn.Add(GameInfo.ResapwnLimit);
            }
        }

        protected override List<MovementAlgorithmTile> GetSpawnLocations(int ActivePlayerIndex)
        {
            if (ActivePlayerIndex >= 10)
            {
                return Owner.GetCampaignEnemySpawnLocations();
            }
            else
            {
                return Owner.GetMultiplayerSpawnLocations(Owner.ListPlayer[ActivePlayerIndex].Team);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (Owner.ListPlayer.Count > 10)
            {
                GameScreen.DrawBox(g, new Vector2(3, 3), 300, 24, Color.FromNonPremultiplied(0, 0, 0, 40));
                g.DrawString(Owner.fntArial8, "Campaign", new Vector2(28, 7), Color.White);
                g.DrawString(Owner.fntArial8, ListRemainingResapwn[0].ToString(), new Vector2(134, 9), Color.White);
                g.DrawString(Owner.fntArial8, ListRemainingResapwn[10].ToString(), new Vector2(334, 9), Color.White);
            }
            else
            {
                GameScreen.DrawBox(g, new Vector2(3, 3), 200, 24, Color.FromNonPremultiplied(0, 0, 0, 40));
                g.DrawString(Owner.fntArial8, "Campaign", new Vector2(28, 7), Color.White);
                g.DrawString(Owner.fntArial8, ListRemainingResapwn[0].ToString(), new Vector2(134, 9), Color.White);
            }

            if (ShowRoomSummary)
            {
                ShowSummary(g);
            }
        }

        public override List<GameRuleError> Validate(RoomInformations Room)
        {
            List<GameRuleError> ListGameRuleError = new List<GameRuleError>();

            foreach (Player ActivePlayer in Room.GetLocalPlayers())
            {
                foreach (Squad ActiveSquad in ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad)
                {
                    for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
                    {
                        Unit ActiveUnit = ActiveSquad.At(U);
                        if (ActiveUnit.SpawnCost > GameInfo.UnitValueLimit)
                        {
                            ListGameRuleError.Add(new GameRuleError("Unit Value is too high", ActiveUnit));
                        }
                    }
                }
            }

            return ListGameRuleError;
        }
    }
}
