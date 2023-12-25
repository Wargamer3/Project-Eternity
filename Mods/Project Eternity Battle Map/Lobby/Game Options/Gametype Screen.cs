using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GameOptionsGametypeScreen : GameScreen
    {
        protected struct GametypeCategory
        {
            public string Category;
            public GameModeInfo[] ArrayGametype;

            public GametypeCategory(string Category, GameModeInfo[] ArrayGametype)
            {
                this.Category = Category;
                this.ArrayGametype = ArrayGametype;
            }
        }

        private SpriteFont fntText;
        private EmptyBoxScrollbar GametypeScrollbar;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen Owner;

        int PanelY = (int)(Constants.Height * 0.15);
        int PanelWidth = (int)(Constants.Width * 0.4);
        int PanelHeight = (int)(Constants.Height * 0.75);

        int LeftPanelX = (int)(Constants.Width * 0.03);

        protected GametypeCategory[] ArrayGametypeCategory;
        private int GametypeScrollbarValue;
        protected GameModeInfo SelectedGametype;

        public GameOptionsGametypeScreen(RoomInformations Room, GameOptionsScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            LoadGameTypes();

            int PanelY = (int)(Constants.Height * 0.15);
            int PanelWidth = (int)(Constants.Width * 0.4);
            int PanelHeight = (int)(Constants.Height * 0.75);

            int LeftPanelX = (int)(Constants.Width * 0.03);

            GametypeScrollbar = new EmptyBoxScrollbar(new Vector2(LeftPanelX + PanelWidth - 20, PanelY), PanelHeight, 10, OnGametypeScrollbarChange);
        }

        protected virtual void LoadGameTypes()
        {
            foreach (GameModeInfo ActiveGameModeInfo in BattleMap.DicBattmeMapType[Room.MapModName].GetAvailableGameModes().Values)
            {

            }

            GameModeInfo GametypeCampaign = new GameModeInfo("Campaign", "Classic mission based mode, no respawn.", GameModeInfo.CategoryPVE, true, null);
            GameModeInfo GametypeArcade = new GameModeInfo("Arcade", "Campaign mission without cutscenes.", GameModeInfo.CategoryPVE, PlayerManager.ListLocalPlayer[0].Records.ListCampaignLevelInformation.Count > 0, null);
            GameModeInfo GametypeHorde = new GameModeInfo("Horde", "Wave survival mode, respawn at the start of each wave.", GameModeInfo.CategoryPVE, true, null);
            GameModeInfo GametypeBaseDefense = new GameModeInfo("Base Defense", "Wave survival mode, respawn at the start of each wave. Must defend a base by building turrets.", GameModeInfo.CategoryPVE, false, null);

            GameModeInfo GametypeDeathmatch = new GameModeInfo("Deathmatch", "Gain points for kills and assists, respawn on death.", GameModeInfo.CategoryPVP, true, null);
            GameModeInfo GametypeCaptureTheFlag = new GameModeInfo("Capture The Flag", "Capture a flag in the enemy base and bring it back to your own flag to score a point.", GameModeInfo.CategoryPVP, true, null);
            GameModeInfo GametypeObjective = new GameModeInfo("Objective", "One team must complete objectives while another prevent them.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeAssault = new GameModeInfo("Assault", "Team deathmatch with limited respawns.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeConquest = new GameModeInfo("Conquest", "Teams must fight to capture respawn bases that give them points. The starting base may or may not be capturable.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeOnslaught = new GameModeInfo("Onslaught", "Teams must fight to capture respawn bases that give them access to the enemy base's core. Last team with a core win.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeTitan = new GameModeInfo("Titan", "Both teams have a flying base protected by a shield. Capture missile silos to bring the shield down. Destroy the core to win.", GameModeInfo.CategoryPVP, true, null);
            GameModeInfo GametypeBaseAssault = new GameModeInfo("Base Assault", "Each team has 3 bases to attack and defend. After destroying the walls with artillery you can plant a bomb to completely destroy it.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeKingOfTheHill = new GameModeInfo("King Of The Hill", "Hold a position without enemies to win points.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeBunny = new GameModeInfo("Bunny", "Unit that holds the flag become the bunny and gets points for kills, everyone else try to kill the bunny.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeFreezeTag = new GameModeInfo("Freeze Tag", "Killing an enemy freeze him, when every enemies are frozen you win. Teamates can unfreeze allie by staying next to them for 2 turns.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeJailbreak = new GameModeInfo("Jailbreak", "Killing an enemy send him to your prison, capture everyone to win. Teamates can be freed by standing on a switch.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeMutant = new GameModeInfo("Mutant", "First kill transform you into the mutant, a unit with overpowered stats and attacks. Only the Mutant can kill or be killed.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeProtectThaPimp = new GameModeInfo("Protect Tha Pimp", "Try to kill the enemy Pimp before it can escape. The pimp move slower and only has a 1 HKO melee attack.", GameModeInfo.CategoryPVP, false, null);
            GameModeInfo GametypeKaiju = new GameModeInfo("Kaiju", "One player controls giant monsters while the other players use their units.", GameModeInfo.CategoryPVP, false, null);

            SelectedGametype = GametypeCampaign;
            ArrayGametypeCategory = new GametypeCategory[2];
            ArrayGametypeCategory[0] = new GametypeCategory(GameModeInfo.CategoryPVE, new GameModeInfo[] { GametypeCampaign, GametypeArcade, GametypeHorde, GametypeBaseDefense });
            ArrayGametypeCategory[1] = new GametypeCategory(GameModeInfo.CategoryPVP, new GameModeInfo[]
            {
                GametypeDeathmatch, GametypeCaptureTheFlag, GametypeObjective, GametypeAssault, GametypeConquest,
                GametypeOnslaught, GametypeTitan, GametypeBaseAssault, GametypeKingOfTheHill, GametypeBunny, GametypeFreezeTag,
                GametypeJailbreak, GametypeMutant, GametypeProtectThaPimp, GametypeKaiju,
            });
        }

        public override void Update(GameTime gameTime)
        {
            GametypeScrollbar.Update(gameTime);

            SelectGametype();
        }

        private void SelectGametype()
        {
            float DrawY = PanelY + 5;
            int CurrentIndex = 0;
            for (int G = 0; G < ArrayGametypeCategory.Length; ++G)
            {
                if (SelectGametype(ArrayGametypeCategory[G], ref CurrentIndex, ref DrawY))
                    break;
            }
        }

        private bool SelectGametype(GametypeCategory ActiveCategory, ref int CurrentIndex, ref float DrawY)
        {
            if (CurrentIndex >= GametypeScrollbarValue)
            {
                DrawY += 20;
            }

            ++CurrentIndex;

            for (int G = 0; G < ActiveCategory.ArrayGametype.Length; ++G)
            {
                if (CurrentIndex >= GametypeScrollbarValue && ActiveCategory.ArrayGametype[G].IsUnlocked)
                {
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20
                        && InputHelper.InputConfirmPressed())
                    {
                        SelectedGametype = ActiveCategory.ArrayGametype[G];
                        Room.GameMode = SelectedGametype.Name;
                        Owner.OnGametypeUpdate();
                        return true;
                    }
                }

                DrawY += 20;
                ++CurrentIndex;
            }

            return false;
        }

        private void OnGametypeScrollbarChange(float ScrollbarValue)
        {
            GametypeScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawEmptyBox(g, new Vector2(LeftPanelX, PanelY), PanelWidth, PanelHeight);

            Color NewBackgroundColor = Color.FromNonPremultiplied((int)(Lobby.BackgroundColor.R * 0.8f), (int)(Lobby.BackgroundColor.G * 0.8f), (int)(Lobby.BackgroundColor.B * 0.8f), 150);
            g.Draw(GameScreen.sprPixel, new Rectangle(LeftPanelX, PanelY, PanelWidth, PanelHeight), NewBackgroundColor);

            float DrawY = PanelY + 5;
            int CurrentIndex = 0;
            for (int G = 0; G < ArrayGametypeCategory.Length; ++G)
            {
                DrawGametypeCategory(g, ArrayGametypeCategory[G], ref CurrentIndex, ref DrawY);
            }

            int RightPanelX = Constants.Width - LeftPanelX - PanelWidth;
            int RightPanelContentOffset = (int)(PanelWidth * 0.05);
            int RightPanelContentX = RightPanelX + RightPanelContentOffset;
            int RightPanelContentWidth = PanelWidth - RightPanelContentOffset - RightPanelContentOffset;

            int PreviewBoxY = PanelY + 10;
            int PreviewBoxHeight = (int)(PanelHeight * 0.4);

            int DescriptionBoxY = PreviewBoxY + PreviewBoxHeight + 10;
            int DescriptionHeight = PanelHeight - (DescriptionBoxY - PanelY) - 10;

            int DescriptionBoxNameOffset = (int)(RightPanelContentWidth * 0.25);
            int DescriptionBoxNameX = RightPanelContentX + DescriptionBoxNameOffset;
            int DescriptionBoxNameWidth = RightPanelContentWidth - DescriptionBoxNameOffset - DescriptionBoxNameOffset;
            int DescriptionBoxNameHeight = 30;

            DrawEmptyBox(g, new Vector2(RightPanelX, PanelY), PanelWidth, PanelHeight);
            g.Draw(GameScreen.sprPixel, new Rectangle(RightPanelX, PanelY, PanelWidth, PanelHeight), NewBackgroundColor);

            DrawEmptyBox(g, new Vector2(RightPanelContentX, PreviewBoxY), RightPanelContentWidth, PreviewBoxHeight);
            DrawEmptyBox(g, new Vector2(RightPanelContentX, DescriptionBoxY), RightPanelContentWidth, DescriptionHeight);
            DrawEmptyBox(g, new Vector2(DescriptionBoxNameX, DescriptionBoxY), DescriptionBoxNameWidth, 30);

            g.DrawStringCentered(fntText, SelectedGametype.Name, new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2, DescriptionBoxY + DescriptionBoxNameHeight / 2), Color.White);

            float DescriptionY = DescriptionBoxY + DescriptionBoxNameHeight;
            foreach (string ActiveLine in TextHelper.FitToWidth(fntText, SelectedGametype.Description, RightPanelContentWidth - 5))
            {
                g.DrawString(fntText, ActiveLine, new Vector2(RightPanelContentX + 5, DescriptionY), Color.White);
                DescriptionY += 20;

            }

            GametypeScrollbar.Draw(g);
        }

        private void DrawGametypeCategory(CustomSpriteBatch g, GametypeCategory ActiveCategory, ref int CurrentIndex, ref float DrawY)
        {
            if (CurrentIndex >= GametypeScrollbarValue)
            {
                g.DrawStringMiddleAligned(fntText, ActiveCategory.Category, new Vector2(LeftPanelX + PanelWidth / 2, DrawY), Color.White);
                DrawY += 20;
            }

            ++CurrentIndex;

            for (int G = 0; G < ActiveCategory.ArrayGametype.Length; ++G)
            {
                if (CurrentIndex >= GametypeScrollbarValue)
                {
                    if (ActiveCategory.ArrayGametype[G].IsUnlocked)
                    {
                        g.DrawString(fntText, ActiveCategory.ArrayGametype[G].Name, new Vector2(LeftPanelX + 5, DrawY), Color.White);

                        if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                            && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20)
                        {
                            g.Draw(sprPixel, new Rectangle(LeftPanelX, (int)DrawY, PanelWidth, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                        }
                    }
                    else
                    {
                        g.DrawString(fntText, ActiveCategory.ArrayGametype[G].Name, new Vector2(LeftPanelX + 5, DrawY), Color.Gray);
                    }
                }

                DrawY += 20;
                ++CurrentIndex;
            }
        }

        public override string ToString()
        {
            return "Gametype";
        }
    }
}
