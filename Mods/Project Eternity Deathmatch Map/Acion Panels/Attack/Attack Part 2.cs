﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using System;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPart2 : ActionPanelDeathmatch
    {
        private const string PanelName = "Attack2";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        private Squad ActiveSquad;
        public List<MovementAlgorithmTile> ListAttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPart2(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackPart2(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            ListAttackChoice = Map.GetAttackChoice(ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack.RangeMaximum);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));

            if (ActiveInputManager.InputConfirmPressed())
            {
                int TargetSelect = 0;
                //Verify if the cursor is over one of the possible MV position.
                while ((Math.Floor(Map.CursorPosition.X / Map.TileSize.X) != ListAttackChoice[TargetSelect].GridPosition.X || Math.Floor(Map.CursorPosition.Y / Map.TileSize.Y) != ListAttackChoice[TargetSelect].GridPosition.Y)
                    && ++TargetSelect < ListAttackChoice.Count) ;

                //If nothing was found.
                if (TargetSelect >= ListAttackChoice.Count)
                    return;

                Map.TargetSquadIndex = -1;

                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    //Find if a Unit is under the cursor.
                    TargetSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                    //If one was found.
                    if (TargetSelect >= 0)
                    {
                        if (Map.ListPlayer[ActivePlayerIndex].TeamIndex != Map.ListPlayer[P].TeamIndex)//If it's an ennemy.
                        {
                            ActiveSquad.CurrentLeader.CurrentAttack.UpdateAttack(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, Map.CursorPosition, Map.ListPlayer[P].TeamIndex,
                                ActiveSquad.ArrayMapSize, Map.TileSize, Map.ListPlayer[P].ListSquad[TargetSelect].CurrentTerrainIndex, ActiveSquad.CanMove);

                            if (!ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                            {
                                Map.sndDeny.Play();
                                return;
                            }

                            Map.PrepareSquadsForBattle(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, P, TargetSelect);

                            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
                            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, P, TargetSelect);

                            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                            TargetSquadSupport.PrepareDefenceSupport(Map, P, Map.ListPlayer[P].ListSquad[TargetSelect]);

                            Map.ComputeTargetPlayerDefence(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, P, TargetSelect, TargetSquadSupport);

                            break;
                        }
                    }
                }

                Map.sndConfirm.Play();
            }
            else
            {
                bool CursorMoved = Map.CursorControl(ActiveInputManager);
                if (CursorMoved)
                {
                    BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);

                    if (Map.OnlineClient != null)
                    {
                        Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                    }
                }
                BattlePreview.UpdateUnitDisplay();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            string ActiveSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveSquadAttackName))
            {
                foreach (Attack ActiveAttack in ActiveSquad.CurrentLeader.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveSquadAttackName)
                    {
                        ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }
            int AttackChoiceCount = BR.ReadInt32();
            ListAttackChoice = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackChoice.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
            }

            bool IsBattlePreviewOpen = BR.ReadBoolean();
            if (IsBattlePreviewOpen)
            {
                int PlayerIndex = BR.ReadInt32();
                int SquadIndex = BR.ReadInt32();
                BattlePreview = new BattlePreviewer(Map, PlayerIndex, SquadIndex, null);
            }

            Map.CursorPosition = new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
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
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendInt32(ListAttackChoice[A].GridPosition.X);
                BW.AppendInt32(ListAttackChoice[A].GridPosition.Y);
                BW.AppendInt32(ListAttackChoice[A].LayerIndex);
            }

            BW.AppendBoolean(BattlePreview != null);
            if (BattlePreview != null)
            {
                BW.AppendInt32(BattlePreview.PlayerIndex);
                BW.AppendInt32(BattlePreview.SquadIndex);
            }

            BW.AppendInt32((int)Map.CursorPosition.X);
            BW.AppendInt32((int)Map.CursorPosition.Y);
            BW.AppendInt32((int)Map.CursorPosition.Z);
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
            return new ActionPanelAttackPart2(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
