using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAutoMove : ActionPanelDeathmatch
    {
        private class SquadAutoMovement
        {
            public readonly Squad Owner;
            public readonly int PlayerIndex;
            public readonly int SquadIndex;
            public readonly Vector3 StartPosition;
            public readonly Vector3 EndPosition;
            public Vector3 LastPosition;
            public Terrain LastTerrain;

            public SquadAutoMovement(int PlayerIndex, int SquadIndex, Squad Owner, Terrain LastTerrain)
            {
                this.PlayerIndex = PlayerIndex;
                this.SquadIndex = SquadIndex;
                this.Owner = Owner;
                this.LastTerrain = LastTerrain;
                StartPosition = Owner.Position;
                EndPosition = StartPosition + Owner.Speed;

                LastPosition = StartPosition;
            }
        }

        private const string PanelName = "AutoMove";

        private const double AnimationLengthInSeconds = 2;
        private const float Friction = 2;
        private const float Gravity = 2;

        private double TimeElapsed;
        private int LastSquadIndex;
        private double Progress;

        private List<SquadAutoMovement> ListSquadAutoMovement;

        public ActionPanelAutoMove(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListSquadAutoMovement = new List<SquadAutoMovement>();
        }

        public ActionPanelAutoMove(DeathmatchMap Map, Squad SquadToMove, Vector3 NextPosition)
            : base(PanelName, Map, false)
        {
            ListSquadAutoMovement = new List<SquadAutoMovement>();
        }

        public override void OnSelect()
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                Player ActivePlayer = Map.ListPlayer[P];
                for (int S = 0; S < ActivePlayer.ListSquad.Count; S++)
                {
                    Squad ActiveSquad = ActivePlayer.ListSquad[S];
                    if (ActiveSquad.Speed != Vector3.Zero)
                    {
                        ListSquadAutoMovement.Add(new SquadAutoMovement(P, S, ActiveSquad, Map.GetTerrain(ActiveSquad)));
                    }
                }
            }

            LastSquadIndex = ListSquadAutoMovement.Count;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (TimeElapsed < AnimationLengthInSeconds)
            {
                Progress = TimeElapsed / AnimationLengthInSeconds;
                float FrameProgress = (float)(gameTime.ElapsedGameTime.TotalSeconds / AnimationLengthInSeconds);

                for (int S = LastSquadIndex - 1; S >= 0; --S)
                {
                    LastSquadIndex = S;

                    if (ListSquadAutoMovement[S].Owner.CurrentLeader == null)
                    {
                        ListSquadAutoMovement.RemoveAt(S);
                        return;
                    }

                    if (IsOnGround(ListSquadAutoMovement[S]))
                    {
                        float NewSpeedX = ListSquadAutoMovement[S].Owner.Speed.X;
                        float NewSpeedY = ListSquadAutoMovement[S].Owner.Speed.Y;
                        float NewSpeedZ = ListSquadAutoMovement[S].Owner.Speed.Z;

                        if (NewSpeedX > Friction)
                            NewSpeedX -= FrameProgress * Friction;
                        else if (NewSpeedX < -Friction)
                            NewSpeedX += FrameProgress * Friction;
                        else
                            NewSpeedX = 0;

                        if (NewSpeedY > Friction)
                            NewSpeedY -= FrameProgress * Friction;
                        else if (NewSpeedY < -Friction)
                            NewSpeedY += FrameProgress * Friction;
                        else
                            NewSpeedY = 0;

                        NewSpeedZ -= FrameProgress * Gravity;

                        ListSquadAutoMovement[S].Owner.Speed = new Vector3(NewSpeedX, NewSpeedY, NewSpeedZ);
                    }
                    else
                    {
                        int LandedLayer = CheckLanding(ListSquadAutoMovement[S]);

                        if (LandedLayer >= 0)
                        {
                            ListSquadAutoMovement[S].Owner.Speed = new Vector3(ListSquadAutoMovement[S].Owner.Speed.X, ListSquadAutoMovement[S].Owner.Speed.Y, 0);
                            ListSquadAutoMovement[S].Owner.IsFlying = false;
                            ListSquadAutoMovement[S].Owner.CurrentMovement = UnitStats.TerrainLand;
                            ListSquadAutoMovement[S].Owner.SetPosition(new Vector3(ListSquadAutoMovement[S].Owner.Position.X, ListSquadAutoMovement[S].Owner.Position.Y, LandedLayer));
                        }
                    }

                    if (CheckForCollisions(ListSquadAutoMovement[S]))
                    {
                        ListSquadAutoMovement.RemoveAt(S);
                        return;
                    }

                    ListSquadAutoMovement[S].Owner.SetPosition(ListSquadAutoMovement[S].LastPosition);
                }

                LastSquadIndex = ListSquadAutoMovement.Count;
                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                RemoveFromPanelList(this);
            }
        }

        private bool CheckForCollisions(SquadAutoMovement ActiveSquad)
        {
            Vector3 NextPostion = ActiveSquad.StartPosition + ActiveSquad.Owner.Speed * (float)Progress;

            if (NextPostion.X < 0 || NextPostion.X >= Map.MapSize.X
                || NextPostion.Y < 0 || NextPostion.Y >= Map.MapSize.Y
                || NextPostion.Z < 0 || NextPostion.Z >= Map.LayerManager.ListLayer.Count)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelFallToDeath(Map, ActiveSquad.PlayerIndex, ActiveSquad.SquadIndex));
                return true;
            }

            Terrain NextTerrain = Map.GetTerrain(NextPostion.X, NextPostion.Y, (int)NextPostion.Z);

            if (NextTerrain == ActiveSquad.LastTerrain)
            {
                return false;
            }

            if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
            {
                NonDemoScreen NonDemoScreen = new NonDemoScreen(Map);
                Map.PushScreen(NonDemoScreen);
                NonDemoScreen.InitNonDemo(ActiveSquad.PlayerIndex, ActiveSquad.SquadIndex, 500);
                ActiveSquad.Owner.Speed = Vector3.Zero;
                return true;
            }

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                int SquadIndex = Map.CheckForSquadAtPosition(P, NextPostion, Vector3.Zero);
                if (SquadIndex >= 0)
                {
                    NonDemoScreen NonDemoScreen = new NonDemoScreen(Map);
                    Map.PushScreen(NonDemoScreen);
                    NonDemoScreen.InitNonDemo(ActiveSquad.PlayerIndex, ActiveSquad.SquadIndex, 500);
                    ActiveSquad.Owner.Speed = Vector3.Zero;
                    return true;
                }
            }

            ActiveSquad.LastPosition = NextPostion;
            ActiveSquad.LastTerrain = NextTerrain;

            return false;
        }

        private int CheckLanding(SquadAutoMovement ActiveSquad)
        {
            for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
            {
                Terrain NextTerrain = Map.LayerManager.ListLayer[L].ArrayTerrain[(int)ActiveSquad.Owner.Position.X, (int)ActiveSquad.Owner.Position.Y];

                if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
                {
                    float CurrentZ = ActiveSquad.Owner.Position.Z;
                    float NextZ = ActiveSquad.LastPosition.Z;

                    float ZValue = L + NextTerrain.Position.Z;

                    //Not touching yet but will touch next frame
                    if (ZValue <= CurrentZ && ZValue >= NextZ)
                    {
                        return L;
                    }
                }
            }

            return -1;
        }

        private bool IsOnGround(SquadAutoMovement ActiveSquad)
        {
            return ActiveSquad.Owner.CurrentMovement == UnitStats.TerrainLand;
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
