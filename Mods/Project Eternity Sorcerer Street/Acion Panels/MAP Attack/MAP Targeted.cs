using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAttackMAPTargeted : ActionPanelSorcererStreet
    {
        private const string PanelName = "AttackMAPTargeted";

        private TerrainSorcererStreet ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        public int RangeMaximum;
        public MAPAttackAttributes MAPAttributes;
        public ExplosionOptions ExplosionOption;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;
        public List<MovementAlgorithmTile> ListCursorTarget;
        public List<MovementAlgorithmTile> ListExplosionTerrain;

        Stack<Tuple<int, int>> ListMAPAttackTarget;

        private TerrainSorcererStreet TemptativeTargetSquad;
        private int TemptativeTargetSquadIndex;
        private int TemptativeTargetPlayerIndex;

        public ActionPanelAttackMAPTargeted(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPTargeted(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            ListAttackChoice = new List<Vector3>();
            ListMAPAttackTarget = new Stack<Tuple<int, int>>();
            ListCursorTarget = new List<MovementAlgorithmTile>();
            ListExplosionTerrain = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ListAttackTerrain = Map.GetAttackChoice(ActiveSquad.DefendingCreature.GamePiece, RangeMaximum);
            ListMAPAttackTarget = Map.GetEnemies(MAPAttributes.FriendlyFire, ListAttackTerrain);

            ListAttackChoice = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTerrain in ListAttackTerrain)
            {
                ListAttackChoice.Add(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.WorldPosition.Z));
            }
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
                if (ListAttackChoice.Contains(Map.CursorPosition) || MAPAttributes.Delay > 0  || ExplosionOption.ExplosionRadius > 0)
                {
                    ListAttackTerrain.Clear();

                    for (int X = 0; X < MAPAttributes.ListChoice.Count; X++)
                    {
                        for (int Y = 0; Y < MAPAttributes.ListChoice[X].Count; Y++)
                        {
                            if (MAPAttributes.ListChoice[X][Y])
                            {
                                ListAttackTerrain.Add(Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[(int)Map.CursorPosition.X + X - MAPAttributes.Width,
                                                       (int)Map.CursorPosition.Y + Y - MAPAttributes.Height]);
                            }
                        }
                    }

                    Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, MAPAttributes, ListMVHoverPoints, ListAttackTerrain);
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
                    if (++SquadIndex >= Map.ListPlayer[PlayerIndex].ListSummonedCreature.Count)
                    {
                        SquadIndex = 0;

                        if (++PlayerIndex >= Map.ListPlayer.Count)
                        {
                            PlayerIndex = 0;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListSummonedCreature[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex || StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.WorldPosition;
                Map.CursorPositionVisible = Map.CursorPosition;
                UpdateExplosionPositions();

                if (TemptativeTargetSquad.WorldPosition.X < Map.Camera2DPosition.X || TemptativeTargetSquad.WorldPosition.Y < Map.Camera2DPosition.Y ||
                    TemptativeTargetSquad.WorldPosition.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.WorldPosition.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, TemptativeTargetSquad.WorldPosition));
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
                        SquadIndex = Map.ListPlayer[PlayerIndex].ListSummonedCreature.Count - 1;

                        if (--PlayerIndex < 0)
                        {
                            PlayerIndex = Map.ListPlayer.Count - 1;
                        }
                    }

                    if (ContainsSquad(PlayerIndex, SquadIndex))
                    {
                        TemptativeTargetSquad = Map.ListPlayer[PlayerIndex].ListSummonedCreature[SquadIndex];
                        TemptativeTargetSquadIndex = SquadIndex;
                        TemptativeTargetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                while (StartSquadIndex != SquadIndex && StartPlayerIndex != PlayerIndex);

                Map.CursorPosition = TemptativeTargetSquad.WorldPosition;
                Map.CursorPositionVisible = Map.CursorPosition;
                UpdateExplosionPositions();

                if (TemptativeTargetSquad.WorldPosition.X < Map.Camera2DPosition.X || TemptativeTargetSquad.WorldPosition.Y < Map.Camera2DPosition.Y ||
                    TemptativeTargetSquad.WorldPosition.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || TemptativeTargetSquad.WorldPosition.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, TemptativeTargetSquad.WorldPosition));
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
            if (ExplosionOption.ExplosionRadius > 0)
            {
                ListExplosionTerrain.Clear();
                float X = Map.CursorPosition.X;
                float Y = Map.CursorPosition.Y;
                float Z = Map.CursorPosition.Z;

                for (float OffsetX = -ExplosionOption.ExplosionRadius; OffsetX < ExplosionOption.ExplosionRadius; ++OffsetX)
                {
                    for (float OffsetY = -ExplosionOption.ExplosionRadius; OffsetY < ExplosionOption.ExplosionRadius; ++OffsetY)
                    {
                        if (Math.Abs(OffsetX) + Math.Abs(OffsetY) < ExplosionOption.ExplosionRadius
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
                if (ActiveTarget.Item1 == PlayerIndex && ActiveTarget.Item2 == SquadIndex && Map.ListPlayer[PlayerIndex].ListSummonedCreature[SquadIndex].DefendingCreature.GamePiece.IsActive)
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
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];

            int AttackChoiceCount = BR.ReadInt32();
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
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
