﻿using System;
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
    public class ActionPanelUpdatePERAttacks : ActionPanelConquest
    {
        private class PERAttackMovement
        {
            public readonly PERAttack Owner;
            public readonly Vector3 StartPosition;
            public readonly Vector3 EndPosition;
            public readonly Vector3 Movement;
            public Vector3 LastRealPosition;
            public Terrain LastTerrain;
            public readonly List<Terrain> ListCrossedTerrain;

            public PERAttackMovement(PERAttack Owner, Vector3 StartPosition, Vector3 EndPosition, Terrain CurrentTerrain)
            {
                this.Owner = Owner;
                this.StartPosition = StartPosition;
                this.EndPosition = EndPosition;
                LastRealPosition = StartPosition;
                LastTerrain = CurrentTerrain;
                Movement = EndPosition - StartPosition;
                ListCrossedTerrain = Owner.GetCrossedTerrain(EndPosition);
            }
        }

        private const string PanelName = "UpdatePERAttacks";

        private const double AnimationLengthInSeconds = 2;
        private double TimeElapsed;
        private float MaxInclineDeviationAllowed = 0.1f;

        private List<PERAttackMovement> ListPERAttackToUpdate;

        public ActionPanelUpdatePERAttacks(ConquestMap Map)
            : base(PanelName, Map)
        {
            TimeElapsed = 0;
            ListPERAttackToUpdate = new List<PERAttackMovement>();
        }

        public ActionPanelUpdatePERAttacks(ConquestMap Map, List<PERAttack> ListNewPERAttack)
            : base(PanelName, Map)
        {
            TimeElapsed = 0;
            ListPERAttackToUpdate = new List<PERAttackMovement>();
            foreach (PERAttack ActivePERAttack in ListNewPERAttack)
            {
                Terrain CurrentTerrain = Map.GetTerrain(ActivePERAttack.Position);
                Vector3 NextPosition = ActivePERAttack.Position + ActivePERAttack.Speed;
                ListPERAttackToUpdate.Add(new PERAttackMovement(ActivePERAttack, ActivePERAttack.Position, NextPosition, CurrentTerrain));
            }
        }

        public void Add(List<PERAttack> ListNewPERAttack)
        {
            foreach (PERAttack ActivePERAttack in ListNewPERAttack)
            {
                Terrain CurrentTerrain = Map.GetTerrain(ActivePERAttack.Position);
                Vector3 NextPosition = ActivePERAttack.Position + ActivePERAttack.Speed;
                ListPERAttackToUpdate.Add(new PERAttackMovement(ActivePERAttack, ActivePERAttack.Position, NextPosition, CurrentTerrain));
            }
        }

        public override void OnSelect()
        {
            if (ListPERAttackToUpdate.Count == 0)
            {
                for (int A = Map.ListPERAttack.Count - 1; A >= 0; --A)
                {
                    PERAttack ActiveAttack = Map.ListPERAttack[A];

                    if (ActiveAttack.PlayerIndex == Map.ActivePlayerIndex)
                    {
                        --ActiveAttack.Lifetime;

                        if (ActiveAttack.Lifetime <= 0)
                        {
                            Map.ListPERAttack.RemoveAt(A);
                            SetAttackContext(ActiveAttack, ActiveAttack.Owner, Vector3.Normalize(ActiveAttack.Speed), ActiveAttack.Position);
                            ActiveAttack.UpdateSkills(AttackPERRequirement.OnDeath);
                        }
                        else
                        {
                            Terrain CurrentTerrain = Map.GetTerrain(ActiveAttack.Position);
                            Vector3 NextPosition = ActiveAttack.Position + ActiveAttack.Speed;
                            ListPERAttackToUpdate.Add(new PERAttackMovement(ActiveAttack, ActiveAttack.Position, NextPosition, CurrentTerrain));
                        }
                    }
                }
            }

            if (ListPERAttackToUpdate.Count == 0)
            {
                RemoveFromPanelList(this);
            }
        }

        public void SetAttackContext(PERAttack ActiveAttackBox, UnitConquest AttackOwner, Vector3 Angle, Vector3 Position)
        {
            Map.GlobalBattleParams.GlobalAttackContext.Owner = AttackOwner;
            Map.GlobalBattleParams.GlobalAttackContext.OwnerProjectile = ActiveAttackBox;
            Map.GlobalBattleParams.GlobalAttackContext.OwnerSandbox = Map;
            Map.GlobalBattleParams.AttackParams.SharedParams.OwnerAngle = Angle;
            Map.GlobalBattleParams.AttackParams.SharedParams.OwnerPosition = Position;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (TimeElapsed >= AnimationLengthInSeconds)
            {
                ListPERAttackToUpdate.Clear();
                RemoveFromPanelList(this);
            }
            else
            {
                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
                double Progress = TimeElapsed / AnimationLengthInSeconds;

                for (int P = ListPERAttackToUpdate.Count - 1; P >= 0; --P)
                {
                    PERAttackMovement ActiveAttack = ListPERAttackToUpdate[P];
                    Terrain StartTerrain = Map.GetTerrain(ActiveAttack.StartPosition);

                    double Movement = ActiveAttack.Movement.Length() * Progress;
                    ActiveAttack.Owner.DistanceTravelled = Movement;

                    Vector3 NextPosition = ActiveAttack.StartPosition + ActiveAttack.Movement * (float)Progress;

                    if (ActiveAttack.Owner.ActiveAttack.PERAttributes.AffectedByGravity && !ActiveAttack.Owner.IsOnGround)
                    {
                        NextPosition = ActiveAttack.StartPosition + (ActiveAttack.Movement - new Vector3(0, 0, (float)Progress * 16f)) * (float)Progress;
                    }

                    if (NextPosition.X < 0 || NextPosition.X >= Map.MapSize.X || NextPosition.Y < 0 || NextPosition.Y >= Map.MapSize.Y || NextPosition.Z < 0 || NextPosition.Z >= Map.LayerManager.ListLayer.Count)
                    {
                        ListPERAttackToUpdate.RemoveAt(P);
                        ActiveAttack.Owner.IsOnGround = true;

                        if (ActiveAttack.Owner.ActiveAttack.PERAttributes.GroundCollision == Core.Attacks.PERAttackAttributes.GroundCollisions.DestroySelf)
                        {
                            SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                            ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnDeath);
                            ActiveAttack.Owner.DestroySelf();
                        }
                        continue;
                    }

                    Terrain NextTerrain = Map.GetTerrain(new Vector3((int)NextPosition.X, (int)NextPosition.Y, (int)NextPosition.Z));
                    Vector3 NextTerrainRealPosition = Map.GetFinalPosition(NextPosition);
                    float Incline = NextPosition.Z - ActiveAttack.LastRealPosition.Z;
                    if (ActiveAttack.Owner.IsOnGround)
                    {
                        Incline = NextTerrainRealPosition.Z - ActiveAttack.LastRealPosition.Z;
                    }

                    if (ActiveAttack.Owner.IsOnGround && NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex && ActiveAttack.LastRealPosition.Z + MaxInclineDeviationAllowed < Map.LayerManager.ListLayer.Count)
                    {
                        Terrain UpperNextTerrain = Map.GetTerrain(new Vector3(NextPosition.X, NextPosition.Y, NextPosition.Z + MaxInclineDeviationAllowed));
                        if (UpperNextTerrain != NextTerrain)
                        {
                            Vector3 UpperNextTerrainRealPosition = Map.GetFinalPosition(NextPosition);
                            Incline = UpperNextTerrainRealPosition.Z - ActiveAttack.LastRealPosition.Z;
                        }
                    }

                    if (NextTerrain != ActiveAttack.LastTerrain)
                    {
                        SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                        if (ActiveAttack.Owner.ProcessMovement(NextPosition, NextTerrain))
                        {
                            ListPERAttackToUpdate.RemoveAt(P);

                            if (ActiveAttack.Owner.ActiveAttack.PERAttributes.GroundCollision == Core.Attacks.PERAttackAttributes.GroundCollisions.DestroySelf)
                            {
                                SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                                ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnDeath);
                                ActiveAttack.Owner.DestroySelf();
                            }
                        }

                        ActiveAttack.LastTerrain = NextTerrain;

                        ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnTileChange);
                    }
                    else if (ActiveAttack.Owner.IsOnGround && Incline > 0 && Incline < MaxInclineDeviationAllowed && ActiveAttack.Owner.Speed.Z == 0)
                    {
                        ActiveAttack.Owner.SetPosition(NextPosition);
                        ActiveAttack.Owner.IsOnGround = true;
                    }
                    else if (Incline > 0 && Incline < MaxInclineDeviationAllowed && ActiveAttack.Owner.Speed.Z == 0)
                    {
                        ActiveAttack.Owner.SetPosition(NextPosition);
                        ActiveAttack.Owner.IsOnGround = true;
                    }
                    else if (!ActiveAttack.Owner.IsOnGround && StartTerrain != NextTerrain && NextPosition.Z < NextTerrainRealPosition.Z && NextTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
                    {
                        NextPosition.Z = NextTerrain.WorldPosition.Z;
                        ActiveAttack.Owner.SetPosition(NextPosition);
                        ListPERAttackToUpdate.RemoveAt(P);
                        ActiveAttack.Owner.IsOnGround = true;

                        if (ActiveAttack.Owner.ActiveAttack.PERAttributes.GroundCollision == Core.Attacks.PERAttackAttributes.GroundCollisions.DestroySelf)
                        {
                            SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                            ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnDeath);
                            ActiveAttack.Owner.DestroySelf();
                        }
                    }
                    else 
                    {
                        ActiveAttack.Owner.SetPosition(NextPosition);
                    }

                    SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                    ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnDistanceTravelled);

                    ActiveAttack.LastRealPosition = NextTerrainRealPosition;
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelUpdatePERAttacks(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
