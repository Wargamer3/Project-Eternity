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

        private readonly CollisionZone<WorldPolygon> WorldCollisions;
        public readonly List<WorldPolygon> ListWorldCollisionPolygon;//Only used to save and load
        public Dictionary<uint, RobotAnimation> DicRobot;
        public List<Vehicle> ListVehicle;
        public readonly List<RobotAnimation> ListRobotToAdd;
        private List<uint> ListRobotToRemove;
        public Dictionary<uint, WeaponDrop> DicWeaponDrop;
        public readonly List<WeaponDrop> ListWeaponDropToAdd;
        private List<uint> ListWeaponDropToRemove;
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
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget { get { return Owner.DicAutomaticSkillTarget; } }

        public ISFXGenerator PlayerSFXGenerator { get { return Owner.PlayerSFXGenerator; } }

        public Layer(FightingZone Owner)
        {
            ListDelayedOnlineCommand = new List<DelayedExecutableOnlineScript>();
            WorldCollisions = new CollisionZone<WorldPolygon>(1, 1, 0 ,0);
            ListWorldCollisionPolygon = new List<WorldPolygon>();
            ListImages = new List<SimpleAnimation>();
            ListProp = new List<Prop>();
            ListSpawnPointTeam = new List<SpawnPoint>();
            ListSpawnPointNoTeam = new List<SpawnPoint>();
            ListVisualEffects = new List<SimpleAnimation>();
            DicRobot = new Dictionary<uint, RobotAnimation>();
            DicWeaponDrop = new Dictionary<uint, WeaponDrop>();
            ListVehicle = new List<Vehicle>();
            ListRobotToAdd = new List<RobotAnimation>();
            ListRobotToRemove = new List<uint>();
            ListWeaponDropToAdd = new List<WeaponDrop>();
            ListWeaponDropToRemove = new List<uint>();
            GroundLevelCollision = new Polygon();
            ListAttackCollisionBox = new List<Projectile>();
            this.Owner = Owner;
        }

        public Layer(FightingZone Owner, BinaryReader BR)
            : this(Owner)
        {
            Dictionary<string, Prop> DicPropsByName = Prop.GetAllPropsByName();

            int ListPolygonCount = BR.ReadInt32();
            WorldCollisions = new CollisionZone<WorldPolygon>((int)Math.Max(Owner.CameraBounds.Width * 1.5, Owner.CameraBounds.Height * 1.5),
                50,
                (int)(Math.Min(Owner.CameraBounds.X * 1.5, -Owner.CameraBounds.X * 1.5)),
                (int)(Math.Min(Owner.CameraBounds.Y * 1.5, -Owner.CameraBounds.Y * 1.5)));

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

                NewPolygon.Collision.ListCollisionPolygon[0].ComputerCenter();
                WorldCollisions.AddToZone(NewPolygon);
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
                ActivePolygon.Save(BW);
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

            #region Weapon Drop Update

            foreach (WeaponDrop ActiveWeaponDrop in DicWeaponDrop.Values)
            {
                if (!ActiveWeaponDrop.IsAlive)
                    continue;

                ActiveWeaponDrop.Update(gameTime);

                UpdateWeaponDropCollisionWithWorld(ActiveWeaponDrop);

                ActiveWeaponDrop.Move(gameTime);
            }

            foreach (WeaponDrop RobotToAdd in ListWeaponDropToAdd)
            {
                AddDroppedWeapon(RobotToAdd);
            }

            ListWeaponDropToAdd.Clear();

            for (int R = ListWeaponDropToRemove.Count - 1; R >= 0; R--)
            {
                DicWeaponDrop.Remove(ListWeaponDropToRemove[R]);
                ListWeaponDropToRemove.RemoveAt(R);
            }

            #endregion

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

        public void SetRobotContext(RobotAnimation ActiveRobotAnimation, ComboWeapon ActiveWeapon, float Angle, Vector2 Position)
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
                SpawnRobot(Owner.NextRobotID, RobotToSpawn);
            }
        }

        public void SpawnVehicle(Vehicle VehicleToSpawn)
        {
            if (Owner.IsOfflineOrServer)
            {
                SpawnVehicle(Owner.NextRobotID, VehicleToSpawn);
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

                foreach (Polygon EnemyCollision in TargetRobot.Collision.ListCollisionPolygon)
                {
                    foreach (Polygon CollisionPolygon in ActiveAttackBox.Collision.ListCollisionPolygon)
                    {
                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(CollisionPolygon, EnemyCollision, ActiveAttackBox.Speed);

                        if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                        {
                            FinalCollisionResult = CollisionResult;
                            FinalCollisionPolygon = EnemyCollision;
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
                        CreateExplosion(CollisionPoint, ActiveAttackBox.Owner, ActiveAttackBox.ExplosionAttributes, GravityVector);
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

        public HashSet<WorldPolygon> GetCollidingWorldObjects(Projectile ActiveAttackBox)
        {
            return WorldCollisions.GetCollidableObjects(ActiveAttackBox.Collision);
        }

        public HashSet<WorldPolygon> GetCollidingWorldObjects(RobotAnimation ActiveRobot)
        {
            return WorldCollisions.GetCollidableObjects(ActiveRobot.Collision);
        }

        public void UpdateAttackCollisionWithWorld(AttackBox ActiveAttackBox)
        {
            PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
            PolygonCollisionResult FinalCollisionGroundResult = new PolygonCollisionResult(Vector2.Zero, -1);
            Polygon FinalCollisionPolygon = null;

            foreach (WorldPolygon ActivePolygon in GetCollidingWorldObjects(ActiveAttackBox))
            {
                foreach (Polygon CollisionPolygon in ActiveAttackBox.Collision.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionGroundResult;
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(CollisionPolygon, ActivePolygon.Collision.ListCollisionPolygon[0], ActiveAttackBox.Speed, out CollisionGroundResult);

                    if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                    {
                        FinalCollisionResult = CollisionResult;
                        FinalCollisionGroundResult = CollisionGroundResult;
                        FinalCollisionPolygon = ActivePolygon.Collision.ListCollisionPolygon[0];
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
                    CreateExplosion(CollisionPoint, ActiveAttackBox.Owner, ActiveAttackBox.ExplosionAttributes, FinalCollisionGroundResult.Axis);
                }
                else if (!ActiveAttackBox.FollowOwner)
                {
                    Owner.PlayerSFXGenerator.PlayBulletHitObjectSound();
                }
            }
        }

        public void UpdateWeaponDropCollisionWithWorld(WeaponDrop ActiveWeaponDrop)
        {
            PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
            PolygonCollisionResult FinalCollisionGroundResult = new PolygonCollisionResult(Vector2.Zero, -1);
            Polygon FinalCollisionPolygon = null;

            foreach (WorldPolygon ActivePolygon in GetCollidingWorldObjects(ActiveWeaponDrop))
            {
                foreach (Polygon CollisionPolygon in ActiveWeaponDrop.Collision.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionGroundResult;
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(CollisionPolygon, ActivePolygon.Collision.ListCollisionPolygon[0], ActiveWeaponDrop.Speed, out CollisionGroundResult);

                    if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                    {
                        FinalCollisionResult = CollisionResult;
                        FinalCollisionGroundResult = CollisionGroundResult;
                        FinalCollisionPolygon = ActivePolygon.Collision.ListCollisionPolygon[0];
                    }
                }
            }

            if (FinalCollisionResult.Distance >= 0)
            {
                ActiveWeaponDrop.Speed = Vector2.Zero;
                ActiveWeaponDrop.UpdateSkills(TripleThunderAttackRequirement.OnGroundCollisionAttackName);
            }
        }

        public void GetCollidingWorldPolygon(RobotAnimation ActiveRobot, out List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon,
            out List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon, out List<Tuple<PolygonCollisionResult, Polygon>> ListCelingCollidingPolygon, out List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
            ListAllCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListFloorCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListCelingCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();
            ListWallCollidingPolygon = new List<Tuple<PolygonCollisionResult, Polygon>>();

            foreach (WorldPolygon ActiveWorldPolygon in GetCollidingWorldObjects(ActiveRobot))
            {
                if (ActiveRobot.ListIgnoredGroundPolygon.Contains(ActiveWorldPolygon))
                    continue;

                foreach (Polygon ActivePlayerCollisionPolygon in ActiveRobot.Collision.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionResultB;
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon, ActiveWorldPolygon.Collision.ListCollisionPolygon[0], ActiveRobot.Speed, out CollisionResultB);

                    if (CollisionResult.Distance >= 0)
                    {
                        PolygonCollisionResult CollisionResult2 = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon, ActiveWorldPolygon.Collision.ListCollisionPolygon[0], Vector2.Zero, ActiveRobot.Speed);

                        ListAllCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon.Collision.ListCollisionPolygon[0]));
                        
                        if (CollisionResult2.Distance != CollisionResult.Distance)
                            continue;
                        Vector2 GroundAxis = new Vector2(-CollisionResult.Axis.Y, CollisionResult.Axis.X);
                        double FinalCollisionResultAngle = Math.Atan2(GroundAxis.X, GroundAxis.Y);

                        //Ground detection
                        if (FinalCollisionResultAngle >= FightingZone.GroundMinAngle && FinalCollisionResultAngle <= FightingZone.GroundMaxAngle)
                        {
                            ListFloorCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon.Collision.ListCollisionPolygon[0]));
                        }
                        //Ceiling
                        else if (FinalCollisionResultAngle <= -FightingZone.GroundMinAngle && FinalCollisionResultAngle >= -FightingZone.GroundMaxAngle)
                        {
                            ListCelingCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon.Collision.ListCollisionPolygon[0]));
                        }
                        //Wall
                        else
                        {
                            ListWallCollidingPolygon.Add(new Tuple<PolygonCollisionResult, Polygon>(CollisionResult, ActiveWorldPolygon.Collision.ListCollisionPolygon[0]));
                        }
                    }
                }
            }
        }

        public double GetMapVariable(string VariableName)
        {
            return Owner.DicMapVariables[VariableName];
        }

        public void DelayOnlineScript(DelayedExecutableOnlineScript ScriptToDelay)
        {
            lock (ListDelayedOnlineCommand)
            {
                ListDelayedOnlineCommand.Add(ScriptToDelay);
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

        public void AddDroppedWeapon(WeaponDrop DroppedWeapon)
        {
            if (Owner.IsOfflineOrServer)
            {
                AddDroppedWeapon(Owner.NextDroppedWeaponID, DroppedWeapon);
            }
        }

        public void AddDroppedWeapon(uint ID, WeaponDrop DroppedWeapon)
        {
            DroppedWeapon.ID = ID;
            DicWeaponDrop.Add(ID, DroppedWeapon);
        }

        public void RemoveDroppedWeapon(WeaponDrop DroppedWeaponToRemove)
        {
            ListWeaponDropToRemove.Add(DroppedWeaponToRemove.ID);
        }

        public void CreateExplosion(Vector2 ExplosionCenter, RobotAnimation RobotOwner, ExplosionOptions ExplosionAttributes, Vector2 CollisionGroundResult)
        {
            foreach (KeyValuePair<uint, RobotAnimation> ActiveRobotPair in DicRobot)
            {
                RobotAnimation TargetRobot = ActiveRobotPair.Value;

                bool IsSelf = TargetRobot == RobotOwner;

                if (TargetRobot.HP <= 0 || (TargetRobot.Team == RobotOwner.Team && !IsSelf))
                {
                    continue;
                }

                float DistanceFromCenter = Vector2.Distance(ExplosionCenter, TargetRobot.Position);

                if (DistanceFromCenter <= ExplosionAttributes.ExplosionRadius)
                {
                    float DistanceRatio = DistanceFromCenter / ExplosionAttributes.ExplosionRadius;
                    float ExplosionDiff = ExplosionAttributes.ExplosionDamageAtCenter - ExplosionAttributes.ExplosionDamageAtEdge;

                    float FinalDamage = ExplosionAttributes.ExplosionDamageAtCenter + ExplosionDiff * DistanceRatio;
                    if (IsSelf)
                    {
                        FinalDamage *= ExplosionAttributes.ExplosionDamageToSelfMultiplier;
                    }

                    if (Owner.IsOfflineOrServer || Owner.IsServer)
                    {
                        TargetRobot.HP -= (int)FinalDamage;

                        if (TargetRobot.HasKnockback)
                        {
                            float WindDiff = ExplosionAttributes.ExplosionWindPowerAtCenter - ExplosionAttributes.ExplosionWindPowerAtEdge;
                            Vector2 WindVector = Vector2.Normalize(TargetRobot.Position - ExplosionCenter);
                            Vector2 FinalWind = WindVector * (ExplosionAttributes.ExplosionWindPowerAtCenter + WindDiff * DistanceRatio);

                            if (IsSelf)
                            {
                                FinalWind *= ExplosionAttributes.ExplosionWindPowerToSelfMultiplier;
                            }

                            TargetRobot.Speed += FinalWind;
                        }

                        OnDamageRobot(RobotOwner, TargetRobot, (int)FinalDamage, TargetRobot.Position, Owner.IsMainCharacter(TargetRobot.ID));
                    }
                }
            }

            if (!Owner.IsServer && ExplosionAttributes.ExplosionAnimation.Path != string.Empty)
            {
                if (ExplosionAttributes.sndExplosion != null)
                {
                    PlayerSFXGenerator.PrepareExplosionSound(ExplosionAttributes.sndExplosion, ExplosionCenter);
                }

                SimpleAnimation NewExplosion = ExplosionAttributes.ExplosionAnimation.Copy();
                NewExplosion.IsLooped = false;
                NewExplosion.Position = new Vector2(ExplosionCenter.X, ExplosionCenter.Y - NewExplosion.PositionRectangle.Height / 2);
                ListImages.Add(NewExplosion);

                if (Owner.IsInsideCamera(ExplosionCenter, new Vector2(1, 1)))
                {
                    for (int i = 0; i < 30; ++i)
                    {
                        AddVisualEffect(Owner.sprExplosionSplinter, ExplosionCenter, new Vector2(CollisionGroundResult.X - 3 + (float)RandomHelper.Random.NextDouble() * 6, CollisionGroundResult.Y * (2 + (float)RandomHelper.Random.NextDouble() * 4)));
                    }
                }
            }
        }

        public void SendOnlineSFX(Vector2 SFXPosition, CreateSFXScriptClient.SFXTypes SFXType)
        {
            if (Owner.OnlineClient != null)
            {
                Owner.OnlineClient.Host.Send(new CreateSFXScriptClient(SFXPosition, SFXType));
            }
        }

        public void SendOnlineVFX(Vector2 VFXPosition, Vector2 VFXSpeed, CreateVFXScriptClient.VFXTypes VFXType)
        {
            if (Owner.OnlineClient != null)
            {
                Owner.OnlineClient.Host.Send(new CreateVFXScriptClient(VFXPosition, VFXSpeed, VFXType));
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

            foreach (WeaponDrop ActiveWeaponDrop in DicWeaponDrop.Values)
            {
                ActiveWeaponDrop.Draw(g);
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
