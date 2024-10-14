using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttackMAPTargeted : ActionPanelConquest
    {
        private const string PanelName = "AttackMAPTargeted";

        private UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;
        public List<MovementAlgorithmTile> ListCursorTarget;
        public List<MovementAlgorithmTile> ListExplosionTerrain;

        Stack<Tuple<int, int>> ListMAPAttackTarget;

        private UnitConquest TemptativeTargetSquad;
        private int TemptativeTargetSquadIndex;
        private int TemptativeTargetPlayerIndex;

        public ActionPanelAttackMAPTargeted(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPTargeted(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            ListAttackChoice = new List<Vector3>();
            ListMAPAttackTarget = new Stack<Tuple<int, int>>();
            ListCursorTarget = new List<MovementAlgorithmTile>();
            ListExplosionTerrain = new List<MovementAlgorithmTile>();

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void OnSelect()
        {
            ListAttackTerrain = GetAttackChoice(ActiveUnit, ActiveUnit.CurrentAttack.RangeMaximum);
            ListMAPAttackTarget = Map.GetEnemies(CurrentAttack.MAPAttributes.FriendlyFire, ListAttackTerrain);

            ListAttackChoice = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTerrain in ListAttackTerrain)
            {
                ListAttackChoice.Add(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.WorldPosition.Z));
            }
        }

        public List<MovementAlgorithmTile> GetAttackChoice(UnitConquest ActiveUnit, int RangeMaximum)
        {
            ConquestMap ActiveMap = Map;
            if (Map.ActivePlatform != null)
            {
                ActiveMap = (ConquestMap)Map.ActivePlatform.Map;
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Map.Pathfinder.FindPath(Map.GetAllTerrain(ActiveUnit.Components, ActiveMap), ActiveUnit.Components, ActiveUnit.UnitStat, RangeMaximum, true);

            List<MovementAlgorithmTile> MovementChoice = new List<MovementAlgorithmTile>();

            for (int i = 0; i < ListAllNode.Count; i++)
            {
                ListAllNode[i].ParentTemp = null;//Unset parents
                ListAllNode[i].MovementCost = 0;

                if (ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainWallIndex || ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                MovementChoice.Add(ListAllNode[i]);
            }

            return MovementChoice;
        }
        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            ListCursorTarget.Clear();
            ListCursorTarget.Add(Map.GetTerrain(Map.CursorPosition));
            Map.LayerManager.AddDrawablePoints(ListCursorTarget, Color.DarkRed);
            Map.LayerManager.AddDrawablePoints(ListExplosionTerrain, Color.FromNonPremultiplied(109, 0, 0, 140));

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListAttackChoice.Contains(Map.CursorPosition) || CurrentAttack.MAPAttributes.Delay > 0  || CurrentAttack.ExplosionOption.ExplosionRadius > 0)
                {
                    ListAttackTerrain.Clear();

                    for (int X = 0; X < CurrentAttack.MAPAttributes.ListChoice.Count; X++)
                    {
                        for (int Y = 0; Y < CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                        {
                            if (CurrentAttack.MAPAttributes.ListChoice[X][Y])
                            {
                                ListAttackTerrain.Add(Map.GetMovementTile((int)Map.CursorPosition.X + X - CurrentAttack.MAPAttributes.Width,
                                                       (int)Map.CursorPosition.Y + Y - CurrentAttack.MAPAttributes.Height, (int)Map.CursorPosition.Z));
                            }
                        }
                    }

                    ActionPanelUseMAPAttack.SelectMAPEnemies(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints, ListAttackTerrain);
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
                    if (++SquadIndex >= Map.ListPlayer[PlayerIndex].ListUnit.Count)
                    {
                        SquadIndex = 0;

                        if (++PlayerIndex >= Map.ListPlayer.Count)
                        {
                            PlayerIndex = 0;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListUnit[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex || StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;
                UpdateExplosionPositions();

                if (TemptativeTargetSquad.X < Map.Camera2DPosition.X || TemptativeTargetSquad.Y < Map.Camera2DPosition.Y ||
                    TemptativeTargetSquad.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
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
                        SquadIndex = Map.ListPlayer[PlayerIndex].ListUnit.Count - 1;

                        if (--PlayerIndex < 0)
                        {
                            PlayerIndex = Map.ListPlayer.Count - 1;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListUnit[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex && StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;
                UpdateExplosionPositions();

                if (TemptativeTargetSquad.X < Map.Camera2DPosition.X || TemptativeTargetSquad.Y < Map.Camera2DPosition.Y ||
                    TemptativeTargetSquad.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, TemptativeTargetSquad.Position));
                }
            }
            else
            {
                if (Map.CursorControl(ActiveInputManager))
                {
                    UpdateExplosionPositions();
                }
            }
        }

        private void UpdateExplosionPositions()
        {
            if (CurrentAttack.ExplosionOption.ExplosionRadius > 0)
            {
                ListExplosionTerrain.Clear();
                float X = Map.CursorPosition.X;
                float Y = Map.CursorPosition.Y;
                float Z = Map.CursorPosition.Z;

                for (float OffsetX = -CurrentAttack.ExplosionOption.ExplosionRadius; OffsetX < CurrentAttack.ExplosionOption.ExplosionRadius; ++OffsetX)
                {
                    for (float OffsetY = -CurrentAttack.ExplosionOption.ExplosionRadius; OffsetY < CurrentAttack.ExplosionOption.ExplosionRadius; ++OffsetY)
                    {
                        if (Math.Abs(OffsetX) + Math.Abs(OffsetY) < CurrentAttack.ExplosionOption.ExplosionRadius
                            && X + OffsetX < Map.MapSize.X && Y + OffsetY < Map.MapSize.Y && X + OffsetX > 0 && Y + OffsetY > 0)
                        {
                            ListExplosionTerrain.Add(Map.GetTerrain(new Vector3(X + OffsetX, Y + OffsetY, Z)));
                        }
                    }
                }
            }
        }

        private bool ContainsSquad(int PlayerIndex, int SquadIndex)
        {
            foreach (Tuple<int, int> ActiveTarget in ListMAPAttackTarget)
            {
                if (ActiveTarget.Item1 == PlayerIndex && ActiveTarget.Item2 == SquadIndex && Map.ListPlayer[PlayerIndex].ListUnit[SquadIndex].HP > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            string ActiveUnitAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveUnitAttackName))
            {
                foreach (Attack ActiveAttack in ActiveUnit.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveUnitAttackName)
                    {
                        ActiveUnit.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }
            int AttackChoiceCount = BR.ReadInt32();
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain));
            }

            CurrentAttack = ActiveUnit.CurrentAttack;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendString(ActiveUnit.ItemName);
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
        }
    }
}
