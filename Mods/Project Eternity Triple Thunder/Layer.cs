using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Layer : IProjectileSandbox
    {
        private readonly List<DelayedExecutableOnlineScript> ListDelayedOnlineCommand;

        public List<WorldPolygon> ListWorldCollisionPolygon;
        public Dictionary<uint, RobotAnimation> DicRobot;
        public List<Vehicle> ListVehicle;
        public readonly List<RobotAnimation> ListRobotToAdd;
        private List<uint> ListRobotToRemove;
        public List<Projectile> ListAttackCollisionBox;
        public Polygon GroundLevelCollision;
        public List<SimpleAnimation> ListImages;
        public List<Prop> ListProp;
        public List<SpawnPoint> ListSpawnPointTeam;
        public List<SpawnPoint> ListSpawnPointNoTeam;
        public List<SimpleAnimation> ListVisualEffects;

        public static float Gravity = 0.4f;
        public static float GravityMax = 5f;
        public static float Friction = 0.3f;
        private Vector2 _GravityVector = new Vector2(0, 1);
        public Vector2 GravityVector { get { return _GravityVector; } }
        public bool IsServer { get { return Owner.IsServer; } }
        public bool IsOfflineOrServer { get { return Owner.IsOfflineOrServer; } }

        private FightingZone Owner;

        public Dictionary<string, BaseSkillRequirement> DicRequirement { get { return Owner.DicRequirement; } }
        public Dictionary<string, BaseEffect> DicEffect { get { return Owner.DicEffect; } }

        public ISFXGenerator PlayerSFXGenerator { get { return Owner.PlayerSFXGenerator; } }

        public Layer(FightingZone Owner)
        {
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();
            ListWorldCollisionPolygon = new List<WorldPolygon>();
            ListImages = new List<SimpleAnimation>();
            ListProp = new List<Prop>();
            ListSpawnPointTeam = new List<SpawnPoint>();
            ListSpawnPointNoTeam = new List<SpawnPoint>();
            ListVisualEffects = new List<SimpleAnimation>();
            DicRobot = new Dictionary<uint, RobotAnimation>();
            ListVehicle = new List<Vehicle>();
            ListRobotToAdd = new List<RobotAnimation>();
            ListRobotToRemove = new List<uint>();
            GroundLevelCollision = new Polygon();
            ListAttackCollisionBox = new List<Projectile>();
            this.Owner = Owner;
        }

        public Layer(FightingZone Owner, BinaryReader BR)
            : this(Owner)
        {
            Dictionary<string, Prop> DicPropsByName = Prop.GetAllPropsByName();

            int ListPolygonCount = BR.ReadInt32();
            ListWorldCollisionPolygon = new List<WorldPolygon>(ListPolygonCount);
            for (int P = 0; P < ListPolygonCount; P++)
            {
                int ArrayVertexCount = BR.ReadInt32();

                Vector2[] ArrayVertex = new Vector2[ArrayVertexCount];
                for (int V = 0; V < ArrayVertexCount; V++)
                {
                    ArrayVertex[V] = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                }

                bool BlockBullets = BR.ReadBoolean();
                bool IsPlatform = BR.ReadBoolean();

                WorldPolygon NewPolygon;
                if (GameScreen.GraphicsDevice != null)
                {
                    NewPolygon = new WorldPolygon(ArrayVertex, GameScreen.GraphicsDevice.PresentationParameters.BackBufferWidth, GameScreen.GraphicsDevice.PresentationParameters.BackBufferHeight);
                }
                else
                {
                    NewPolygon = new WorldPolygon(ArrayVertex, 1, 1);
                }

                NewPolygon.BlockBullets = BlockBullets;
                NewPolygon.IsPlatform = IsPlatform;

                NewPolygon.ComputerCenter();
                ListWorldCollisionPolygon.Add(NewPolygon);
            }

            int ArrayGroundLevelCollisionCount = BR.ReadInt32();
            Vector2[] ArrayGroundLevelCollision = new Vector2[ArrayGroundLevelCollisionCount];
            for (int V = 0; V < ArrayGroundLevelCollisionCount; V++)
            {
                ArrayGroundLevelCollision[V] = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }
            short[] ArrayGroundLevelCollisionIndex = new short[2];
            ArrayGroundLevelCollisionIndex[0] = 0;
            ArrayGroundLevelCollisionIndex[1] = 1;

            if (GameScreen.GraphicsDevice != null)
            {
                GroundLevelCollision = new Polygon(ArrayGroundLevelCollision, ArrayGroundLevelCollisionIndex, GameScreen.GraphicsDevice.PresentationParameters.BackBufferWidth, GameScreen.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
            else
            {
                GroundLevelCollision = new Polygon(ArrayGroundLevelCollision, ArrayGroundLevelCollisionIndex, 1, 1);
            }

            int ListImagesCount = BR.ReadInt32();
            ListImages = new List<SimpleAnimation>(ListImagesCount);
            for (int P = 0; P < ListImagesCount; P++)
            {
                SimpleAnimation NewBackground = new SimpleAnimation(BR, true);
                NewBackground.Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                NewBackground.Depth = BR.ReadSingle();

                if (!Owner.IsServer)
                {
                    NewBackground.Load(Owner.Content, "");
                    ListImages.Add(NewBackground);
                }
            }

            int ListPropCount = BR.ReadInt32();
            for (int P = 0; P < ListPropCount; ++P)
            {
                string PropName = BR.ReadString();
                Prop NewProp = DicPropsByName[PropName].Copy();
                NewProp.Load(BR, Owner.Content, this, Owner);

                //Props are Client side only.
                if (NewProp.CanRunOnServer || !Owner.IsServer)
                {
                    ListProp.Add(NewProp);
                }
            }

            int ListSpawnPointSPCount = BR.ReadInt32();
            for (int S = 0; S < ListSpawnPointSPCount; ++S)
            {
                ListSpawnPointTeam.Add(SpawnPoint.Load(BR));
            }

            int ListSpawnPointMPCount = BR.ReadInt32();
            for (int S = 0; S < ListSpawnPointMPCount; ++S)
            {
                ListSpawnPointTeam.Add(SpawnPoint.Load(BR));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListWorldCollisionPolygon.Count);
            foreach (WorldPolygon ActivePolygon in ListWorldCollisionPolygon)
            {
                BW.Write(ActivePolygon.ArrayVertex.Length);
                for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                {
                    BW.Write(ActivePolygon.ArrayVertex[V].X);
                    BW.Write(ActivePolygon.ArrayVertex[V].Y);
                }

                BW.Write(ActivePolygon.BlockBullets);
                BW.Write(ActivePolygon.IsPlatform);
            }

            BW.Write(GroundLevelCollision.ArrayVertex.Length);
            for (int V = 0; V < GroundLevelCollision.ArrayVertex.Length; V++)
            {
                BW.Write(GroundLevelCollision.ArrayVertex[V].X);
                BW.Write(GroundLevelCollision.ArrayVertex[V].Y);
            }

            BW.Write(ListImages.Count);
            for (int B = 0; B < ListImages.Count; B++)
            {
                ListImages[B].Save(BW);
                BW.Write(ListImages[B].Position.X);
                BW.Write(ListImages[B].Position.Y);
                BW.Write(ListImages[B].Depth);
            }

            BW.Write(ListProp.Count);
            for (int P = 0; P < ListProp.Count; P++)
            {
                BW.Write(ListProp[P].Name);
                ListProp[P].Save(BW);
            }

            BW.Write(ListSpawnPointTeam.Count);
            for (int S = 0; S < ListSpawnPointTeam.Count; S++)
            {
                ListSpawnPointTeam[S].Save(BW);
            }

            BW.Write(ListSpawnPointNoTeam.Count);
            for (int S = 0; S < ListSpawnPointNoTeam.Count; S++)
            {
                ListSpawnPointNoTeam[S].Save(BW);
            }
        }

        public void Update(GameTime gameTime)
        {
            lock (ListDelayedOnlineCommand)
            {
                foreach (DelayedExecutableOnlineScript ActiveCommand in ListDelayedOnlineCommand)
                {
                    ActiveCommand.ExecuteOnMainThread();
                }

                ListDelayedOnlineCommand.Clear();
            }

            for (int A = ListVisualEffects.Count - 1; A >= 0; --A)
            {
                SimpleAnimation ActiveVisualEffect = ListVisualEffects[A];
                ActiveVisualEffect.Update(gameTime);

                if (ActiveVisualEffect.HasEnded)
                {
                    ListVisualEffects.RemoveAt(A);
                }
            }

            for (int A = 0; A < ListAttackCollisionBox.Count; A++)
            {
                AttackBox ActiveAttackBox = (AttackBox)ListAttackCollisionBox[A];
                if (!ActiveAttackBox.IsAlive)
                    continue;

                ActiveAttackBox.Update(gameTime);

                foreach (KeyValuePair<uint, RobotAnimation> ActiveRobot in DicRobot)
                {
                    UpdateAttackCollisionWithRobot(gameTime, ActiveAttackBox, ActiveRobot.Value);
                }

                UpdateAttackCollisionWithWorld(ActiveAttackBox);

                ActiveAttackBox.Move(gameTime);
            }

            for (int A = ListImages.Count - 1; A >= 0; --A)
            {
                SimpleAnimation ActiveAnimation = ListImages[A];
                ActiveAnimation.Update(gameTime);

                if (ActiveAnimation.HasEnded)
                {
                    ListImages.RemoveAt(A);
                }
            }

            for (int P = ListProp.Count - 1; P >= 0; --P)
            {
                if (ListProp[P].HasEnded)
                {
                    ListProp.RemoveAt(P);
                }
                else
                {
                    ListProp[P].Update(gameTime);
                }
            }

            #region Robot Update

            foreach (RobotAnimation ActiveRobot in DicRobot.Values)
            {
                if (!ActiveRobot.IsUpdated)
                    continue;

                if (ActiveRobot.RobotAI != null && !Owner.UsePreview)
                {
                    ActiveRobot.RobotAI.UpdateStep(gameTime);
                }

                ActiveRobot.Update(gameTime, DicRobot);
            }

            foreach (RobotAnimation RobotToAdd in ListRobotToAdd)
            {
                SpawnRobot(RobotToAdd);
            }

            ListRobotToAdd.Clear();

            for (int R = ListRobotToRemove.Count - 1; R >= 0; R--)
            {
                DicRobot.Remove(ListRobotToRemove[R]);
                ListRobotToRemove.RemoveAt(R);

                if (ListRobotToRemove.Count == 0)
                {
                    Owner.CheckIfAllEnemiesAreDead();
                }
            }

            #endregion
        }

        public void SetRobotContext(RobotAnimation ActiveRobotAnimation)
        {
            Owner.GlobalRobotContext.SetRobotContext(this, ActiveRobotAnimation);
        }

        public void SetRobotContext(RobotAnimation ActiveRobotAnimation, Weapon ActiveWeapon, float Angle, Vector2 Position)
        {
            Owner.GlobalRobotContext.SetRobotContext(this, ActiveRobotAnimation, ActiveWeapon, Angle, Position);
        }

        public void SetAttackContext(Projectile ActiveAttackBox, RobotAnimation AttackOwner, float Angle, Vector2 Position)
        {
            Owner.GlobalAttackContext.Owner = AttackOwner;
            Owner.GlobalAttackContext.OwnerProjectile = ActiveAttackBox;
            Owner.GlobalAttackContext.OwnerSandbox = this;
            Owner.AttackParams.SharedParams.OwnerAngle = Angle;
            Owner.AttackParams.SharedParams.OwnerPosition = Position;
        }

        public void SpawnRobot(RobotAnimation RobotToSpawn)
        {
            if (Owner.IsOfflineOrServer)
            {
                SpawnRobot(Owner.NextID, RobotToSpawn);
            }
        }

        public void SpawnVehicle(Vehicle VehicleToSpawn)
        {
            if (Owner.IsOfflineOrServer)
            {
                SpawnVehicle(Owner.NextID, VehicleToSpawn);
            }
        }

        public void SpawnRobot(uint ID, RobotAnimation RobotToSpawn)
        {
            RobotToSpawn.HP = RobotToSpawn.MaxHP;
            RobotToSpawn.EN = RobotToSpawn.MaxEN;
            RobotToSpawn.ID = ID;
            DicRobot.Add(ID, RobotToSpawn);
        }

        public void SpawnVehicle(uint ID, Vehicle VehicleToSpawn)
        {
            VehicleToSpawn.HP = VehicleToSpawn.MaxHP;
            VehicleToSpawn.EN = VehicleToSpawn.MaxEN;
            VehicleToSpawn.ID = ID;
            DicRobot.Add(ID, VehicleToSpawn);
            ListVehicle.Add(VehicleToSpawn);
        }

        public void RemoveRobot(RobotAnimation RobotToRemove)
        {
            ListRobotToRemove.Add(RobotToRemove.ID);
        }

        public void OnDamageRobot(RobotAnimation ActiveRobot, RobotAnimation TargetRobot, int Damage, Vector2 Position, bool IsPlayerControlled)
        {
            if (Owner.IsServer)
            {
                int LayerIndex = Owner.ListLayer.IndexOf(this);
                Owner.OnlineServer.SharedWriteBuffer.ClearWriteBuffer();

                Owner.OnlineServer.SharedWriteBuffer.WriteScript(new SendPlayerDamageScriptServer(LayerIndex, ActiveRobot.ID, TargetRobot.ID, Position,
                    Damage, TargetRobot.HP, IsPlayerControlled));

                foreach (IOnlineConnection ActivePlayer in Owner.GameGroup.Room.ListOnlinePlayer)
                {
                    ActivePlayer.SendWriteBuffer();
                }

                if (TargetRobot.HP <= 0)
                {
                    Owner.Rules.OnKill(ActiveRobot, TargetRobot);

                    if (IsPlayerControlled)
                    {
                        TargetRobot.RespawnTimer = Owner.RespawnTime;
                    }
                    else
                    {
                        ListRobotToRemove.Add(TargetRobot.ID);
                    }
                }
            }
            else
            {
                Owner.AddDamageNumber(new DamageNumber(Position, Damage, 1000));
                Owner.PlayerSFXGenerator.PlayGetHitSound();

                if (TargetRobot.HP <= 0)
                {
                    Owner.Rules.OnKill(ActiveRobot, TargetRobot);

                    if (Owner.IsMainCharacter(TargetRobot.ID))
                    {
                        TargetRobot.RespawnTimer = Owner.RespawnTime;
                        Owner.AddKillMessage(true, true, TargetRobot.Name);
                    }
                    else if (IsPlayerControlled)
                    {
                        TargetRobot.RespawnTimer = Owner.RespawnTime;
                    }
                    else
                    {
                        ListRobotToRemove.Add(TargetRobot.ID);
                        Owner.AddKillMessage(true, false, TargetRobot.Name);
                    }

                    TargetRobot.UpdateSkills(TripleThunderRobotRequirement.OnDestroyedName);
                }
            }
        }

        public void ChangeRobotLayer(RobotAnimation RobotToChange, int NewLayerIndex)
        {
            DicRobot.Remove(RobotToChange.ID);
            Owner.ChangeRobotLayer(RobotToChange, NewLayerIndex);
        }

        public void PrepareNextLevel(string NextLevelPath)
        {
            Owner.PrepareNextLevel(NextLevelPath);
        }

        public void UpdateAttackCollisionWithRobot(GameTime gameTime, AttackBox ActiveAttackBox, RobotAnimation TargetRobot)
        {
            if (TargetRobot.HP <= 0 || TargetRobot.Team == ActiveAttackBox.Owner.Team)
                return;

            if (!ActiveAttackBox.ListAttackedRobots.Contains(TargetRobot))
            {
                PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
                Polygon FinalCollisionPolygon = null;

                foreach (CollisionPolygon EnemyCollision in TargetRobot.ListCollisionPolygon)
                {
                    foreach (Polygon CollisionPolygon in ActiveAttackBox.ListCollisionPolygon)
                    {
                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(CollisionPolygon, EnemyCollision.ActivePolygon, ActiveAttackBox.Speed);

                        if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                        {
                            FinalCollisionResult = CollisionResult;
                            FinalCollisionPolygon = EnemyCollision.ActivePolygon;
                        }
                    }
                }

                if (FinalCollisionResult.Distance >= 0)
                {
                    if (TargetRobot.RobotAI != null)
                        TargetRobot.RobotAI.Update(gameTime, "On Hit");

                    Vector2 CollisionPoint;
                    ActiveAttackBox.OnCollision(FinalCollisionResult, FinalCollisionPolygon, out CollisionPoint);

                    if (ActiveAttackBox.ExplosionAttributes.ExplosionRadius > 0)
                    {
                        CreateExplosion(CollisionPoint, ActiveAttackBox, GravityVector);
                    }

                    ActiveAttackBox.ListAttackedRobots.Add(TargetRobot);

                    if (TargetRobot.HasKnockback)
                    {
                        TargetRobot.Speed.X = Math.Sign(ActiveAttackBox.Speed.X) * 3;
                        TargetRobot.Speed.Y = Math.Sign(ActiveAttackBox.Speed.Y) * 3;
                    }

                    if (Owner.IsOfflineOrServer || Owner.IsServer)
                    {
                        int FinalDamage = (int)ActiveAttackBox.Damage;

                        TargetRobot.HP -= FinalDamage;

                        OnDamageRobot(ActiveAttackBox.Owner, TargetRobot, FinalDamage, FinalCollisionPolygon.Center, Owner.IsMainCharacter(TargetRobot.ID));
                    }
                }
            }
        }

        public void UpdateAttackCollisionWithWorld(AttackBox ActiveAttackBox)
        {
            PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
            PolygonCollisionResult FinalCollisionGroundResult = new PolygonCollisionResult(Vector2.Zero, -1);
            Polygon FinalCollisionPolygon = null;

            foreach (Polygon ActivePolygon in ListWorldCollisionPolygon)
            {
                foreach (Polygon CollisionPolygon in ActiveAttackBox.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionGroundResult;
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(CollisionPolygon, ActivePolygon, ActiveAttackBox.Speed, out CollisionGroundResult);

                    if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                    {
                        FinalCollisionResult = CollisionResult;
                        FinalCollisionGroundResult = CollisionGroundResult;
                        FinalCollisionPolygon = ActivePolygon;
                    }
                }
            }

            if (FinalCollisionResult.Distance >= 0)
            {
                Vector2 CollisionPoint;
                ActiveAttackBox.OnCollision(FinalCollisionResult, FinalCollisionPolygon, out CollisionPoint);
                SetAttackContext(ActiveAttackBox, ActiveAttackBox.Owner, (float)Math.Atan2(-FinalCollisionGroundResult.Axis.X, -FinalCollisionGroundResult.Axis.Y), CollisionPoint);
                ActiveAttackBox.UpdateSkills(TripleThunderAttackRequirement.OnGroundCollisionAttackName);

                if (ActiveAttackBox.ExplosionAttributes.ExplosionRadius > 0)
                {
                    CreateExplosion(CollisionPoint, ActiveAttackBox, FinalCollisionGroundResult.Axis);
                }
                else if (!ActiveAttackBox.FollowOwner)
                {
                    Owner.PlayerSFXGenerator.PlayBulletHitObjectSound();
                }
            }
        }

        public void GetCollidingWorldPolygon(RobotAnimation ActiveRobot, out List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon,
            out List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon, out List<Tuple<PolygonCollisionResult, Polygon>> ListCelingCollidingPolygon, out List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
            ListAllCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListFloorCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListCelingCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListWallCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();

            foreach (Polygon ActiveWorldPolygon in ListWorldCollisionPolygon)
            {
                if (ActiveRobot.ListIgnoredGroundPolygon.Contains(ActiveWorldPolygon))
                    continue;

                foreach (CollisionPolygon ActivePlayerCollisionPolygon in ActiveRobot.ListCollisionPolygon)
                {
                    if (ActivePlayerCollisionPolygon.IsDead)
                        continue;

                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon.ActivePolygon, ActiveWorldPolygon, ActiveRobot.Speed);

                    if (CollisionResult.Distance >= 0)
                    {
                        ListAllCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon));

                        Vector2 GroundAxis = new Vector2(-CollisionResult.Axis.Y, CollisionResult.Axis.X);
                        double FinalCollisionResultAngle = Math.Atan2(GroundAxis.X, GroundAxis.Y);

                        //Ground detection
                        if (FinalCollisionResultAngle >= FightingZone.GroundMinAngle && FinalCollisionResultAngle <= FightingZone.GroundMaxAngle)
                        {
                            ListFloorCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon));
                        }
                        //Ceiling
                        else if (FinalCollisionResultAngle <= -FightingZone.GroundMinAngle && FinalCollisionResultAngle >= -FightingZone.GroundMaxAngle)
                        {
                            ListCelingCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon));
                        }
                        //Wall
                        else
                        {
                            ListWallCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon));
                        }
                    }
                }
            }
        }

        public double GetMapVariable(string VariableName)
        {
            return Owner.DicMapVariables[VariableName];
        }

        public void DelayOnlineScript(DelayedExecutableOnlineScript NewProjectile)
        {
            lock (ListDelayedOnlineCommand)
            {
                ListDelayedOnlineCommand.Add(NewProjectile);
            }
        }

        public void AddProjectile(Projectile NewProjectile)
        {
            bool AttackReplaced = false;
            for (int A = 0; A < ListAttackCollisionBox.Count && !AttackReplaced; A++)
            {
                if (!ListAttackCollisionBox[A].IsAlive)
                {
                    ListAttackCollisionBox[A] = NewProjectile;
                    AttackReplaced = true;
                    break;
                }
            }

            if (!AttackReplaced)
                ListAttackCollisionBox.Add(NewProjectile);
        }

        internal void AddProjectile(uint OwnerID, string WeaponName, Vector2 GunNozzlePosition, List<AttackBox> ListAttack)
        {
            foreach (AttackBox NewProjectile in ListAttack)
            {
                bool AttackReplaced = false;
                for (int A = 0; A < ListAttackCollisionBox.Count && !AttackReplaced; A++)
                {
                    if (!ListAttackCollisionBox[A].IsAlive)
                    {
                        ListAttackCollisionBox[A] = NewProjectile;
                        AttackReplaced = true;
                        break;
                    }
                }

                if (!AttackReplaced)
                {
                    ListAttackCollisionBox.Add(NewProjectile);
                }
            }

            if (!Owner.IsOfflineOrServer && Owner.IsMainCharacter(OwnerID))
            {
                Owner.OnlineClient.Host.Send(new ShootBulletScriptClient(OwnerID, Owner.ListLayer.IndexOf(this), WeaponName, GunNozzlePosition, ListAttack));
            }
        }

        private void CreateExplosion(Vector2 ExplosionCenter, AttackBox ActiveAttackBox, Vector2 CollisionGroundResult)
        {
            foreach (KeyValuePair<uint, RobotAnimation> ActiveRobotPair in DicRobot)
            {
                RobotAnimation TargetRobot = ActiveRobotPair.Value;

                bool IsSelf = TargetRobot == ActiveAttackBox.Owner;

                if (TargetRobot.HP <= 0 || (TargetRobot.Team == ActiveAttackBox.Owner.Team && !IsSelf))
                {
                    continue;
                }

                float DistanceFromCenter = Vector2.Distance(ExplosionCenter, TargetRobot.Position);

                if (DistanceFromCenter <= ActiveAttackBox.ExplosionAttributes.ExplosionRadius)
                {
                    float DistanceRatio = DistanceFromCenter / ActiveAttackBox.ExplosionAttributes.ExplosionRadius;
                    float ExplosionDiff = ActiveAttackBox.ExplosionAttributes.ExplosionDamageAtCenter - ActiveAttackBox.ExplosionAttributes.ExplosionDamageAtEdge;

                    float FinalDamage = ActiveAttackBox.ExplosionAttributes.ExplosionDamageAtCenter + ExplosionDiff * DistanceRatio;
                    if (IsSelf)
                    {
                        FinalDamage *= ActiveAttackBox.ExplosionAttributes.ExplosionDamageToSelfMultiplier;
                    }

                    if (Owner.IsOfflineOrServer || Owner.IsServer)
                    {
                        TargetRobot.HP -= (int)FinalDamage;

                        if (TargetRobot.HasKnockback)
                        {
                            float WindDiff = ActiveAttackBox.ExplosionAttributes.ExplosionWindPowerAtCenter - ActiveAttackBox.ExplosionAttributes.ExplosionWindPowerAtEdge;
                            Vector2 WindVector = Vector2.Normalize(TargetRobot.Position - ExplosionCenter);
                            Vector2 FinalWind = WindVector * (ActiveAttackBox.ExplosionAttributes.ExplosionWindPowerAtCenter + WindDiff * DistanceRatio);

                            if (IsSelf)
                            {
                                FinalWind *= ActiveAttackBox.ExplosionAttributes.ExplosionWindPowerToSelfMultiplier;
                            }

                            TargetRobot.Speed += FinalWind;
                        }

                        OnDamageRobot(ActiveAttackBox.Owner, TargetRobot, (int)FinalDamage, TargetRobot.Position, Owner.IsMainCharacter(TargetRobot.ID));
                    }
                }
            }

            if (!Owner.IsServer && ActiveAttackBox.ExplosionAttributes.ExplosionAnimation.Path != string.Empty)
            {
                if (ActiveAttackBox.ExplosionAttributes.sndExplosion != null)
                {
                    PlayerSFXGenerator.PrepareExplosionSound(ActiveAttackBox.ExplosionAttributes.sndExplosion, ExplosionCenter);
                }

                SimpleAnimation NewExplosion = ActiveAttackBox.ExplosionAttributes.ExplosionAnimation.Copy();
                NewExplosion.IsLooped = false;
                NewExplosion.Position = new Vector2(ExplosionCenter.X, ExplosionCenter.Y - NewExplosion.PositionRectangle.Height / 2);
                ListImages.Add(NewExplosion);

                for (int i = 0; i < 30; ++i)
                {
                    AddVisualEffect(Owner.sprExplosionSplinter, ExplosionCenter, new Vector2(CollisionGroundResult.X - 3 + (float)RandomHelper.Random.NextDouble() * 6, CollisionGroundResult.Y * (2 +  (float)RandomHelper.Random.NextDouble() * 4)));
                }
            }
        }

        public void AddVisualEffect(AnimatedSprite NewEffect, Vector2 Position, Vector2 Speed)
        {
            MovingSimpleAnimation NewVisualEffect = new MovingSimpleAnimation(1, Position, Speed, GravityVector * Gravity * 0.5f, (float)(RandomHelper.Random.NextDouble() * Math.PI));
            NewVisualEffect.ActualSprite = NewEffect.Copy();
            NewVisualEffect.ActualSprite.SetRandomFrame();

            NewVisualEffect.Position = Position;

            ListVisualEffects.Add(NewVisualEffect);
        }

        public void Draw(CustomSpriteBatch g)
        {
            foreach (SimpleAnimation ActiveAnimation in ListVisualEffects)
            {
                ActiveAnimation.Draw(g);
            }

            foreach (SimpleAnimation ActiveAnimation in ListImages)
            {
                ActiveAnimation.Draw(g);
            }

            foreach (AttackBox ActiveCollision in ListAttackCollisionBox)
            {
                if (!ActiveCollision.IsAlive)
                    continue;

                ActiveCollision.DrawRegular(g);
            }

            foreach (Prop ActiveProp in ListProp)
            {
                ActiveProp.Draw(g);
            }
        }

        public void DrawAdditive(CustomSpriteBatch g)
        {

            foreach (AttackBox ActiveCollision in ListAttackCollisionBox)
            {
                if (!ActiveCollision.IsAlive)
                    continue;

                ActiveCollision.DrawAdditive(g);
            }
        }
    }
}
