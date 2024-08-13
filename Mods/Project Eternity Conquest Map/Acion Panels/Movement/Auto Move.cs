using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAutoMove : ActionPanelConquest
    {
        private class SquadAutoMovement
        {
            public readonly UnitConquest Owner;
            public readonly int PlayerIndex;
            public readonly int SquadIndex;
            public readonly Vector3 StartPosition;
            public readonly Vector3 EndPosition;
            public Vector3 LastPosition;
            public Terrain LastTerrain;

            public SquadAutoMovement(int PlayerIndex, int SquadIndex, UnitConquest Owner, Terrain LastTerrain)
            {
                this.PlayerIndex = PlayerIndex;
                this.SquadIndex = SquadIndex;
                this.Owner = Owner;
                this.LastTerrain = LastTerrain;
                StartPosition = Owner.Position;
                EndPosition = StartPosition + Owner.Components.Speed;

                LastPosition = StartPosition;
            }
        }

        private const string PanelName = "AutoMove";

        private const double AnimationLengthInSeconds = 2;
        private const float Friction = 2;
        private const float Gravity = 1;

        private double TimeElapsed;
        private int LastSquadIndex;

        private List<SquadAutoMovement> ListSquadAutoMovement;

        public ActionPanelAutoMove(ConquestMap Map)
            : base(PanelName, Map, false)
        {
            ListSquadAutoMovement = new List<SquadAutoMovement>();
        }

        public ActionPanelAutoMove(ConquestMap Map, int PlayerIndex, int SquadIndex, UnitConquest SquadToMove)
            : base(PanelName, Map, false)
        {
            ListSquadAutoMovement = new List<SquadAutoMovement>();
            ListSquadAutoMovement.Add(new SquadAutoMovement(PlayerIndex, SquadIndex, SquadToMove, Map.GetTerrain(SquadToMove.Components)));
        }

        public override void OnSelect()
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                Player ActivePlayer = Map.ListPlayer[P];
                for (int S = 0; S < ActivePlayer.ListUnit.Count; S++)
                {
                    UnitConquest ActiveUnit = ActivePlayer.ListUnit[S];
                    if (ActiveUnit.Components.Speed != Vector3.Zero && ActiveUnit.HP > 0)
                    {
                        ListSquadAutoMovement.Add(new SquadAutoMovement(P, S, ActiveUnit, Map.GetTerrain(ActiveUnit.Components)));
                    }
                }
            }

            LastSquadIndex = ListSquadAutoMovement.Count;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ListSquadAutoMovement.Count > 0)
            {
                if (ListSquadAutoMovement.Count == 1)
                {
                    Map.CursorPosition = Map.CursorPositionVisible = ListSquadAutoMovement[0].Owner.Position;
                }
                else
                {
                    Map.CursorPosition = ListSquadAutoMovement[0].StartPosition;
                    Map.CursorPositionVisible = Map.CursorPosition;
                }

                float FrameProgress = (float)(gameTime.ElapsedGameTime.TotalSeconds / AnimationLengthInSeconds);

                for (int S = LastSquadIndex - 1; S >= 0; --S)
                {
                    LastSquadIndex = S;

                    if (ListSquadAutoMovement[S].Owner.HP == 0)
                    {
                        ListSquadAutoMovement[S].Owner.SetPosition(new Vector3((int)ListSquadAutoMovement[S].LastPosition.X,
                            (int)ListSquadAutoMovement[S].LastPosition.Y, (int)ListSquadAutoMovement[S].LastPosition.Z));
                        Map.LayerManager.UnitKilled(ListSquadAutoMovement[S].PlayerIndex);
                        ListSquadAutoMovement.RemoveAt(S);
                        return;
                    }

                    Vector3 NextPosition = ListSquadAutoMovement[S].LastPosition + ListSquadAutoMovement[S].Owner.Components.Speed * FrameProgress;
                    if (ListSquadAutoMovement[S].Owner.Components.IsOnGround)
                    {
                        NextPosition.Z = ListSquadAutoMovement[S].LastPosition.Z;
                    }

                    ListSquadAutoMovement[S].Owner.SetPosition(NextPosition);
                    Terrain NextTerrain = Map.GetTerrain(ListSquadAutoMovement[S].Owner.Position);

                    if (Math.Round(ListSquadAutoMovement[S].LastPosition.X) != Math.Round(ListSquadAutoMovement[S].Owner.Position.X)
                        || Math.Round(ListSquadAutoMovement[S].LastPosition.Y) != Math.Round(ListSquadAutoMovement[S].Owner.Position.Y)
                        || Math.Round(ListSquadAutoMovement[S].LastPosition.Z) != Math.Round(ListSquadAutoMovement[S].Owner.Position.Z))
                    {
                        Map.LayerManager.UnitMoved(ListSquadAutoMovement[S].PlayerIndex);
                    }

                    if (ListSquadAutoMovement[S].Owner.Position.Z < Map.LayerManager.ListLayer.Count && ListSquadAutoMovement[S].Owner.Components.Speed.Z <= 0
                        && IsOnGround(ListSquadAutoMovement[S]))
                    {
                        float NewSpeedX = ListSquadAutoMovement[S].Owner.Components.Speed.X;
                        float NewSpeedY = ListSquadAutoMovement[S].Owner.Components.Speed.Y;
                        float NewSpeedZ = ListSquadAutoMovement[S].Owner.Components.Speed.Z;

                        if (ListSquadAutoMovement[S].Owner.Components.IsStunned)
                        {
                            if (NewSpeedX > FrameProgress * Friction)
                                NewSpeedX -= FrameProgress * Friction;
                            else if (NewSpeedX < FrameProgress * -Friction)
                                NewSpeedX += FrameProgress * Friction;
                            else
                                NewSpeedX = 0;

                            if (NewSpeedY > FrameProgress * Friction)
                                NewSpeedY -= FrameProgress * Friction;
                            else if (NewSpeedY < FrameProgress * -Friction)
                                NewSpeedY += FrameProgress * Friction;
                            else
                                NewSpeedY = 0;
                        }
                        else
                        {
                            if (NewSpeedX > FrameProgress * Friction * 10)
                                NewSpeedX -= FrameProgress * Friction * 10;
                            else if (NewSpeedX < FrameProgress * -Friction * 10)
                                NewSpeedX += FrameProgress * Friction * 10;
                            else
                                NewSpeedX = 0;

                            if (NewSpeedY > FrameProgress * Friction * 10)
                                NewSpeedY -= FrameProgress * Friction * 10;
                            else if (NewSpeedY < FrameProgress * -Friction * 10)
                                NewSpeedY += FrameProgress * Friction * 10;
                            else
                                NewSpeedY = 0;
                        }

                        if (NewSpeedX == 0 && NewSpeedY == 0)
                        {
                            NewSpeedZ = 0;
                        }

                        ListSquadAutoMovement[S].Owner.Components.Speed = new Vector3(NewSpeedX, NewSpeedY, NewSpeedZ);

                        ListSquadAutoMovement[S].Owner.Components.IsOnGround = true;
                        if (NewSpeedX == 0 && NewSpeedY == 0 && NewSpeedZ == 0)
                        {
                            ListSquadAutoMovement[S].Owner.SetPosition(new Vector3((int)ListSquadAutoMovement[S].LastPosition.X,
                                (int)ListSquadAutoMovement[S].LastPosition.Y, (int)ListSquadAutoMovement[S].LastPosition.Z));
                            Map.LayerManager.UnitMoved(ListSquadAutoMovement[S].PlayerIndex);
                            ListSquadAutoMovement.RemoveAt(S);
                            return;
                        }
                    }
                    else
                    {
                        ListSquadAutoMovement[S].Owner.Components.Speed = new Vector3(ListSquadAutoMovement[S].Owner.Components.Speed.X, ListSquadAutoMovement[S].Owner.Components.Speed.Y, ListSquadAutoMovement[S].Owner.Components.Speed.Z - FrameProgress * Gravity * 32);

                        if (ListSquadAutoMovement[S].Owner.Components.Speed.Z < 0)
                        {
                            int LandedLayer = CheckLanding(ListSquadAutoMovement[S], FrameProgress);

                            if (LandedLayer >= 0)
                            {
                                if (ListSquadAutoMovement[S].Owner.Components.IsStunned)
                                {
                                    ListSquadAutoMovement[S].Owner.Components.Speed = new Vector3(ListSquadAutoMovement[S].Owner.Components.Speed.X, ListSquadAutoMovement[S].Owner.Components.Speed.Y, 0);
                                }
                                else
                                {
                                    ListSquadAutoMovement[S].Owner.Components.Speed = Vector3.Zero;
                                }
                                ListSquadAutoMovement[S].Owner.Components.CurrentTerrainIndex = Map.GetTerrain(new Vector3(ListSquadAutoMovement[S].Owner.Position.X, (int)ListSquadAutoMovement[S].Owner.Position.Y, LandedLayer)).TerrainTypeIndex;
                                ListSquadAutoMovement[S].Owner.SetPosition(new Vector3(ListSquadAutoMovement[S].Owner.Position.X, ListSquadAutoMovement[S].Owner.Position.Y, LandedLayer));
                                ListSquadAutoMovement[S].Owner.Components.IsOnGround = true;
                                Map.LayerManager.UnitMoved(ListSquadAutoMovement[S].PlayerIndex);
                            }
                        }

                        if (TimeElapsed > AnimationLengthInSeconds)
                        {
                            ListSquadAutoMovement.RemoveAt(S);
                            return;
                        }
                    }

                    if (CheckForCollisions(ListSquadAutoMovement[S], FrameProgress))
                    {
                        ListSquadAutoMovement[S].Owner.SetPosition(new Vector3((int)ListSquadAutoMovement[S].LastPosition.X,
                            (int)ListSquadAutoMovement[S].LastPosition.Y, (int)ListSquadAutoMovement[S].LastPosition.Z));
                        Map.LayerManager.UnitMoved(ListSquadAutoMovement[S].PlayerIndex);
                        //TODO: bounche against walls
                        ListSquadAutoMovement.RemoveAt(S);
                        return;
                    }

                    ListSquadAutoMovement[S].LastPosition = ListSquadAutoMovement[S].Owner.Position;
                    ListSquadAutoMovement[S].LastTerrain = NextTerrain;
                }

                LastSquadIndex = ListSquadAutoMovement.Count;
                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                RemoveFromPanelList(this);
            }
        }

        private bool CheckForCollisions(SquadAutoMovement ActiveUnit, float FrameProgress)
        {
            Vector3 NextPosition = ActiveUnit.Owner.Position;

            if (NextPosition.X < 0 || NextPosition.X >= Map.MapSize.X
                || NextPosition.Y < 0 || NextPosition.Y >= Map.MapSize.Y
                || NextPosition.Z < 0 || NextPosition.Z >= Map.LayerManager.ListLayer.Count)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelFallToDeath(Map, ActiveUnit.PlayerIndex, ActiveUnit.SquadIndex));
                return true;
            }

            Terrain NextTerrain = Map.GetTerrain(NextPosition);

            if (NextTerrain == ActiveUnit.LastTerrain)
            {
                return false;
            }

            if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
            {
                throw new NotImplementedException();
                ActiveUnit.Owner.Components.Speed = Vector3.Zero;
                return true;
            }

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                int SquadIndex = Map.CheckForSquadAtPosition(P, NextPosition, Vector3.Zero);
                if (SquadIndex >= 0 && P != ActiveUnit.PlayerIndex && SquadIndex != ActiveUnit.SquadIndex)
                {
                    throw new NotImplementedException();
                    ActiveUnit.Owner.Components.Speed = Vector3.Zero;
                    return true;
                }
            }

            return false;
        }

        private int CheckLanding(SquadAutoMovement ActiveUnit, float FrameProgress)
        {
            Vector3 NextPosition = ActiveUnit.Owner.Position;
            float LastZ = ActiveUnit.LastPosition.Z;

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
            {
                Terrain NextTerrain = Map.GetTerrain(new Vector3(ActiveUnit.Owner.Position.X, ActiveUnit.Owner.Position.Y, L));

                if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
                {
                    float GroundZValue = NextTerrain.WorldPosition.Z;

                    //Not touching yet but will touch next frame
                    if (LastZ > GroundZValue && NextPosition.Z < GroundZValue)
                    {
                        return L;
                    }
                }
            }

            return -1;
        }

        private bool IsOnGround(SquadAutoMovement ActiveUnit)
        {
            return ActiveUnit.Owner.Components.CurrentTerrainIndex == UnitStats.TerrainLandIndex
                && Map.GetTerrain(ActiveUnit.Owner.Components).TerrainTypeIndex != UnitStats.TerrainVoidIndex;
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAutoMove(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
