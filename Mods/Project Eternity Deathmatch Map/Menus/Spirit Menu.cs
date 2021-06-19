using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SpiritMenu : GameScreen
    {
        #region Ressources

        private Texture2D sprSpiritMenuMain;
        private Texture2D sprSpiritMenuPilotBox;
        private Texture2D sprSpiritMenuSelectBox;
        private Texture2D sprSpiritMenuSelectChosen;
        private Texture2D sprSpiritMenuDescription;
        private Texture2D sprSpiritMenuHighlight;

        public static Texture2D sprBarSmallBackground;
        public static Texture2D sprBarSmallEN;
        public static Texture2D sprBarSmallHP;

        protected SpriteFont fntBattleNumberSmall;

        #endregion

        #region Variables

        private DeathmatchMap Map;
        private Squad ActiveSquad;
        
        protected List<ManualSkill> ListSelectedSpirit;
        protected List<ManualSkill>[][] PilotSpiritActivation;//Unit, Pilot, Active Spirits list.
        protected ManualSkill ActiveSpirit//Used to tell Spirit is selected in the menu.
        {
            get
            {
                if (ActiveUnitIndex >= 0 && ActiveUnitIndex < ActiveSquad.UnitsAliveInSquad &&
                    PilotIndex >= 0 && PilotIndex < ActiveSquad[ActiveUnitIndex].ArrayCharacterActive.Length &&
                    CursorIndex >= 0 && CursorIndex < ActiveSquad[ActiveUnitIndex].ArrayCharacterActive[PilotIndex].ArrayPilotSpirit.Length)
                {
                    return ActiveSquad[ActiveUnitIndex].ArrayCharacterActive[PilotIndex].ArrayPilotSpirit[CursorIndex];
                }
                else
                    return null;
            }
        }

        protected int ActiveUnitIndex;

        protected int PilotIndex
        {
            get
            {
                switch (ActiveUnitIndex)
                {
                    case 0:
                        return PilotIndexLeader;

                    case 1:
                        return PilotIndexWingmanA;

                    case 2:
                        return PilotIndexWingmanB;
                }
                return 0;
            }
            set
            {
                switch (ActiveUnitIndex)
                {
                    case 0:
                        PilotIndexLeader = value;
                        break;

                    case 1:
                        PilotIndexWingmanA = value;
                        break;

                    case 2:
                        PilotIndexWingmanB = value;
                        break;
                }
            }
        }

        protected int PilotIndexLeader;
        protected int PilotIndexWingmanA;
        protected int PilotIndexWingmanB;

        protected int CursorIndex
        {
            get
            {
                switch (ActiveUnitIndex)
                {
                    case 0:
                        return CursorIndexLeader;

                    case 1:
                        return CursorIndexWingmanA;

                    case 2:
                        return CursorIndexWingmanB;
                }
                return 0;
            }
            set
            {
                switch (ActiveUnitIndex)
                {
                    case 0:
                        CursorIndexLeader = value;
                        break;

                    case 1:
                        CursorIndexWingmanA = value;
                        break;

                    case 2:
                        CursorIndexWingmanB = value;
                        break;
                }
            }
        }

        protected int CursorIndexLeader;
        protected int CursorIndexWingmanA;
        protected int CursorIndexWingmanB;

        #endregion

        public SpiritMenu(DeathmatchMap Map)
        {
            this.Map = Map;
            RequireDrawFocus = true;
            if (Map != null)
                ListGameScreen = Map.ListGameScreen;
        }

        public override void Load()
        {
            ListSelectedSpirit = new List<ManualSkill>();

            sprSpiritMenuMain = Content.Load<Texture2D>("Spirit Screen/Main");
            sprSpiritMenuPilotBox = Content.Load<Texture2D>("Spirit Screen/Pilot Box");
            sprSpiritMenuSelectBox = Content.Load<Texture2D>("Spirit Screen/Select Box");
            sprSpiritMenuSelectChosen = Content.Load<Texture2D>("Spirit Screen/Select Chosen");
            sprSpiritMenuDescription = Content.Load<Texture2D>("Spirit Screen/Description");
            sprSpiritMenuHighlight = Content.Load<Texture2D>("Spirit Screen/Highlight");

            sprBarSmallBackground = Content.Load<Texture2D>("Battle/Bars/Small Bar");
            sprBarSmallEN = Content.Load<Texture2D>("Battle/Bars/Small Energy");
            sprBarSmallHP = Content.Load<Texture2D>("Battle/Bars/Small Health");

            fntBattleNumberSmall = Content.Load<SpriteFont>("Fonts/Battle Numbers Small");
            fntBattleNumberSmall.Spacing = -3;
        }
        
        public void InitSpiritScreen(Squad ActiveSquad)
        {
            Alive = true;
            PushScreen(this);
            this.ActiveSquad = ActiveSquad;

            ActiveUnitIndex = 0;

            PilotIndex = 0;
            PilotIndexWingmanA = 0;
            PilotIndexWingmanB = 0;

            CursorIndex = 0;
            CursorIndexWingmanA = 0;
            CursorIndexWingmanB = 0;

            ListSelectedSpirit.Clear();
            PilotSpiritActivation = new List<ManualSkill>[ActiveSquad.UnitsAliveInSquad][];
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
            {
                PilotSpiritActivation[U] = new List<ManualSkill>[ActiveSquad[U].ArrayCharacterActive.Length];

                for (int C = 0; C < ActiveSquad[U].ArrayCharacterActive.Length; C++)
                {
                    Map.GlobalDeathmatchContext.SetContext(ActiveSquad, ActiveSquad[U], ActiveSquad[U].ArrayCharacterActive[C], null, null, null);

                    PilotSpiritActivation[U][C] = new List<ManualSkill>();

                    for (int S = 0; S < ActiveSquad[U].ArrayCharacterActive[C].ArrayPilotSpirit.Length; ++S)
                    {
                        //Update spirits so you know which one you can use.
                        ActiveSquad[U].ArrayCharacterActive[C].ArrayPilotSpirit[S].UpdateSkillActivation();
                    }
                }
            }

            Map.GlobalDeathmatchContext.SetContext(null, null, null, null, null, null);
        }

        public override void Update(GameTime gameTime)
        {
            #region Toggle Spirit

            //Add or remove the EN needed to the pilot.
            if (InputHelper.InputCommand1Pressed())
            {
                if (ActiveSpirit == null || !ActiveSpirit.IsUnlocked)
                    return;

                if (ActiveSpirit.CanActivate)
                {
                    if (PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Contains(ActiveSpirit))//Deactivate Skill
                    {
                        ListSelectedSpirit.Remove(ActiveSpirit);
                        PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Remove(ActiveSpirit);

                        Map.sndSelection.Play();
                    }
                    else if (GetPilotRemainingSP() - ActiveSpirit.SPCost >= 0)//Activate Skill
                    {
                        if (ListSelectedSpirit.Count == 0 || (!ActiveSpirit.Target.MustBeUsedAlone && !ListSelectedSpirit.Last().Target.MustBeUsedAlone))
                        {
                            ListSelectedSpirit.Add(ActiveSpirit);
                            PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Add(ActiveSpirit);

                            Map.sndSelection.Play();
                        }
                    }
                    else
                    {
                        Map.sndDeny.Play();
                    }
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }

            #endregion

            else if (InputHelper.InputConfirmPressed())
            {
                if (ActiveSpirit == null || !ActiveSpirit.IsUnlocked)
                    return;

                if (ListSelectedSpirit.Count == 0)
                {
                    if (ActiveSpirit.CanActivate && GetPilotRemainingSP() - ActiveSpirit.SPCost >= 0)
                    {
                        ListSelectedSpirit.Add(ActiveSpirit);
                        PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Add(ActiveSpirit);
                    }
                    else
                        return;
                }
                
                if (ListSelectedSpirit[0].Range > 0)
                {
                    List<Vector3> ListMVChoice = Map.ComputeRange(ActiveSquad.Position, 0, ListSelectedSpirit[0].Range);
                    Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
                }
                
                for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                {
                    for (int C = 0; C < ActiveSquad[U].ArrayCharacterActive.Length; C++)
                    {
                        for (int S = 0; S < PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Count; ++S)
                        {
                            Map.GlobalDeathmatchContext.SetContext(ActiveSquad, ActiveSquad[U], ActiveSquad[U].ArrayCharacterActive[C], null, null, null);

                            ManualSkill SpiritToActivate = PilotSpiritActivation[ActiveUnitIndex][PilotIndex][S];

                            //Consume SP and activate skills.
                            SpiritToActivate.ActiveSkillFromMenu(ActiveSquad[U].ArrayCharacterActive[C], ActiveSquad);
                            //Update skills activation to disable those who can't be used anymore.
                            SpiritToActivate.UpdateSkillActivation();

                            ListSelectedSpirit.Remove(SpiritToActivate);
                            PilotSpiritActivation[ActiveUnitIndex][PilotIndex].Remove(SpiritToActivate);
                        }
                    }
                }

                for (int P = 0; P < Map.ListPlayer.Count; ++P)
                {
                    for (int S = 0; S < Map.ListPlayer[P].ListSquad.Count; ++S)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad; ++U)
                        {
                            Map.ActivateAutomaticSkills(Map.ListPlayer[P].ListSquad[S], Map.ListPlayer[P].ListSquad[S][U], string.Empty, Map.ListPlayer[P].ListSquad[S], Map.ListPlayer[P].ListSquad[S][U]);
                        }
                    }
                }

                Map.GlobalDeathmatchContext.SetContext(null, null, null, null, null, null);

                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed())
            {
                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                Map.sndCancel.Play();

                RemoveScreen(this);
            }

            #region Move the Spirit Menu cursor

            else if (InputHelper.InputUpPressed())
            {//Move Spirit cursor down.
                if (CursorIndex - 1 >= 0)
                {
                    CursorIndex--;

                    Map.sndSelection.Play();
                }
                //End of Spirit list, move to last pilot.
                else if (PilotIndex - 1 > 0)
                {
                    PilotIndex--;
                    Map.sndSelection.Play();
                }
            }
            else if (InputHelper.InputDownPressed())
            {//Move Spirit cursor up.
                if (CursorIndex + 1 < ActiveSquad[ActiveUnitIndex].ArrayCharacterActive[PilotIndex].ArrayPilotSpirit.Length)
                {
                    CursorIndex++;

                    Map.sndSelection.Play();
                }
                //End of Spirit list, move to next pilot.
                else if (PilotIndex + 1 < ActiveSquad[ActiveUnitIndex].ArrayCharacterActive.Length)
                {
                    PilotIndex++;
                    Map.sndSelection.Play();
                }
            }
            else if (InputHelper.InputRightPressed())
            {
                if (ActiveUnitIndex < ActiveSquad.UnitsAliveInSquad - 1)
                {
                    ActiveUnitIndex++;
                    PilotIndex = 0;
                    CursorIndex = 0;
                }
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (ActiveUnitIndex > 0)
                {
                    ActiveUnitIndex--;
                    PilotIndex = 0;
                    CursorIndex = 0;
                }
            }

            #endregion
        }

        private int GetPilotRemainingSP()
        {
            int CurrentPilotSP = ActiveSquad[ActiveUnitIndex].ArrayCharacterActive[PilotIndex].SP;

            foreach (ManualSkill ActiveSpirit in PilotSpiritActivation[ActiveUnitIndex][PilotIndex])
            {
                CurrentPilotSP -= ActiveSpirit.SPCost;
            }

            return CurrentPilotSP;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //In spirit stats page.
            DrawBox(g, new Vector2(25, 0), 578, 25, Color.White);

            DrawSpiritMenu(g, 0, 25, PilotIndexLeader, CursorIndexLeader, ActiveUnitIndex == 0);
            if (ActiveSquad.CurrentWingmanA != null)
                DrawSpiritMenu(g, 1, 225, PilotIndexWingmanA, CursorIndexWingmanA, ActiveUnitIndex == 1);
            if (ActiveSquad.CurrentWingmanB != null)
                DrawSpiritMenu(g, 2, 425, PilotIndexWingmanB, CursorIndexWingmanB, ActiveUnitIndex == 2);
        }

        public void DrawSpiritMenu(CustomSpriteBatch g, int DisplayedUnitIndex, int StartX, int PilotIndex, int CursorIndex, bool DrawCursor)
        {
            int Y = 255;

            for (int P = 1; P < ActiveSquad[DisplayedUnitIndex].ArrayCharacterActive.Length; P++)
            {
                Y += 10;

                DrawBox(g, new Vector2(StartX, Y), 189, 25, Color.White);
                DrawText(g, ActiveSquad[DisplayedUnitIndex].ArrayCharacterActive[P].Name,
                    new Vector2(StartX + 8, Y + 3), Color.White);
                Y += sprSpiritMenuPilotBox.Height;
            }

            Y = 25 + sprSpiritMenuPilotBox.Height * PilotIndex;

            DrawBox(g, new Vector2(StartX, Y), 189, 120, Color.White);
            DrawBox(g, new Vector2(StartX, Y + 120), 189, 115, Color.White);

            #region Draw Active Spirit Informations

            Character ActiveCharacter = ActiveSquad[DisplayedUnitIndex].ArrayCharacterActive[PilotIndex];

            DrawBox(g, new Vector2(StartX + 4, Y + 4), 88, 88, Color.White);
            g.Draw(sprPixel, new Rectangle(StartX + 8, Y + 8, 80, 80), Color.Gray);
            g.Draw(ActiveCharacter.sprPortrait,
                new Vector2(StartX + 8, Y + 8), Color.White);

            DrawText(g, ActiveCharacter.Name,
                new Vector2(StartX + 93, Y + 10), Color.White);

            DrawText(g, "Lvl", new Vector2(StartX + 95, Y + 30), Color.Yellow);
            DrawTextRightAligned(g, ActiveCharacter.Level.ToString(),
                new Vector2(StartX + 183, Y + 30), Color.White);

            DrawText(g, "SP", new Vector2(StartX + 95, Y + 50), Color.Yellow);
            DrawTextRightAligned(g, "/" + ActiveCharacter.MaxSP,
                new Vector2(StartX + 183, Y + 50), Color.White);
            int ENWidth = (int)fntShadowFont.MeasureString(ActiveCharacter.MaxSP.ToString()).X - 2;
            
            DrawTextRightAligned(g, ActiveCharacter.SP.ToString(), new Vector2(StartX + 183 - ENWidth, Y + 50), Color.Lime);

            DrawText(g, "Will", new Vector2(StartX + 95, Y + 70), Color.Yellow);
            DrawTextRightAligned(g, ActiveCharacter.Will.ToString(),
                new Vector2(StartX + 183, Y + 70), Color.White);

            DrawText(g, "HP", new Vector2(StartX + 6, Y + 99), Color.CornflowerBlue);
            DrawText(g, "EN", new Vector2(StartX + 100, Y + 99), Color.Orange);
            DrawBar(g, sprBarSmallBackground, sprBarSmallHP, new Vector2(StartX + 36, Y + 105), ActiveSquad[DisplayedUnitIndex].HP, ActiveSquad[DisplayedUnitIndex].MaxHP);
            DrawBar(g, sprBarSmallBackground, sprBarSmallEN, new Vector2(StartX + 128, Y + 105), ActiveSquad[DisplayedUnitIndex].EN, ActiveSquad[DisplayedUnitIndex].MaxEN);

            g.DrawStringRightAligned(fntBattleNumberSmall, ActiveSquad[DisplayedUnitIndex].HP.ToString(), new Vector2(StartX + 82, Y + 97), Color.White);
            g.DrawStringRightAligned(fntBattleNumberSmall, ActiveSquad[DisplayedUnitIndex].EN.ToString(), new Vector2(StartX + 175, Y + 97), Color.White);

            if (DrawCursor)
            {
                if (CursorIndex < ActiveCharacter.ArrayPilotSpirit.Length)
                {
                    g.Draw(sprSpiritMenuHighlight, new Vector2(StartX + 4, Y + 124 + CursorIndex * 18), Color.White);

                    if (CursorIndex < ActiveCharacter.ArrayPilotSpirit.Length)
                        DrawText(g, ActiveCharacter.ArrayPilotSpirit[CursorIndex].Description,
                            new Vector2(32, 3), Color.White);
                }
            }

            for (int S = 0; S < ActiveCharacter.ArrayPilotSpirit.Length; S++)
            {
                g.Draw(sprSpiritMenuSelectBox, new Vector2(StartX + 5, Y + 125 + S * 18), Color.White);
                if (PilotSpiritActivation[DisplayedUnitIndex][PilotIndex].Contains(ActiveCharacter.ArrayPilotSpirit[S]))
                    g.Draw(sprSpiritMenuSelectChosen, new Vector2(StartX + 5, Y + 125 + S * 18), Color.White);

                if (ActiveCharacter.ArrayPilotSpirit[S].IsUnlocked)
                {
                    if (ActiveCharacter.ArrayPilotSpirit[S].CanActivate && !PilotSpiritActivation[DisplayedUnitIndex][PilotIndex].Contains(ActiveCharacter.ArrayPilotSpirit[S])
                        && GetPilotRemainingSP() - ActiveCharacter.ArrayPilotSpirit[S].SPCost >= 0)
                    {
                        DrawText(g, ActiveCharacter.ArrayPilotSpirit[S].Name,
                            new Vector2(StartX + 20, Y + 123 + S * 18), Color.White);
                        DrawTextRightAligned(g, ActiveCharacter.ArrayPilotSpirit[S].SPCost.ToString(),
                            new Vector2(StartX + 183, Y + 123 + S * 18), Color.White);
                    }
                    else
                    {
                        DrawText(g, ActiveCharacter.ArrayPilotSpirit[S].Name,
                            new Vector2(StartX + 20, Y + 123 + S * 18), Color.Gray);
                        DrawTextRightAligned(g, ActiveCharacter.ArrayPilotSpirit[S].SPCost.ToString(),
                            new Vector2(StartX + 183, Y + 123 + S * 18), Color.Gray);
                    }
                }
                else
                {
                    DrawText(g, "------------------",
                        new Vector2(StartX + 20, Y + 123 + S * 18), Color.White);
                    DrawTextRightAligned(g, "--",
                        new Vector2(StartX + 183, Y + 123 + S * 18), Color.White);
                }
            }

            #endregion
        }
    }
}
