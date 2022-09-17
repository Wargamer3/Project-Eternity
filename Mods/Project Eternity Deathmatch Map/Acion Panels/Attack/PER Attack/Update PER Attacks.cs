using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelUpdatePERAttacks : ActionPanelDeathmatch
    {
        private class PERAttackMovement
        {
            public readonly PERAttack Owner;
            public readonly Vector3 StartPosition;
            public readonly Vector3 EndPosition;
            public readonly Vector3 Movement;
            public Vector3 LastPosition;
            public Terrain LastTerrain;
            public readonly List<Terrain> ListCrossedTerrain;

            public PERAttackMovement(PERAttack Owner, Vector3 StartPosition, Vector3 EndPosition, Terrain CurrentTerrain)
            {
                this.Owner = Owner;
                this.StartPosition = StartPosition;
                this.EndPosition = EndPosition;
                LastPosition = StartPosition;
                LastTerrain = CurrentTerrain;
                Movement = EndPosition - StartPosition;
                ListCrossedTerrain = Owner.GetCrossedTerrain(EndPosition);
            }
        }

        private const string PanelName = "UpdatePERAttacks";

        private const double AnimationLengthInSeconds = 2;
        private double TimeElapsed;

        private List<PERAttackMovement> ListPERAttackToUpdate;

        public ActionPanelUpdatePERAttacks(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            TimeElapsed = 0;
            ListPERAttackToUpdate = new List<PERAttackMovement>();
        }

        public ActionPanelUpdatePERAttacks(DeathmatchMap Map, List<PERAttack> ListNewPERAttack)
            : base(PanelName, Map)
        {
            TimeElapsed = 0;
            ListPERAttackToUpdate = new List<PERAttackMovement>();
            foreach (PERAttack ActivePERAttack in ListNewPERAttack)
            {
                Terrain CurrentTerrain = Map.GetTerrain(ActivePERAttack.Position.X, ActivePERAttack.Position.Y, (int)ActivePERAttack.Position.Z);
                Vector3 NextPosition = ActivePERAttack.Position + ActivePERAttack.Speed;
                ListPERAttackToUpdate.Add(new PERAttackMovement(ActivePERAttack, ActivePERAttack.Position, NextPosition, CurrentTerrain));
            }
        }

        public void Add(List<PERAttack> ListNewPERAttack)
        {
            foreach (PERAttack ActivePERAttack in ListNewPERAttack)
            {
                Terrain CurrentTerrain = Map.GetTerrain(ActivePERAttack.Position.X, ActivePERAttack.Position.Y, (int)ActivePERAttack.Position.Z);
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
                            Terrain CurrentTerrain = Map.GetTerrain(ActiveAttack.Position.X, ActiveAttack.Position.Y, (int)ActiveAttack.Position.Z);
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

        public void SetAttackContext(Projectile3D ActiveAttackBox, Squad AttackOwner, Vector3 Angle, Vector3 Position)
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
                    Terrain StartTerrain = Map.GetTerrain(ActiveAttack.StartPosition.X, ActiveAttack.StartPosition.Y, (int)ActiveAttack.StartPosition.Z);

                    Vector3 NextPostion = ActiveAttack.StartPosition + ActiveAttack.Movement * (float)Progress;
                    if (ActiveAttack.Owner.ActiveAttack.PERAttributes.AffectedByGravity && !ActiveAttack.Owner.IsOnGround)
                    {
                        NextPostion = ActiveAttack.StartPosition + (ActiveAttack.Movement - new Vector3(0, 0, (float)Progress * 16f)) * (float)Progress;
                    }

                    if (NextPostion.X < 0 || NextPostion.X >= Map.MapSize.X || NextPostion.Y < 0 || NextPostion.Y >= Map.MapSize.Y || NextPostion.Z < 0 || NextPostion.Z >= Map.LayerManager.ListLayer.Count)
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
                    Terrain NextTerrain = Map.GetTerrain(NextPostion.X, NextPostion.Y, (int)NextPostion.Z);
                    Vector3 NextTerrainRealPosition = NextTerrain.GetRealPosition(NextPostion);

                    if (StartTerrain != NextTerrain && NextPostion.Z < NextTerrainRealPosition.Z && NextTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex)
                    {
                        ActiveAttack.Owner.SetPosition(NextTerrainRealPosition);
                        ListPERAttackToUpdate.RemoveAt(P);
                        ActiveAttack.Owner.IsOnGround = true;

                        if (ActiveAttack.Owner.ActiveAttack.PERAttributes.GroundCollision == Core.Attacks.PERAttackAttributes.GroundCollisions.DestroySelf)
                        {
                            SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                            ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnDeath);
                            ActiveAttack.Owner.DestroySelf();
                        }
                    }
                    else if (NextTerrain != ActiveAttack.LastTerrain && ActiveAttack.ListCrossedTerrain.Contains(NextTerrain))
                    {
                        ActiveAttack.Owner.SetPosition(ActiveAttack.LastPosition);
                        SetAttackContext(ActiveAttack.Owner, ActiveAttack.Owner.Owner, Vector3.Normalize(ActiveAttack.Owner.Speed), ActiveAttack.Owner.Position);
                        ActiveAttack.Owner.ProcessMovement(NextPostion, NextTerrain);
                        ActiveAttack.LastPosition = NextPostion;
                        ActiveAttack.LastTerrain = NextTerrain;

                        ActiveAttack.Owner.UpdateSkills(AttackPERRequirement.OnTileChange);
                    }
                    else
                    {
                        ActiveAttack.Owner.SetPosition(NextPostion);
                    }
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
