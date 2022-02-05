using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPTargeted : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackMAPTargeted";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;
        public List<MovementAlgorithmTile> ListCursorTarget;
        private BattlePreviewer BattlePreview;

        Stack<Tuple<int, int>> ListMAPAttackTarget;

        private Squad TemptativeTargetSquad;
        private int TemptativeTargetSquadIndex;
        private int TemptativeTargetPlayerIndex;

        public ActionPanelAttackMAPTargeted(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPTargeted(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            ListAttackChoice = new List<Vector3>();
            ListMAPAttackTarget = new Stack<Tuple<int, int>>();
            ListCursorTarget = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            ListAttackChoice = Map.GetAttackChoice(ActiveSquad.CurrentLeader, ActiveSquad.Position);
            ListMAPAttackTarget = Map.GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.FriendlyFire, ListAttackChoice);

            ListAttackTerrain = new List<MovementAlgorithmTile>();
            foreach (Vector3 ActiveTerrain in ListAttackChoice)
            {
                ListAttackTerrain.Add(Map.GetTerrain(ActiveTerrain.X, ActiveTerrain.Y, (int)ActiveTerrain.Z));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            ListCursorTarget.Clear();
            ListCursorTarget.Add(Map.GetTerrain(Map.CursorPosition.X, Map.CursorPosition.Y, (int)Map.CursorPosition.Z));
            Map.LayerManager.AddDrawablePoints(ListCursorTarget, Color.DarkRed);

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListAttackChoice.Contains(Map.CursorPosition))
                {
                    ListAttackChoice.Clear();
                    for (int X = 0; X < CurrentAttack.MAPAttributes.ListChoice.Count; X++)
                    {
                        for (int Y = 0; Y < CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                        {
                            if (CurrentAttack.MAPAttributes.ListChoice[X][Y])
                            {
                                ListAttackChoice.Add(new Vector3(Map.CursorPosition.X + X - CurrentAttack.MAPAttributes.Width,
                                                       Map.CursorPosition.Y + Y - CurrentAttack.MAPAttributes.Height, Map.CursorPosition.Z));
                            }
                        }
                    }

                    Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints, ListAttackChoice);
                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }

            if (ActiveInputManager.InputLButtonPressed())
            {
                int PlayerIndex = 0;
                int SquadIndex = 0;
                if (TemptativeTargetSquad != null)
                {
                    SquadIndex = TemptativeTargetSquadIndex;
                    PlayerIndex = TemptativeTargetPlayerIndex;
                }

                int StartPlayerIndex = PlayerIndex;
                int StartSquadIndex = SquadIndex;

                do
                {
                    if (++SquadIndex >= Map.ListPlayer[PlayerIndex].ListSquad.Count)
                    {
                        SquadIndex = 0;

                        if (++PlayerIndex >= Map.ListPlayer.Count)
                        {
                            PlayerIndex = 0;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListSquad[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex || StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                if (TemptativeTargetSquad.X < Map.CameraPosition.X || TemptativeTargetSquad.Y < Map.CameraPosition.Y ||
                    TemptativeTargetSquad.X >= Map.CameraPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.Y >= Map.CameraPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, TemptativeTargetSquad.Position));
                }
            }
            else if (ActiveInputManager.InputRButtonPressed())
            {
                int PlayerIndex = 0;
                int SquadIndex = 0;
                if (TemptativeTargetSquad != null)
                {
                    SquadIndex = TemptativeTargetSquadIndex;
                    PlayerIndex = TemptativeTargetPlayerIndex;
                }

                int StartPlayerIndex = PlayerIndex;
                int StartSquadIndex = SquadIndex;

                do
                {
                    if (--SquadIndex < 0)
                    {
                        SquadIndex = Map.ListPlayer[PlayerIndex].ListSquad.Count - 1;

                        if (--PlayerIndex < 0)
                        {
                            PlayerIndex = Map.ListPlayer.Count - 1;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListSquad[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex && StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                if (TemptativeTargetSquad.X < Map.CameraPosition.X || TemptativeTargetSquad.Y < Map.CameraPosition.Y ||
                    TemptativeTargetSquad.X >= Map.CameraPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.Y >= Map.CameraPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, TemptativeTargetSquad.Position));
                }
            }
            else
            {
                Map.CursorControl(ActiveInputManager);//Move the cursor
                BattlePreview.UpdateUnitDisplay();
            }
        }

        private bool ContainsSquad(int PlayerIndex, int SquadIndex)
        {
            foreach (Tuple<int, int> ActiveTarget in ListMAPAttackTarget)
            {
                if (ActiveTarget.Item1 == PlayerIndex && ActiveTarget.Item2 == SquadIndex && Map.ListPlayer[PlayerIndex].ListSquad[SquadIndex].CurrentLeader != null)
                {
                    return true;
                }
            }

            return false;
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
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain.X, NewTerrain.Y, (int)NewTerrain.Z));
            }

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendFloat(ListAttackChoice[A].X);
                BW.AppendFloat(ListAttackChoice[A].Y);
                BW.AppendInt32((int)ListAttackChoice[A].Z);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackMAPTargeted(Map);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
