using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ConsumablePartMenu : GameScreen
    {
        #region Ressources

        private Texture2D sprSpiritMenuHighlight;

        protected SpriteFont fntBattleNumberSmall;

        #endregion
        
        private DeathmatchMap Map;
        private Squad ActiveSquad;

        private int CursorIndex;
        private List<UnitConsumablePart> ListUnitPart;

        public ConsumablePartMenu(DeathmatchMap Map, Squad ActiveSquad)
        {
            this.Map = Map;
            this.ActiveSquad = ActiveSquad;
            RequireDrawFocus = true;
            ListGameScreen = Map.ListGameScreen;
        }

        public override void Load()
        {
            sprSpiritMenuHighlight = Content.Load<Texture2D>("Spirit Screen/Highlight");

            fntBattleNumberSmall = Content.Load<SpriteFont>("Fonts/Battle Numbers Small");
            fntBattleNumberSmall.Spacing = -3;
            ListUnitPart = new List<UnitConsumablePart>();
            for (int P = 0; P < ActiveSquad.CurrentLeader.ArrayParts.Length; ++P)
            {
                if (ActiveSquad.CurrentLeader.ArrayParts[P] != null && ActiveSquad.CurrentLeader.ArrayParts[P].PartType == PartTypes.Consumable)
                {
                    ListUnitPart.Add((UnitConsumablePart)ActiveSquad.CurrentLeader.ArrayParts[P]);
                }
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() && ListUnitPart.Count > 0)
            {
                ManualSkill ActiveSpirit = ListUnitPart[CursorIndex].Spirit;

                if (ActiveSpirit.Range > 0)
                {
                    Map.ComputeRange(ActiveSquad.Position, 0, ActiveSpirit.Range);
                }
                
                for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                {
                    for (int C = 0; C < ActiveSquad[U].ArrayCharacterActive.Length; C++)
                    {
                        Map.Params.GlobalContext.SetContext(ActiveSquad, ActiveSquad[U], ActiveSquad[U].ArrayCharacterActive[C], null, null, null, Map.ActiveParser);

                        ManualSkill SpiritToActivate = ActiveSpirit;

                        //Consume SP and activate skills.
                        SpiritToActivate.ActiveSkillFromMenu();
                        //Update skills activation to disable those who can't be used anymore.
                        SpiritToActivate.UpdateSkillActivation();
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

                Map.Params.GlobalContext.SetContext(null, null, null, null, null, null, Map.ActiveParser);

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
            }
            else if (InputHelper.InputDownPressed())
            {//Move Spirit cursor up.
                if (CursorIndex + 1 < ListUnitPart.Count)
                {
                    CursorIndex++;

                    Map.sndSelection.Play();
                }
            }

            #endregion
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //In spirit stats page.
            DrawBox(g, new Vector2(25, 0), 578, 25, Color.White);

            int BoxWidth = 185;
            int StartX = (Constants.Width - BoxWidth) / 2;
            int Y = (Constants.Height - 150) / 2;

            DrawBox(g, new Vector2(StartX, Y + 95), BoxWidth, 25, Color.White);
            TextHelper.DrawText(g, "SELECT: PARTS",
                new Vector2(StartX + 8, Y + 98), Color.White);
            DrawBox(g, new Vector2(StartX, Y + 120), BoxWidth, 110, Color.White);

            if (CursorIndex < ListUnitPart.Count)
            {
                g.Draw(sprSpiritMenuHighlight, new Vector2(StartX + 4, Y + 124 + CursorIndex * 18), Color.White);

                if (CursorIndex < ListUnitPart.Count)
                {
                    TextHelper.DrawText(g, ListUnitPart[CursorIndex].Spirit.Description,
                        new Vector2(32, 3), Color.White);
                }
            }

            for (int S = 0; S < ListUnitPart.Count; S++)
            {
                if (ListUnitPart[S].Spirit.CanActivate)
                {
                    TextHelper.DrawText(g, ListUnitPart[S].Name,
                        new Vector2(StartX + 8, Y + 123 + S * 18), Color.White);
                }
                else
                {
                    TextHelper.DrawText(g, ListUnitPart[S].Name,
                        new Vector2(StartX + 8, Y + 123 + S * 18), Color.Gray);
                }
            }
        }
    }
}
