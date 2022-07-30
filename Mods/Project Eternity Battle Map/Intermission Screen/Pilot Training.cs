﻿using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class PilotTrainingScreen : GameScreen
    {
        public class CharacterInfo
        {
            public int UnitIndex;

            public CharacterInfo()
            {
                UnitIndex = -1;
            }
            public CharacterInfo(int UnitIndex)
            {
                this.UnitIndex = UnitIndex;
            }
        }

        private Texture2D sprMapMenuBackground;

        private Texture2D sprBarLargeBackground;
        private Texture2D sprBarLargeEN;
        private Texture2D sprBarLargeHP;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;
        private FMODSound sndCancel;

        private SpriteFont fntFinlanderFont;

        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;

        public int PilotChoice;
        public int CurrentPage;
        private int PageMax;
        public const int MaxPerPage = 5;

        private List<Character> ListPresentCharacter;
        private List<Unit> ListPresentUnit;

        public List<CharacterInfo> ListCharacterInfo;
        public CharacterInfo SelectedCharacter { get { return ListCharacterInfo[PilotChoice + (CurrentPage - 1) * MaxPerPage]; } }

        private int Stage = 0;

        public PilotTrainingScreen(BattleMapPlayer ActivePlayer, Roster PlayerRoster)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.PlayerRoster = PlayerRoster;

            CurrentPage = 1;
        }

        public override void Load()
        {
            ListPresentCharacter = PlayerRoster.TeamCharacters.GetPresent();
            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();

            PageMax = (int)Math.Ceiling(ListPresentCharacter.Count / (double)MaxPerPage);
            sprMapMenuBackground = Content.Load<Texture2D>("Status Screen/Background Black");

            sprBarLargeBackground = Content.Load<Texture2D>("Battle/Bars/Large Bar");
            sprBarLargeEN = Content.Load<Texture2D>("Battle/Bars/Large Energy");
            sprBarLargeHP = Content.Load<Texture2D>("Battle/Bars/Large Health");

            sprLand = Content.Load<Texture2D>("Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Status Screen/Space");

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            ListCharacterInfo = new List<CharacterInfo>();
            for (int P = 0; P < ListPresentCharacter.Count; P++)
            {
                if (ListPresentCharacter[P].CanPilot)
                {
                    ListCharacterInfo.Add(new CharacterInfo());
                }
            }

            //Link characters to Intermission Unit
            for (int U = 0; U < ListPresentUnit.Count; U++)
            {
                for (int P = 0; P < ListPresentUnit[U].ArrayCharacterActive.Length; P++)
                {
                    for (int i = 0; i < ListCharacterInfo.Count; i++)
                    {
                        if (ListPresentUnit[U].ArrayCharacterActive[P] != null && ListPresentCharacter[i] == ListPresentUnit[U].ArrayCharacterActive[P])
                        {
                            ListCharacterInfo[i].UnitIndex = U;
                            break;
                        }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (Stage)
            {
                case 0:
                    if (InputHelper.InputCancelPressed())
                    {
                        RemoveScreen(this);
                    }
                    else if (InputHelper.InputConfirmPressed() && PilotChoice < ListPresentCharacter.Count)
                    {
                        Stage = 1;
                    }
                    else
                    {
                        UpdateMenu(gameTime);
                    }
                    break;

                case 1:
                    if (InputHelper.InputCancelPressed())
                    {
                        Stage = 0;
                    }
                    break;
            }
        }

        public void UpdateMenu(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                PilotChoice -= (PilotChoice > 0) ? 1 : 0;
                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ++PilotChoice;
                if (PilotChoice >= MaxPerPage)
                    PilotChoice = MaxPerPage - 1;
                else if ((CurrentPage - 1) * MaxPerPage + PilotChoice >= ListCharacterInfo.Count)
                    PilotChoice = Math.Max(0, (ListCharacterInfo.Count - 1) % MaxPerPage);

                sndSelection.Play();
            }
            else if (InputHelper.InputLeftPressed())
            {
                CurrentPage -= (CurrentPage > 1) ? 1 : 0;
                if ((CurrentPage - 1) * MaxPerPage + PilotChoice >= ListCharacterInfo.Count)
                    PilotChoice = Math.Max(0, (ListCharacterInfo.Count - 1) % MaxPerPage);
                sndSelection.Play();
            }
            else if (InputHelper.InputRightPressed())
            {
                CurrentPage += (CurrentPage < PageMax) ? 1 : 0;
                if ((CurrentPage - 1) * MaxPerPage + PilotChoice >= ListCharacterInfo.Count)
                    PilotChoice = Math.Max(0, (ListCharacterInfo.Count - 1) % MaxPerPage);
                sndSelection.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            switch (Stage)
            {
                case 0:
                    DrawMenu(g);
                    g.DrawString(fntFinlanderFont, "Pilot Status", new Vector2(120, 10), Color.White);
                    break;

                case 1:
                    Character ActiveCharacter = ListPresentCharacter[(CurrentPage - 1) * MaxPerPage + PilotChoice];

                    DrawBox(g, new Vector2(10, 20), 104, 150, Color.White);
                    g.Draw(sprPixel, new Rectangle(20, 13, 84, 84), Color.Gray);
                    g.Draw(sprPixel, new Rectangle(22, 15, 80, 80), Color.White);
                    g.Draw(ActiveCharacter.sprPortrait, new Vector2(22, 15), Color.White);
                    TextHelper.DrawText(g, ActiveCharacter.Name, new Vector2(20, 100), Color.Yellow);
                    TextHelper.DrawText(g, "Lv", new Vector2(20, 120), Color.Yellow);
                    TextHelper.DrawText(g, ActiveCharacter.Level.ToString(), new Vector2(60, 120), Color.White);
                    TextHelper.DrawText(g, "SP", new Vector2(20, 140), Color.Yellow);
                    TextHelper.DrawText(g, ActiveCharacter.SP.ToString(), new Vector2(60, 140), Color.White);

                    DrawBox(g, new Vector2(10, 180), 150, 100, Color.White);
                    TextHelper.DrawText(g, "Current PP", new Vector2(20, 190), Color.Yellow);
                    TextHelper.DrawTextRightAligned(g, ActiveCharacter.PilotPoints.ToString(), new Vector2(150, 190), Color.White);
                    TextHelper.DrawText(g, "Skill PP Cost", new Vector2(20, 220), Color.Yellow);
                    TextHelper.DrawTextRightAligned(g, ActiveCharacter.PilotPoints.ToString(), new Vector2(150, 220), Color.White);
                    TextHelper.DrawText(g, "Remaining PP", new Vector2(20, 250), Color.Yellow);
                    TextHelper.DrawTextRightAligned(g, ActiveCharacter.PilotPoints.ToString(), new Vector2(150, 250), Color.White);

                    DrawBox(g, new Vector2(120, 10), 510, 100, Color.White);
                    TextHelper.DrawText(g, "Skill Description", new Vector2(125, 15), Color.Yellow);
                    DrawBox(g, new Vector2(170, 120), 200, 160, Color.White);
                    TextHelper.DrawText(g, "Pilot Skills", new Vector2(180, 125), Color.Yellow);
                    for (int S = 0; S < 6; ++S)
                    {
                        if (S < ActiveCharacter.ArrayPilotSkill.Length)
                        {
                            TextHelper.DrawText(g, ActiveCharacter.ArrayPilotSkill[S].Name, new Vector2(180, 150 + S * 20), Color.White);
                        }
                        else
                        {
                            TextHelper.DrawText(g, "-------", new Vector2(180, 150 + S * 20), Color.White);
                        }
                    }
                    DrawBox(g, new Vector2(375, 120), 255, 160, Color.White);
                    TextHelper.DrawText(g, "Available Skills", new Vector2(385, 125), Color.Yellow);
                    TextHelper.DrawTextRightAligned(g, "1/4", new Vector2(530, 125), Color.LightBlue);
                    TextHelper.DrawTextRightAligned(g, "Required PP", new Vector2(620, 125), Color.Yellow);
                    for (int S = 0; S < 6; ++S)
                    {
                        TextHelper.DrawText(g, "-------", new Vector2(385, 150 + S * 20), Color.White);
                        TextHelper.DrawTextRightAligned(g, "0", new Vector2(620, 150 + S * 20), Color.White);
                    }
                    break;
            }
        }

        public void DrawMenu(CustomSpriteBatch g)
        {
            g.Draw(sprMapMenuBackground, new Vector2(0, 0), Color.White);
            int LineSpacing = 30;
            g.DrawString(fntFinlanderFont, CurrentPage + "/" + PageMax, new Vector2(420, 10), Color.White);
            DrawBox(g, new Vector2(10, 45), 340, 300, Color.White);
            g.DrawString(fntFinlanderFont, "Pilot", new Vector2(20, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "LV", new Vector2(230, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "KILLS", new Vector2(340, 50), Color.Yellow);
            DrawBox(g, new Vector2(350, 45), 280, 300, Color.White);
            g.DrawString(fntFinlanderFont, "UNIT", new Vector2(360, 50), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "HP", new Vector2(590, 50), Color.Yellow);

            int X = 20;
            int Y = 80;
            int UnitIndex = (CurrentPage - 1) * MaxPerPage;
            for (int U = 0; U + UnitIndex < ListCharacterInfo.Count && U < MaxPerPage; U++)
            {
                var ActiveCharacterInfo = ListCharacterInfo[U + UnitIndex];
                int Offset = ListCharacterInfo.Count - ListPresentCharacter.Count;
                if (U - Offset >= 0)
                {
                    Character ActiveCharacter = ListPresentCharacter[U + UnitIndex - Offset];

                    g.DrawString(fntFinlanderFont, ActiveCharacter.Name, new Vector2(X, Y), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, ActiveCharacter.Level.ToString(), new Vector2(230, Y), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, ActivePlayer.Records.PlayerUnitRecords.DicCharacterIDByNumberOfKills[ActiveCharacter.ID].ToString(), new Vector2(340, Y), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "-----------", new Vector2(X, Y), Color.White);
                }

                if (ActiveCharacterInfo.UnitIndex >= 0)
                {
                    Unit ActiveUnit = ListPresentUnit[ActiveCharacterInfo.UnitIndex];
                    g.Draw(ActiveUnit.SpriteMap, new Vector2(355, Y), Color.White);
                    g.DrawString(fntFinlanderFont, ActiveUnit.ItemName, new Vector2(385, Y), Color.White);
                    TextHelper.DrawTextRightAligned(g, ActiveUnit.MaxHP.ToString(), new Vector2(600, Y), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "-----------", new Vector2(380, Y), Color.White);
                    TextHelper.DrawTextRightAligned(g, "---------", new Vector2(600, Y), Color.White);
                }
                Y += LineSpacing;
            }
            g.Draw(BattleMap.sprPixel, new Rectangle(X, 80 + PilotChoice * LineSpacing, 600, LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 127));
        }
    }
}