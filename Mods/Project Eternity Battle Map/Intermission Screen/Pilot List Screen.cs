using System;
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
    public sealed class PilotListScreen : GameScreen
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

        private readonly Roster PlayerRoster;

        public int PilotChoice;
        public int CurrentPage;
        private int PageMax;
        public const int MaxPerPage = 5;

        private List<Unit> ListPresentUnit;
        private List<Character> ListPresentCharacter;

        public List<CharacterInfo> ListCharacterInfo;
        public CharacterInfo SelectedCharacter { get { return ListCharacterInfo[PilotChoice + (CurrentPage - 1) * MaxPerPage]; } }

        public PilotListScreen(Roster PlayerRoster)
            : base()
        {
            this.PlayerRoster = PlayerRoster;

            CurrentPage = 1;

            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
            ListPresentCharacter = PlayerRoster.TeamCharacters.GetPresent();

            PageMax = (int)Math.Ceiling(ListPresentCharacter.Count / (double)MaxPerPage);
        }

        public override void Load()
        {
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
                for (int P = 0; P < ListPresentUnit[U].MaxCharacter; P++)
                {
                    for (int i = 1; i < ListCharacterInfo.Count; i++)
                    {
                        if (ListPresentUnit[U].ArrayCharacterActive[P] != null && ListPresentCharacter[i - 1] == ListPresentUnit[U].ArrayCharacterActive[P])
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
            if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputConfirmPressed())
            {
            }
            else
            {
                UpdateMenu(gameTime);
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
                else if ((CurrentPage - 1) * MaxPerPage + PilotChoice >= ListPresentUnit.Count)
                    PilotChoice = Math.Max(0, (ListPresentUnit.Count) % MaxPerPage);

                sndSelection.Play();
            }
            else if (InputHelper.InputLeftPressed())
            {
                CurrentPage -= (CurrentPage > 1) ? 1 : 0;
                sndSelection.Play();
            }
            else if (InputHelper.InputRightPressed())
            {
                CurrentPage += (CurrentPage < PageMax) ? 1 : 0;
                sndSelection.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            DrawMenu(g);
            g.DrawString(fntFinlanderFont, "Pilot Status", new Vector2(120, 10), Color.White);
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
                var ActiveCharacterInfo = ListCharacterInfo[U];
                int Offset = ListCharacterInfo.Count - ListPresentCharacter.Count;
                if (U - Offset >= 0)
                {
                    Character ActiveCharacter = ListPresentCharacter[U - Offset];

                    g.DrawString(fntFinlanderFont, ActiveCharacter.Name, new Vector2(X, Y), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, ActiveCharacter.Level.ToString(), new Vector2(230, Y), Color.White);
                    g.DrawStringRightAligned(fntFinlanderFont, ActiveCharacter.Kills.ToString(), new Vector2(340, Y), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "-----------", new Vector2(X, Y), Color.White);
                }

                if (ActiveCharacterInfo.UnitIndex >= 0)
                {
                    Unit ActiveUnit = ListPresentUnit[ActiveCharacterInfo.UnitIndex];
                    g.Draw(ActiveUnit.SpriteMap, new Vector2(350, Y), Color.White);
                    g.DrawString(fntFinlanderFont, ActiveUnit.RelativePath, new Vector2(380, Y), Color.White);
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