using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGametypeScreen : GameScreen
    {
        private struct GametypeCategory
        {
            public string Category;
            public Gametype[] ArrayGametype;

            public GametypeCategory(string Category, Gametype[] ArrayGametype)
            {
                this.Category = Category;
                this.ArrayGametype = ArrayGametype;
            }
        }
        private struct Gametype
        {
            public string Name;
            public string Description;
            public Texture2D sprPreview;

            public Gametype(string Name, string Description, Texture2D sprPreview)
            {
                this.Name = Name;
                this.Description = Description;
                this.sprPreview = sprPreview;
            }
        }

        private SpriteFont fntText;
        private BoxScrollbar GametypeScrollbar;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen Owner;

        int PanelY = (int)(Constants.Height * 0.15);
        int PanelWidth = (int)(Constants.Width * 0.4);
        int PanelHeight = (int)(Constants.Height * 0.75);

        int LeftPanelX = (int)(Constants.Width * 0.03);

        private GametypeCategory[] ArrayGametypeCategory;
        private int GametypeScrollbarValue;
        private Gametype SelectedGametype;

        public GameOptionsGametypeScreen(RoomInformations Room, GameOptionsScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            Gametype GametypeCampaign = new Gametype("Campaign", "Classic mission based mode, no respawn.", null);
            Gametype GametypeHorde = new Gametype("Horde", "Wave survival mode, respawn at the start of each wave.", null);
            Gametype GametypeBaseDefense = new Gametype("Base Defense", "Wave survival mode, respawn at the start of each wave. Must defend a base by building turrets.", null);

            Gametype GametypeDeathmatch = new Gametype("Deathmatch", "Gain points for kills and assists, respawn on death.", null);
            Gametype GametypeCaptureTheFlag = new Gametype("Capture The Flag", "Capture a flag in the enemy base and bring it back to your own flag to score a point.", null);
            Gametype GametypeObjective = new Gametype("Objective", "One team must complete objectives while another prevent them.", null);
            Gametype GametypeAssault = new Gametype("Assault", "Team deathmatch with limited respawns.", null);
            Gametype GametypeConquest = new Gametype("Conquest", "Teams must fight to capture respawn bases that give them points. The starting base may or may not be capturable.", null);
            Gametype GametypeOnslaught = new Gametype("Onslaught", "Teams must fight to capture respawn bases that give them access to the enemy base's core. Last team with a core win.", null);
            Gametype GametypeKingOfTheHill = new Gametype("King Of The Hill", "Hold a position without enemies to win points.", null);
            Gametype GametypeBunny = new Gametype("Bunny", "Unit that holds the flag become the bunny and gets points for kills, everyone else try to kill the bunny.", null);
            Gametype GametypeFreezeTag = new Gametype("Freeze Tag", "Killing an enemy freeze him, when every enemies are frozen you win. Teamates can unfreeze allie by staying next to them for 2 turns.", null);
            Gametype GametypeJailbreak = new Gametype("Jailbreak", "Killing an enemy send him to your prison, capture everyone to win. Teamates can be freed by standing on a switch.", null);
            Gametype GametypeMutant = new Gametype("Mutant", "First kill transform you into the mutant, a unit with overpowered stats and attacks. Only the Mutant can kill or be killed.", null);
            Gametype GametypeKaiju = new Gametype("Kaiju", "One player controls giant monsters while the other players use their units.", null);

            SelectedGametype = GametypeCampaign;
            ArrayGametypeCategory = new GametypeCategory[2];
            ArrayGametypeCategory[0] = new GametypeCategory("PVE", new Gametype[] { GametypeCampaign, GametypeHorde, GametypeBaseDefense });
            ArrayGametypeCategory[1] = new GametypeCategory("PVP", new Gametype[]
            {
                GametypeDeathmatch, GametypeCaptureTheFlag, GametypeObjective, GametypeAssault, GametypeConquest,
                GametypeOnslaught, GametypeKingOfTheHill, GametypeBunny, GametypeFreezeTag,
                GametypeJailbreak, GametypeMutant, GametypeKaiju,
            });

            int PanelY = (int)(Constants.Height * 0.15);
            int PanelWidth = (int)(Constants.Width * 0.4);
            int PanelHeight = (int)(Constants.Height * 0.75);

            int LeftPanelX = (int)(Constants.Width * 0.03);

            GametypeScrollbar = new BoxScrollbar(new Vector2(LeftPanelX + PanelWidth - 20, PanelY), PanelHeight, 10, OnGametypeScrollbarChange);
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
                if (CurrentIndex >= GametypeScrollbarValue)
                {
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20
                        && InputHelper.InputConfirmPressed())
                    {
                        SelectedGametype = ActiveCategory.ArrayGametype[G];
                        Room.RoomType = SelectedGametype.Name;
                        Owner.OnGametypeUpdate();
                        return true;
                    }

                    DrawY += 20;
                }

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
            DrawBox(g, new Vector2(LeftPanelX, PanelY), PanelWidth, PanelHeight, Color.White);

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

            DrawBox(g, new Vector2(RightPanelX, PanelY), PanelWidth, PanelHeight, Color.White);

            DrawBox(g, new Vector2(RightPanelContentX, PreviewBoxY), RightPanelContentWidth, PreviewBoxHeight, Color.White);
            DrawBox(g, new Vector2(RightPanelContentX, DescriptionBoxY), RightPanelContentWidth, DescriptionHeight, Color.White);
            DrawBox(g, new Vector2(DescriptionBoxNameX, DescriptionBoxY), DescriptionBoxNameWidth, 30, Color.White);

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
                    g.DrawString(fntText, ActiveCategory.ArrayGametype[G].Name, new Vector2(LeftPanelX + 5, DrawY), Color.White);
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20)
                    {
                        g.Draw(sprPixel, new Rectangle(LeftPanelX, (int)DrawY, PanelWidth, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawY += 20;
                }

                ++CurrentIndex;
            }
        }

        public override string ToString()
        {
            return "Gametype";
        }
    }
}
