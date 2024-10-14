using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerHumanStep : ActionPanelDeathmatch
    {
        private const string PanelName = "PlayerHumanStep";

        private BattlePreviewer BattlePreview;

        public ActionPanelPlayerHumanStep(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            bool CursorMoved = Map.CursorControl(ActiveInputManager);
            if (CursorMoved)
            {
                BattlePreview = null;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            //Loop through the players to find a Unit to control.
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                if (CursorSelect >= 0)
                {
                    if (BattlePreview == null)
                    {
                        BattlePreview = new BattlePreviewer(Map, P, CursorSelect, null);
                    }
                    BattlePreview.UpdateUnitDisplay();
                }
            }

            if (ActiveInputManager.InputCancelPressed())
            {
                AddToPanelListAndSelect(Map.BattleMapMenu);

                Map.ActiveSquadIndex = -1;
                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                Map.ActiveSquadIndex = -1;
                Map.TargetSquadIndex = -1;

                ActionPanelMainMenu.AddIfUsable(Map, this);
                ActionPanelMoveVehicle.AddIfUsable(Map, this);

                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCommand1Pressed())
            {
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                    if (CursorSelect >= 0)
                    {
                        List<ActionPanel> DicOptionalPanel = Map.ListPlayer[P].ListSquad[CursorSelect].CurrentLeader.OnInputPressed(P, Map.ListPlayer[P].ListSquad[CursorSelect], ListActionMenuChoice);
                        foreach (ActionPanel OptionalPanel in DicOptionalPanel)
                        {
                            AddChoiceToCurrentPanel(OptionalPanel);
                        }
                    }
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            bool IsBattlePreviewOpen = BR.ReadBoolean();
            if (IsBattlePreviewOpen)
            {
                int PlayerIndex = BR.ReadInt32();
                int SquadIndex = BR.ReadInt32();
                BattlePreview = new BattlePreviewer(Map, PlayerIndex, SquadIndex, null);
            }
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            ByteReader BR = new ByteReader(ArrayUpdateData);
            Map.CursorPosition.X = BR.ReadFloat();
            Map.CursorPosition.Y = BR.ReadFloat();
            BR.Clear();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendBoolean(BattlePreview != null);
            if (BattlePreview != null)
            {
                BW.AppendInt32(BattlePreview.PlayerIndex);
                BW.AppendInt32(BattlePreview.SquadIndex);
            }
        }

        public override byte[] DoWriteUpdate()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);

            byte[] ArrayUpdateData = BW.GetBytes();
            BW.ClearWriteBuffer();

            return ArrayUpdateData;
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerHumanStep(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //If the action menu is open.
            if (BattlePreview != null)
            {
                BattlePreview.DrawDisplayUnit(g);
            }
        }
    }
}
