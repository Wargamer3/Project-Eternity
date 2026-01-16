using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class CampaignGameInfo : GameModeInfo
    {
        #region Selector

        public class ConquestFactionSelector : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathFactionsConquest);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(26);
                    }
                }
                return value;
            }
        }

        #endregion

        public const string ModeName = "Campaign";

        private string _FactionPlayer1;
        private string _FactionPlayer2;
        private string _FactionPlayer3;
        private string _FactionPlayer4;
        private string _BehaviorPlayer1;
        private string _BehaviorPlayer2;
        private string _BehaviorPlayer3;
        private string _BehaviorPlayer4;

        public CampaignGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Classic mission based mode, no respawn.", CategoryPVE, IsUnlocked, sprPreview)
        {
            _FactionPlayer1 = "Red";
            _FactionPlayer2 = "Blue";
            _FactionPlayer3 = string.Empty;
            _FactionPlayer4 = string.Empty;

            _BehaviorPlayer1 = "Human";
            _BehaviorPlayer2 = "Default AI";
            _BehaviorPlayer3 = string.Empty;
            _BehaviorPlayer4 = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_FactionPlayer1);
            BW.Write(_FactionPlayer2);
            BW.Write(_FactionPlayer3);
            BW.Write(_FactionPlayer4);

            BW.Write(_BehaviorPlayer1);
            BW.Write(_BehaviorPlayer2);
            BW.Write(_BehaviorPlayer3);
            BW.Write(_BehaviorPlayer4);
        }

        public override void Load(BinaryReader BR)
        {
            _FactionPlayer1 = BR.ReadString();
            _FactionPlayer2 = BR.ReadString();
            _FactionPlayer3 = BR.ReadString();
            _FactionPlayer4 = BR.ReadString();

            _BehaviorPlayer1 = BR.ReadString();
            _BehaviorPlayer2 = BR.ReadString();
            _BehaviorPlayer3 = BR.ReadString();
            _BehaviorPlayer4 = BR.ReadString();
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new CampaignGameRule((ConquestMap)Map, this);
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

        [Editor(typeof(ConquestFactionSelector), typeof(UITypeEditor)),
        DisplayNameAttribute("Player 1 Faction"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string FactionPlayer1
        {
            get
            {
                return _FactionPlayer1;
            }
            set
            {
                _FactionPlayer1 = value;
            }
        }

        [Editor(typeof(ConquestFactionSelector), typeof(UITypeEditor)),
        DisplayNameAttribute("Player 2 Faction"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string FactionPlayer2
        {
            get
            {
                return _FactionPlayer2;
            }
            set
            {
                _FactionPlayer2 = value;
            }
        }

        [Editor(typeof(ConquestFactionSelector), typeof(UITypeEditor)),
        DisplayNameAttribute("Player 3 Faction"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string FactionPlayer3
        {
            get
            {
                return _FactionPlayer3;
            }
            set
            {
                _FactionPlayer3 = value;
            }
        }

        [Editor(typeof(ConquestFactionSelector), typeof(UITypeEditor)),
        DisplayNameAttribute("Player 4 Faction"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string FactionPlayer4
        {
            get
            {
                return _FactionPlayer4;
            }
            set
            {
                _FactionPlayer4 = value;
            }
        }

        [DisplayNameAttribute("Player 1 Behavior"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string BehaviorPlayer1
        {
            get
            {
                return _BehaviorPlayer1;
            }
            set
            {
                _BehaviorPlayer1 = value;
            }
        }

        [DisplayNameAttribute("Player 2 Behavior"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string BehaviorPlayer2
        {
            get
            {
                return _BehaviorPlayer2;
            }
            set
            {
                _BehaviorPlayer2 = value;
            }
        }

        [DisplayNameAttribute("Player 3 Behavior"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string BehaviorPlayer3
        {
            get
            {
                return _BehaviorPlayer3;
            }
            set
            {
                _BehaviorPlayer3 = value;
            }
        }

        [DisplayNameAttribute("Player 4 Behavior"),
        CategoryAttribute("Faction Attributes"),
        DescriptionAttribute(".")]
        public string BehaviorPlayer4
        {
            get
            {
                return _BehaviorPlayer4;
            }
            set
            {
                _BehaviorPlayer4 = value;
            }
        }

        #endregion
    }

    class CampaignGameRule : LobbyMPGameRule
    {
        private readonly ConquestMap Owner;
        private CampaignGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public CampaignGameRule(ConquestMap Owner, CampaignGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;

            CheckForGameOver = false;
            UseTeamsForSpawns = false;
        }

        protected override List<Vector3> GetSpawnLocations(int ActivePlayerIndex)
        {
            if (ActivePlayerIndex >= 10)
            {
                return Owner.GetCampaignEnemySpawnLocations();
            }
            else
            {
                return Owner.GetMultiplayerSpawnLocations(Owner.ListPlayer[ActivePlayerIndex].TeamIndex);
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

            return ListGameRuleError;
        }
    }
}
