using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
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
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntOxanimumBold;

        private Texture2D sprHighlight;

        private Texture2D sprFrameTop;
        private Texture2D sprFrameDescription;
        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        private Scrollbar GametypeScrollbar;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen Owner;

        int PanelY;
        int PanelWidth;

        int LeftPanelX;
        int CategoryOffsetY;
        int HeaderOffsetY;
        int FirstLineOffsetY;
        int LineOffsetY;

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
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");

            sprHighlight = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Select Highlight");

            sprFrameTop = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Frame Top Large");
            sprFrameDescription = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Extra Frame");
            sprScrollbarBackground = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Scrollbar Bar");

            LoadGameTypes();

            float Ratio = Constants.Height / 2160f;
            PanelY = (int)(510 * Ratio);
            PanelWidth = (int)(sprFrameTop.Width * Ratio);

            LeftPanelX = (int)(390 * Ratio);

            CategoryOffsetY = (int)(200 * Ratio);
            HeaderOffsetY = (int)(78 * Ratio);
            FirstLineOffsetY = (int)(30 * Ratio);
            LineOffsetY = (int)(76 * Ratio);

            GametypeScrollbar = new Scrollbar(sprScrollbar, new Vector2(LeftPanelX + PanelWidth, PanelY), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnGametypeScrollbarChange);
        }

        protected virtual void LoadGameTypes()
        {
            foreach (GameModeInfo ActiveGameModeInfo in BattleMap.DicBattmeMapType[Room.MapModName].GetAvailableGameModes().Values)
            {

            }

            GameModeInfo GametypeCampaign = new GameModeInfo("Campaign", "Classic mission based mode, no respawn.", GameModeInfo.CategoryPVE, true, null);
            GameModeInfo GametypeArcade = new GameModeInfo("Arcade", "Campaign mission without cutscenes.", GameModeInfo.CategoryPVE, PlayerManager.ListLocalPlayer[0].Records.DicCampaignLevelInformation.Count > 0, null);
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
            float DrawY = PanelY;
            int CurrentIndex = 0;
            for (int G = 0; G < ArrayGametypeCategory.Length; ++G)
            {
                DrawY += HeaderOffsetY;
                DrawY += FirstLineOffsetY;

                if (SelectGametype(ArrayGametypeCategory[G], ref CurrentIndex, ref DrawY))
                    break;

                DrawY += CategoryOffsetY;
            }
        }

        private bool SelectGametype(GametypeCategory ActiveCategory, ref int CurrentIndex, ref float DrawY)
        {
            ++CurrentIndex;

            for (int G = 0; G < ActiveCategory.ArrayGametype.Length; ++G)
            {
                if (CurrentIndex >= GametypeScrollbarValue && ActiveCategory.ArrayGametype[G].IsUnlocked)
                {
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + LineOffsetY
                        && InputHelper.InputConfirmPressed())
                    {
                        SelectedGametype = ActiveCategory.ArrayGametype[G];
                        Room.GameMode = SelectedGametype.Name;
                        Owner.OnGametypeUpdate();
                        return true;
                    }
                }

                DrawY += LineOffsetY;
                ++CurrentIndex;
            }

            return false;
        }

        public void SelectGametype(int CategoryIndex, int GametypeIndex)
        {
            SelectedGametype = ArrayGametypeCategory[CategoryIndex].ArrayGametype[GametypeIndex];
            Room.GameMode = SelectedGametype.Name;
            Owner.OnGametypeUpdate();
        }

        private void OnGametypeScrollbarChange(float ScrollbarValue)
        {
            GametypeScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            float DrawY = PanelY;

            g.Draw(sprFrameDescription, new Vector2(2280 * Ratio, DrawY + 78 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            int CurrentIndex = 0;
            for (int G = 0; G < ArrayGametypeCategory.Length; ++G)
            {
                DrawGametypeCategory(g, ArrayGametypeCategory[G], ref CurrentIndex, ref DrawY);
                DrawY += CategoryOffsetY;
            }

            if (SelectedGametype != null)
            {
                g.Draw(sprHighlight, new Vector2(2484 * Ratio, 1100 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);
                g.DrawStringCentered(fntOxanimumRegular, SelectedGametype.Name, new Vector2(2800 * Ratio, 1150 * Ratio), ColorText);

                float DescriptionY = 1220 * Ratio;
                foreach (string ActiveLine in TextHelper.FitToWidth(fntText, SelectedGametype.Description, (int)(350 * Ratio)))
                {
                    g.DrawString(fntOxanimumRegular, ActiveLine, new Vector2(2340 * Ratio, DescriptionY), ColorText);
                    DescriptionY += 80 * Ratio;
                }
            }

            GametypeScrollbar.Draw(g);
        }

        private void DrawGametypeCategory(CustomSpriteBatch g, GametypeCategory ActiveCategory, ref int CurrentIndex, ref float DrawY)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            g.DrawStringMiddleAligned(fntOxanimumBold, ActiveCategory.Category, new Vector2(LeftPanelX + PanelWidth / 2, DrawY), ColorText);
            DrawY += HeaderOffsetY;

            int BoxHeight = (int)(ActiveCategory.ArrayGametype.Length * LineOffsetY + FirstLineOffsetY);

            g.Draw(sprFrameTop, new Vector2(364 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(364 * Ratio), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), null, ColorBox, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            g.Draw(sprFrameTop, new Vector2(364 * Ratio, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

            DrawY += FirstLineOffsetY;
            ++CurrentIndex;

            for (int G = 0; G < ActiveCategory.ArrayGametype.Length; ++G)
            {
                if (CurrentIndex >= GametypeScrollbarValue)
                {
                    if (ActiveCategory.ArrayGametype[G].IsUnlocked)
                    {
                        if (SelectedGametype == ActiveCategory.ArrayGametype[G]
                            || (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                                && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + LineOffsetY
                                && Owner.IsOnTop))
                        {
                            g.Draw(sprHighlight, new Vector2(400 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);
                        }

                        g.DrawString(fntOxanimumRegular, ActiveCategory.ArrayGametype[G].Name, new Vector2(LeftPanelX + 5, DrawY), ColorText);
                    }
                    else
                    {
                        g.DrawString(fntOxanimumRegular, ActiveCategory.ArrayGametype[G].Name, new Vector2(LeftPanelX + 5, DrawY), Color.Gray);
                    }
                }

                DrawY += LineOffsetY;
                ++CurrentIndex;
            }
        }

        public override string ToString()
        {
            return "Gametype";
        }
    }
}
