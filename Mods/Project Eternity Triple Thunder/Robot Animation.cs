using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class RobotAnimation : ComplexAnimation, ICollisionObjectHolder2D<RobotAnimation>
    {
        public uint ID;
        public readonly string Name;
        public int MaxHP;
        public int MaxEN;
        public int HP;
        public int EN;
        public int Team;
        public int Kill;
        public int Death;
        public bool HasKnockback;
        public bool IsDynamic;
        public bool IsUpdated;//Used to disable robot inside vehicles
        public float RespawnTimer;
        public Rectangle Camera;
        public RobotInput InputManager;
        public RobotInputManager InputManagerHelper;

        public Vector2 Speed;
        public Vector2 TotalMovementThisFrame;
        public Vector2 NormalizedGroundVector;
        public Vector2 NormalizedPerpendicularGroundVector;
        public List<WorldPolygon> ListCollidingGroundPolygon;
        public List<WorldPolygon> ListIgnoredGroundPolygon;

        public int CurrentLane;//Current max Y position.
        private CollisionObject2D<RobotAnimation> CollisionBox;
        public CollisionObject2D<RobotAnimation> Collision => CollisionBox;

        public bool LockAnimation;
        protected WeaponBase CurrentStanceAnimations;
        protected WeaponBase StandingAnimations { get { return ListStanceAnimation[0]; } }
        protected WeaponBase CrouchAnimations { get { return ListStanceAnimation.Count > 1 ? ListStanceAnimation[1] : null; } }
        protected WeaponBase RollAnimations { get { return ListStanceAnimation.Count > 2 ? ListStanceAnimation[2] : null; } }
        protected WeaponBase ProneAnimations { get { return ListStanceAnimation.Count > 3 ? ListStanceAnimation[3] : null; } }
        public WeaponHolder PrimaryWeapons;
        public WeaponHolder SecondaryWeapons;
        protected List<WeaponBase> ListStanceAnimation;
        public WeaponBase ActiveAddedWeapon;
        public MovementInputs CurrentMovementInput;
        public string ActiveAttackStance;
        public string ActiveMovementStance;
        public int ViewDistance = 20;
        public float Accel;
        public float MaxWalkSpeed;
        public float JumpSpeed;
        public Vector2 LastPositionOnGround;
        public bool IsInAir { get; private set; }
        public bool IsOnGround { get { return !IsInAir; } }

        public readonly EquipmentLoadout Equipment;
        public readonly UnitSounds Sounds;

        public static Random Random = new Random();
        protected Layer CurrentLayer;
        private bool CollisionBetweenRobot = false;
        public Core.AI.AIContainer RobotAI;
        public EffectHolder Effects;
        public ISFXGenerator PlayerSFXGenerator { get; private set; }
        public float GravityMax { get { return Layer.GravityMax; } }
        public float Gravity { get { return Layer.Gravity; } }
        public Vector2 GravityVector { get { return CurrentLayer.GravityVector; } }
        public float Friction { get { return Layer.Friction; } }

        public List<MagicSpell> ListMagicSpell;
        public Dictionary<string, string> DicStoredVariable;// Used for extra stuff

        private RobotAnimation()
            : base()
        {
            IsUpdated = true;
            Camera = new Rectangle(0, 0, Constants.Width, Constants.Height);
            InputManager = new NullRobotInput();
            InputManagerHelper = new NullRobotInputManager();
            PrimaryWeapons = new WeaponHolder(0);
            IsInAir = false;
            CurrentLane = 1350;
            CollisionBox = new CollisionObject2D<RobotAnimation>();
            ListCollidingGroundPolygon = new List<WorldPolygon>();
            ListIgnoredGroundPolygon = new List<WorldPolygon>();
            Effects = new EffectHolder();
            ListMagicSpell = new List<MagicSpell>();
            ListMagicSpell.Add(new MagicSpell(null));
            DicStoredVariable = new Dictionary<string, string>();
        }

        /// <summary>
        /// Used for tests
        /// </summary>
        /// <param name="CurrentLayer"></param>
        /// <param name="Position"></param>
        /// <param name="Team"></param>
        /// <param name="ListWeapon"></param>
        public RobotAnimation(Layer CurrentLayer, Vector2 Position, int Team, List<WeaponBase> ListWeapon)
            : this()
        {
            this.CurrentLayer = CurrentLayer;
            this.Position = Position;
            this.Team = Team;

            if (ListWeapon != null)
            {
                PrimaryWeapons = new WeaponHolder(ListWeapon.Count);
                foreach (WeaponBase ActiveWeapon in ListWeapon)
                {
                    PrimaryWeapons.AddWeaponToStash(ActiveWeapon);
                }
            }

            if (ListWeapon.Count > 0)
                ChangeWeapon(0);
        }

        protected RobotAnimation(string Name, Layer CurrentLayer, Vector2 Position, int Team, EquipmentLoadout Equipment, ISFXGenerator PlayerSFXGenerator)
            : this()
        {
            this.PlayerSFXGenerator = PlayerSFXGenerator.Copy();
            this.Name = Name;
            this.CurrentLayer = CurrentLayer;
            this.Position = Position;
            this.Team = Team;
            this.Equipment = Equipment;
        }

        public RobotAnimation(string Name, Layer CurrentLayer, Vector2 Position, int Team, PlayerInventory Equipment, ISFXGenerator PlayerSFXGenerator, List<WeaponBase> ListExtraWeapon)
            : this()
        {
            this.PlayerSFXGenerator = PlayerSFXGenerator.Copy();
            this.Name = Name;
            this.CurrentLayer = CurrentLayer;
            this.Position = Position;
            this.Team = Team;
            this.Equipment = new EquipmentLoadout(Equipment, this);

            FileStream FS = new FileStream("Content/Units/Triple Thunder/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MaxHP = BR.ReadInt32();
            MaxEN = BR.ReadInt32();
            Accel = BR.ReadSingle();
            MaxWalkSpeed = BR.ReadSingle();
            JumpSpeed = BR.ReadSingle();
            HasKnockback = BR.ReadBoolean();
            IsDynamic = BR.ReadBoolean();

            Dictionary<string, BaseSkillRequirement> DicRequirement = null;
            Dictionary<string, BaseEffect> DicEffect = null;
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget = null;

            if (CurrentLayer != null)
            {
                DicRequirement = CurrentLayer.DicRequirement;
                DicEffect = CurrentLayer.DicEffect;
                DicAutomaticSkillTarget = CurrentLayer.DicAutomaticSkillTarget;
            }

            ListStanceAnimation = new List<WeaponBase>(4);

            if (File.Exists("Content/Triple Thunder/Weapons/" + Name + "/Default" + ".ttw"))
                ListStanceAnimation.Add(WeaponBase.CreateFromFile(Name, Name + "/Default", true, DicRequirement, DicEffect, DicAutomaticSkillTarget));

            if (File.Exists("Content/Triple Thunder/Weapons/" + Name + "/Crouch" + ".ttw"))
                ListStanceAnimation.Add(WeaponBase.CreateFromFile(Name, Name + "/Crouch", true, DicRequirement, DicEffect, DicAutomaticSkillTarget));

            if (File.Exists("Content/Triple Thunder/Weapons/" + Name + "/Roll" + ".ttw"))
                ListStanceAnimation.Add(WeaponBase.CreateFromFile(Name, Name + "/Roll", true, DicRequirement, DicEffect, DicAutomaticSkillTarget));

            if (File.Exists("Content/Triple Thunder/Weapons/" + Name + "/Prone" + ".ttw"))
                ListStanceAnimation.Add(WeaponBase.CreateFromFile(Name, Name + "/Prone", true, DicRequirement, DicEffect, DicAutomaticSkillTarget));

            CurrentStanceAnimations = StandingAnimations;

            int ListWeaponCount = BR.ReadInt32();

            PrimaryWeapons = new WeaponHolder(ListWeaponCount);
            SecondaryWeapons = new WeaponHolder(1);
            for (int W = 0; W < ListWeaponCount; ++W)
            {
                string WeaponName = BR.ReadString();
                WeaponBase NewWeapon;
                string WeaponPath = Name + "/Weapons/" + WeaponName;
                bool IsGrenade = false;
                if (!File.Exists("Content/Triple Thunder/Weapons/" + WeaponPath + ".ttw"))
                {
                    WeaponPath = Name + "/Grenades/" + WeaponName;
                    IsGrenade = true;
                }

                if (CurrentLayer == null)
                {
                    NewWeapon = WeaponBase.CreateFromFile(Name, WeaponPath, false, null, null, null);
                }
                else
                {
                    NewWeapon = WeaponBase.CreateFromFile(Name, WeaponPath, false, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
                }

                NewWeapon.WeaponName = WeaponName;
                if (IsGrenade)
                {
                    SecondaryWeapons.AddWeaponToStash(NewWeapon);
                }
                else
                {
                    PrimaryWeapons.AddWeaponToStash(NewWeapon);
                }
            }

            if (ListExtraWeapon != null)
            {
                foreach (WeaponBase ActiveWeapon in ListExtraWeapon)
                {
                    ActiveWeapon.IsExtra = true;
                    PrimaryWeapons.AddWeaponToStash(ActiveWeapon);
                    PrimaryWeapons.UseWeapon(ActiveWeapon);
                }
            }

            Sounds = new UnitSounds(BR);

            FS.Close();
            BR.Close();

            Load();

            if (!PrimaryWeapons.HasActiveWeapons)
            {
                if (PrimaryWeapons.HasWeapons)
                    ChangeWeapon(0);
                else
                    ChangeWeapon(-1);
            }

            if (CurrentLayer != null)
            {
                UpdateSkills(BaseSkillRequirement.OnCreatedRequirementName);
            }
        }

        public override void Load()
        {
            DicTimeline.Clear();
            foreach (KeyValuePair<string, Timeline> Timeline in LoadTimelines(typeof(CoreTimeline)))
            {
                if (Timeline.Value is AnimationOriginTimeline)
                    continue;

                DicTimeline.Add(Timeline.Key, Timeline.Value);
            }
            foreach (KeyValuePair<string, Timeline> Timeline in LoadTimelines(typeof(TripleThunderTimeline), this))
            {
                DicTimeline.Add(Timeline.Key, Timeline.Value);
            }

            base.Load();

            if (Content != null)
            {
                PrimaryWeapons.Load(Content);
                SecondaryWeapons.Load(Content);

                for (int W = 0; W < ListStanceAnimation.Count; ++W)
                {
                    if (ListStanceAnimation[W].ActiveProjectileInfo != null && ListStanceAnimation[W].ActiveProjectileInfo.ProjectileAnimation.Path != string.Empty)
                    {
                        ListStanceAnimation[W].ActiveProjectileInfo.ProjectileAnimation.Load(Content, "Animations/Sprites/");
                        ListStanceAnimation[W].NozzleFlashAnimation.Load(Content, "Animations/Sprites/");
                    }
                    if (ListStanceAnimation[W].ActiveProjectileInfo != null && ListStanceAnimation[W].ActiveProjectileInfo.TrailAnimation != null
                        && ListStanceAnimation[W].ActiveProjectileInfo.TrailAnimation.Path != string.Empty)
                    {
                        ListStanceAnimation[W].ActiveProjectileInfo.TrailAnimation.Load(Content, "Animations/Sprites/");
                    }
                    if (ListStanceAnimation[W].ExplosionAttributes.ExplosionAnimation.Path != string.Empty)
                    {
                        ListStanceAnimation[W].ExplosionAttributes.ExplosionAnimation.Load(Content, "Animations/Sprites/");
                    }
                }
            }

            SetAnimation(StandingAnimations.GetDefaultAnimationName());
            CurrentMovementInput = MovementInputs.None;
            ActiveMovementStance = "None";
            Update(new GameTime());
            SetIdle();
        }

        public void ChangeMap(Rectangle CameraBounds)
        {
            for (int W = 0; W < ListStanceAnimation.Count; ++W)
            {
                ListStanceAnimation[W] = WeaponBase.CreateFromFile(Name, ListStanceAnimation[W].WeaponPath, false, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
            }

            PrimaryWeapons.ChangeMap(CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
            SecondaryWeapons.ChangeMap(CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
            InputManager.ResetCameraBounds(CameraBounds);
            Load();
        }

        public void UpdateControls(GameplayTypes GameplayType, Rectangle CameraBounds)
        {
            InputManager = InputManagerHelper.GetRobotInput(GameplayType, this, CameraBounds);
        }

        public void Move(MovementInputs MovementInput)
        {
            Equipment.Move(MovementInput);
            
            if (LockAnimation)
                return;

            if (IsOnGround)
            {
                if (CurrentStanceAnimations == CrouchAnimations && Sounds.CrouchMoveSound != UnitSounds.CrouchMoveSounds.None)
                {
                    PlayerSFXGenerator.PlayCrouchMoveSound(Sounds.CrouchMoveSound);
                }
                else if (CurrentStanceAnimations == ProneAnimations && Sounds.ProneMoveSound != UnitSounds.ProneMoveSounds.None)
                {
                    PlayerSFXGenerator.PlayProneMoveSound(Sounds.ProneMoveSound);
                }
                else if (Sounds.StepGrassSound != UnitSounds.StepGrassSounds.None)
                {
                    PlayerSFXGenerator.PlayStepGrassSound(Sounds.StepGrassSound);
                }
            }

            SetRobotAnimation(ActiveMovementStance);
        }

        public void Crouch()
        {
            if (LockAnimation)
                return;

            if (CurrentStanceAnimations != CrouchAnimations)
            {
                if (CurrentMovementInput == MovementInputs.Left || CurrentMovementInput == MovementInputs.Right)
                {
                    Roll();
                }
                else
                {
                    CurrentStanceAnimations = CrouchAnimations;
                    SetRobotAnimation(ActiveMovementStance);
                    if (Sounds.CrouchStartSound != UnitSounds.CrouchStartSounds.None)
                    {
                        PlayerSFXGenerator.PlayCrouchStartSound(Sounds.CrouchStartSound);
                    }
                }
            }
            else
            {
                CurrentStanceAnimations = CrouchAnimations;
                SetRobotAnimation(ActiveMovementStance);
            }
        }

        public void StartCrouch()
        {
            if (CurrentMovementInput == MovementInputs.Left || CurrentMovementInput == MovementInputs.Right)
            {
                Roll();
            }
            else
            {
                CurrentStanceAnimations = CrouchAnimations;
                SetRobotAnimation(ActiveMovementStance);
                if (Sounds.CrouchStartSound != UnitSounds.CrouchStartSounds.None)
                {
                    PlayerSFXGenerator.PlayCrouchStartSound(Sounds.CrouchStartSound);
                }
            }
        }

        public void Roll()
        {
            LockAnimation = true;
            CurrentStanceAnimations = RollAnimations;
            SetRobotAnimation(CurrentStanceAnimations.GetDefaultAnimationName());
            CurrentStanceAnimations = CrouchAnimations;
            if (Sounds.RollSound != UnitSounds.RollSounds.None)
            {
                PlayerSFXGenerator.PlayRollSound(Sounds.RollSound);
            }
        }

        public void GoProne()
        {
            if (LockAnimation)
                return;

            if (CurrentMovementInput == MovementInputs.Left || CurrentMovementInput == MovementInputs.Right)
            {
            }
            else
            {
                CurrentStanceAnimations = ProneAnimations;
                SetRobotAnimation(CurrentStanceAnimations.GetDefaultAnimationName());
                if (Sounds.ProneStartSound != UnitSounds.ProneStartSounds.None)
                {
                    PlayerSFXGenerator.PlayProneStartSound(Sounds.ProneStartSound);
                }
            }
        }

        public void SetIdle()
        {
            if (LockAnimation)
                return;

            if (CurrentStanceAnimations == CrouchAnimations && Sounds.CrouchEndSound != UnitSounds.CrouchEndSounds.None)
            {
                PlayerSFXGenerator.PlayCrouchEndSound(Sounds.CrouchEndSound);
            }
            else if (CurrentStanceAnimations == ProneAnimations && Sounds.ProneEndSound != UnitSounds.ProneEndSounds.None)
            {
                PlayerSFXGenerator.PlayProneEndSound(Sounds.ProneEndSound);
            }

            Equipment.OnIdle();

            CurrentStanceAnimations = StandingAnimations;
            SetRobotAnimation("None");
        }

        public void SetRobotAnimation(string ActiveMovementStance)
        {
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                if (ActiveWeapon.GetAnimationType(ActiveMovementStance) == AnimationTypes.FullAnimation)
                {
                    ActiveWeapon.CurrentAnimation = SetAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
                }
                else
                {
                    SetAnimation(CurrentStanceAnimations.GetAnimationName(ActiveMovementStance));
                    if (ActiveWeapon.CurrentAnimation == null)
                    {
                        ActivatePartialWeapon(ActiveWeapon, ActiveWeapon.GetAnimationName(ActiveMovementStance));
                    }
                }
            }
        }

        public void Jump()
        {
            CurrentMovementInput = MovementInputs.Up;
            Equipment.OnJump();
        }

        public void StopJump()
        {
            Equipment.OnStopJump();
        }

        public void UseJetpack(GameTime gameTime)
        {
            Equipment.OnJetpackUse(gameTime);
            IsInAir = true;
        }

        public void Land()
        {
            if (IsInAir)
            {
                IsInAir = false;
                Equipment.OnLand();
                UpdateSkills(TripleThunderRobotRequirement.OnGroundCollisionName);
            }
        }

        public void Fall()
        {
            IsInAir = true;
            Equipment.OnFall();
        }

        public override void Update(GameTime gameTime)
        {
            ActiveMovementStance = "None";
            CurrentMovementInput = MovementInputs.None;

            base.Update(gameTime);
        }

        public virtual void Update(GameTime gameTime, Dictionary<uint, RobotAnimation> DicRobot)
        {
            if (IsOnGround)
            {
                LastPositionOnGround = Position;
            }

            ActiveMovementStance = "None";
            CurrentMovementInput = MovementInputs.None;

            InputManager.Update(gameTime);
            UpdateSkills(TripleThunderRobotRequirement.OnStepName);

            if (CollisionBetweenRobot)
            {
                UpdateRobotCollisionWithRobot(this, DicRobot);
            }

            if (!UpdateRobotCollisionWithWorld(this))
            {
                Move(Speed);
            }

            CheckForWeaponDrop();

            if (IsDynamic)
            {
                //Ground.X is always positive.
                if (NormalizedGroundVector.X < 0)
                    NormalizedGroundVector = -NormalizedGroundVector;

                if (Speed.X > Friction)
                    Speed.X -= Friction;
                else if (Speed.X < -Friction)
                    Speed.X += Friction;
                else
                    Speed.X = 0;

                Equipment.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected void UpdateRobotCollisionWithRobot(RobotAnimation ActiveRobot, Dictionary<uint, RobotAnimation> DicRobot)
        {
            PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);

            foreach (KeyValuePair<uint, RobotAnimation> EnemyRobot in DicRobot)
            {
                if (ActiveRobot == EnemyRobot.Value)
                    continue;

                foreach (Polygon ActiveCollision in ActiveRobot.Collision.ListCollisionPolygon)
                {
                    foreach (Polygon EnemyCollision in EnemyRobot.Value.Collision.ListCollisionPolygon)
                    {
                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActiveCollision, EnemyCollision, ActiveRobot.Speed);

                        if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance < FinalCollisionResult.Distance))
                            FinalCollisionResult = CollisionResult;
                    }
                }
            }
            Vector2 FinalMovement;
            if (FinalCollisionResult.Distance >= 0)
            {
                Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
                FinalMovement = ActiveRobot.Speed + MovementCorection;
            }
            else
            {
                FinalMovement = ActiveRobot.Speed;
            }
            ActiveRobot.Move(FinalMovement);
        }

        public bool UpdateRobotCollisionWithWorld(RobotAnimation ActiveRobot)
        {
            bool HasCollided = false;
            ActiveRobot.ListCollidingGroundPolygon.Clear();

            List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon;
            List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon;
            List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon;
            List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon;
            CurrentLayer.GetCollidingWorldPolygon(ActiveRobot, out ListAllCollidingPolygon, out ListFloorCollidingPolygon, out ListCeilingCollidingPolygon, out ListWallCollidingPolygon);

            Equipment.OnAnyCollision(ListAllCollidingPolygon);

            //Floor
            if (ListFloorCollidingPolygon.Count > 0)
            {
                Equipment.OnFloorCollision(ListFloorCollidingPolygon);

                HasCollided = true;
            }
            //Ceiling
            if (ListCeilingCollidingPolygon.Count > 0)
            {
                Equipment.OnCeilingCollision(ListCeilingCollidingPolygon);

                HasCollided = true;
            }
            //Wall
            if (ListWallCollidingPolygon.Count > 0)
            {
                Equipment.OnWallCollision(ListWallCollidingPolygon);

                HasCollided = true;
            }

            if (ListAllCollidingPolygon.Count == 0)
            {
                if (ActiveRobot.IsOnGround)
                {
                    ActiveRobot.Fall();
                }

                ActiveRobot.ListIgnoredGroundPolygon.Clear();
                ActiveRobot.NormalizedGroundVector = new Vector2(-GravityVector.Y, GravityVector.X);
            }

            return HasCollided;
        }

        private void CheckForWeaponDrop()
        {
            foreach (WeaponDrop ActiveWeaponDrop in CurrentLayer.DicWeaponDrop.Values)
            {
                if (!ActiveWeaponDrop.IsAlive || ActiveWeaponDrop.TimeAlive <= 1)
                    return;

                PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);

                foreach (Polygon ActiveCollision in Collision.ListCollisionPolygon)
                {
                    foreach (Polygon EnemyCollision in ActiveWeaponDrop.Collision.ListCollisionPolygon)
                    {
                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActiveCollision, EnemyCollision, Speed);

                        if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance < FinalCollisionResult.Distance))
                            FinalCollisionResult = CollisionResult;
                    }
                }

                if (FinalCollisionResult.Distance >= 0)
                {
                    if (PrimaryWeapons.HolsteredWeaponsCount == 0)
                    {
                        ReplacePrimaryWeapon(ActiveWeaponDrop.WeaponName);
                        ActiveWeaponDrop.IsAlive = false;
                        CurrentLayer.RemoveDroppedWeapon(ActiveWeaponDrop);
                        return;
                    }
                }
            }
        }

        public void UpdateSkills(string RequirementName)
        {
            CurrentLayer.SetRobotContext(this);

            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                ActiveWeapon.UpdateSkills(RequirementName);
            }
        }

        public double GetMapVariable(string VariableName)
        {
            return CurrentLayer.GetMapVariable(VariableName);
        }

        public void SetRobotContext(WeaponBase ActiveWeapon, float Angle, Vector2 Position)
        {
            CurrentLayer.SetRobotContext(this, ActiveWeapon, Angle, Position);
        }

        public void SetAttackContext(Projectile2D ActiveAttackBox, RobotAnimation Owner, float Angle, Vector2 Position)
        {
            CurrentLayer.SetAttackContext(ActiveAttackBox, Owner, Angle, Position);
        }

        public void Freefall(GameTime gameTime)
        {
            Equipment.OnJetpackRest(gameTime);

            if (IsInAir)
            {
                ActiveMovementStance = "Airborne";
                NormalizedPerpendicularGroundVector = GravityVector;
            }
        }

        public void InitiateAttack(GameTime gameTime, AttackInputs AttackInput)
        {
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                if (ActiveWeapon.InitiateAttack(gameTime, AttackInput, CurrentMovementInput, ActiveMovementStance, false, this))
                    break;
            }
        }

        public void ChangeWeapon(int WeaponIndex)
        {
            if (WeaponIndex == -1)//Unequip weapon
            {
                foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
                {
                    RemovePartialAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
                }

                PrimaryWeapons.RemoveAllActiveWeapons();
                PrimaryWeapons.UseWeapon(CurrentStanceAnimations);
            }
            else
            {
                string WeaponName = PrimaryWeapons.GetWeaponName(WeaponIndex);
                PrimaryWeapons.RemoveAllActiveWeapons();
                PrimaryWeapons.UseWeapon(WeaponName);
                WeaponBase WeaponToUse = PrimaryWeapons.GetWeapon(WeaponName);

                WeaponToUse.CurrentAnimation = null;
                WeaponToUse.ResetAnimation(ActiveMovementStance);
                WeaponToUse.InitiateFollowingAttack(true, ActiveMovementStance, this);

                if (WeaponToUse.CurrentAnimation == null)
                {
                    ActivatePartialWeapon(WeaponToUse, WeaponToUse.GetAnimationName(ActiveMovementStance));
                }
            }
        }

        public void HolsterAndReplaceWeapon(WeaponBase WeaponToUse)
        {
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                RemovePartialAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
            }

            PrimaryWeapons.HolsterAllActiveWeapons();
            PrimaryWeapons.UseWeapon(WeaponToUse);

            WeaponToUse.CurrentAnimation = null;
            WeaponToUse.ResetAnimation(ActiveMovementStance);
            WeaponToUse.InitiateFollowingAttack(true, ActiveMovementStance, this);

            if (WeaponToUse.CurrentAnimation == null)
            {
                ActivatePartialWeapon(WeaponToUse, WeaponToUse.GetAnimationName(ActiveMovementStance));
            }
        }

        public void UnholsterWeaponsIfNeeded()
        {
            if (PrimaryWeapons.HasHolsteredWeapons)
            {
                if (PrimaryWeapons.ActiveWeapons.Contains(SecondaryWeapons.ActiveWeapons[0]))
                {
                    foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
                    {
                        RemovePartialAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
                    }
                    PrimaryWeapons.RemoveAllActiveWeapons();
                }

                List<WeaponBase> ListUnHolsteredWeapon = PrimaryWeapons.UseHolsteredWeapons();

                foreach (WeaponBase WeaponToUse in ListUnHolsteredWeapon)
                {
                    WeaponToUse.CurrentAnimation = null;
                    WeaponToUse.ResetAnimation(ActiveMovementStance);
                    WeaponToUse.InitiateFollowingAttack(true, ActiveMovementStance, this);

                    if (WeaponToUse.CurrentAnimation == null)
                    {
                        ActivatePartialWeapon(WeaponToUse, WeaponToUse.GetAnimationName(ActiveMovementStance));
                    }
                }
            }
        }

        public void DropActiveWeapons()
        {
            foreach (WeaponDrop DroppedWeapon in PrimaryWeapons.DropActiveWeapon(Position, CurrentLayer))
            {
                CurrentLayer.AddDroppedWeapon(DroppedWeapon);
            }

            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                RemovePartialAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
            }

            PrimaryWeapons.UseWeapon(CurrentStanceAnimations);
        }

        public void ReplacePrimaryWeapon(string WeaponToPickUpName)
        {
            if (PrimaryWeapons.HasActiveWeapon(WeaponToPickUpName))
            {
                return;
            }

            if (PrimaryWeapons.HasActiveWeapons)
            {
                foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
                {
                    RemovePartialAnimation(ActiveWeapon.GetAnimationName(ActiveMovementStance));
                }

                PrimaryWeapons.DropActiveWeapon();
            }

            EquipWeapon(WeaponToPickUpName);
        }

        public void EquipWeapon(string WeaponToEquipName)
        {
            WeaponBase NewWeapon;
            string WeaponPath = Name + "/Weapons/" + WeaponToEquipName;
            if (!File.Exists("Content/Triple Thunder/Weapons/" + WeaponPath + ".ttw"))
            {
                WeaponPath = Name + "/Grenades/" + WeaponToEquipName;
            }

            if (CurrentLayer == null)
            {
                NewWeapon = WeaponBase.CreateFromFile(Name, WeaponPath, false, null, null, null);
            }
            else
            {
                NewWeapon = WeaponBase.CreateFromFile(Name, WeaponPath, false, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
            }

            NewWeapon.WeaponName = WeaponToEquipName;
            NewWeapon.Load(Content);
            PrimaryWeapons.AddWeaponToStash(NewWeapon);

            ChangeWeapon(0);

            WeaponBase WeaponToUse = PrimaryWeapons.GetWeapon(WeaponPath);
            WeaponToUse.CurrentAnimation = null;
            WeaponToUse.ResetAnimation(ActiveMovementStance);

            WeaponToUse.InitiateFollowingAttack(true, ActiveMovementStance, this);

            if (WeaponToUse.CurrentAnimation == null)
            {
                ActivatePartialWeapon(WeaponToUse, WeaponToUse.GetAnimationName(ActiveMovementStance));
            }
        }

        public void FallThroughFloor()
        {
            foreach (WorldPolygon ActivePolygon in ListCollidingGroundPolygon)
            {
                if (!ListIgnoredGroundPolygon.Contains(ActivePolygon))
                    ListIgnoredGroundPolygon.Add(ActivePolygon);
            }
        }

        public void Move(Vector2 Movement)
        {
            Position += Movement;
            Collision.Position = Position;
            TotalMovementThisFrame += Movement;

            foreach (Polygon ActiveCollision in Collision.ListCollisionPolygon)
            {
                ActiveCollision.Offset(Movement.X, Movement.Y);
            }
        }

        public void Charge(bool UseSecondaryWeapon, int MaxCharge, int ChargeAmountPerFrame)
        {
            if (UseSecondaryWeapon)
            {
                SecondaryWeapons.ChargePrimaryWeapon(ChargeAmountPerFrame, MaxCharge);
            }
            else
            {
                PrimaryWeapons.ChargePrimaryWeapon(ChargeAmountPerFrame, MaxCharge);
            }
        }
        
        public void Shoot(Vector2 GunNozzlePosition, bool UseSecondaryWeapon)
        {
            if (UseSecondaryWeapon)
            {
                int i = 0;
                foreach (WeaponBase ActiveWeapon in SecondaryWeapons.ActiveWeapons)
                {
                    Shoot(GunNozzlePosition, ActiveWeapon, i++);
                }

                SecondaryWeapons.ResetPrimaryWeaponCharge();
            }
            else
            {
                int i = 0;
                foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
                {
                    Shoot(GunNozzlePosition, ActiveWeapon, i++);
                }

                PrimaryWeapons.ResetPrimaryWeaponCharge();
            }
        }

        public void Shoot(Vector2 GunNozzlePosition, WeaponBase ActiveWeapon, int WeaponIndex)
        {
            bool CanShoot;

            if (ActiveWeapon.AmmoPerMagazine > 0)
            {
                if (ActiveWeapon.AmmoCurrent > 0)
                {
                    --ActiveWeapon.AmmoCurrent;
                    CanShoot = true;
                }
                else
                {
                    CanShoot = false;
                }
            }
            else
            {
                CanShoot = true;
            }

            VisibleTimeline WeaponSlotTimeline;
            PartialAnimation WeaponAnimation;

            if (CanShoot)
            {
                float OffsetX = GunNozzlePosition.X - AnimationOrigin.Position.X;
                float OffsetY = GunNozzlePosition.Y - AnimationOrigin.Position.Y;

                if (ActiveWeapon.CurrentAnimation != null && DicPartialAnimation.TryGetValue(ActiveWeapon.CurrentAnimation.AnimationPath, out WeaponAnimation))
                {
                    OffsetX = GunNozzlePosition.X - WeaponAnimation.AnimationOrigin.Position.X;
                    OffsetY = GunNozzlePosition.Y - WeaponAnimation.AnimationOrigin.Position.Y;
                }

                float WeaponOffsetX = 0;
                float WeaponOffsetY = 0;
                if (DicActiveAnimationObject.TryGetValue("Weapon Slot " + (WeaponIndex + 1), out WeaponSlotTimeline))
                {
                    WeaponOffsetX = WeaponSlotTimeline.Position.X - AnimationOrigin.Position.X;
                    WeaponOffsetY = WeaponSlotTimeline.Position.Y - AnimationOrigin.Position.Y;
                }

                float Angle = ActiveWeapon.WeaponAngle;

                if (ActiveSpriteEffects != SpriteEffects.FlipHorizontally)
                {
                    WeaponOffsetX = -WeaponOffsetX;
                }

                double LenghtDirX = Math.Cos(Angle) * OffsetX;
                double LenghtDirY = Math.Sin(Angle) * OffsetX;
                double LenghtDirX2 = Math.Cos(Angle + MathHelper.ToRadians(90)) * OffsetY;
                double LenghtDirY2 = Math.Sin(Angle + MathHelper.ToRadians(90)) * OffsetY;

                Vector2 RealGunNozzlePosition = Position + new Vector2(WeaponOffsetX, WeaponOffsetY)
                    - new Vector2((float)(LenghtDirX + LenghtDirX2), (float)(LenghtDirY + LenghtDirY2));

                SetRobotContext(ActiveWeapon, Angle, RealGunNozzlePosition);

                foreach (MagicSpell ActiveSpell in ListMagicSpell)
                {
                    ActiveSpell.ExecuteSpell();
                }

                if (ActiveWeapon.HasSkills)
                {
                    ActiveWeapon.UpdateSkills("Shoot");
                }
                else
                {
                    ActiveWeapon.Shoot(this, RealGunNozzlePosition, Angle, new List<BaseAutomaticSkill>());
                }

                CreateNozzleFlashAnimation(ActiveWeapon.NozzleFlashAnimation, RealGunNozzlePosition, Angle);
            }
        }

        public void CreateNozzleFlashAnimation(SimpleAnimation NozzleFlashAnimation, Vector2 Position, float Angle)
        {
            SimpleAnimation NewVisualEffect = NozzleFlashAnimation.Copy();
            NewVisualEffect.Position = Position;
            NewVisualEffect.Angle = Angle;
            CurrentLayer.ListVisualEffects.Add(NewVisualEffect);
        }

        public void CreateAttackBox(AttackBox NewAttackBox)
        {
            CurrentLayer.AddProjectile(NewAttackBox);
        }

        public void CreateAttackBox(string WeaponName, Vector2 GunNozzlePosition, List<AttackBox> ListAttack)
        {
            CurrentLayer.AddProjectile(ID, WeaponName, GunNozzlePosition, ListAttack);
        }

        public void CreateExplosion(Vector2 ExplosionCenter, ExplosionOptions ExplosionAttributes, Vector2 CollisionGroundResult)
        {
            CurrentLayer.CreateExplosion(ExplosionCenter, this, ExplosionAttributes, CollisionGroundResult);
        }

        public void SendOnlineSFX(Vector2 SFXPosition, CreateSFXScriptClient.SFXTypes SFXType)
        {
            CurrentLayer.SendOnlineSFX(SFXPosition, SFXType);
        }

        public void SendOnlineVFX(Vector2 VFXPosition, Vector2 VFXSpeed, CreateVFXScriptClient.VFXTypes VFXType)
        {
            CurrentLayer.SendOnlineVFX(VFXPosition, VFXSpeed, VFXType);
        }

        public void Reload()
        {
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                ActiveWeapon.Reload(ActiveMovementStance, this);
            }
        }

        public void CreateCollisionBox(List<Polygon> ListNewCollisionPolygon)
        {
            foreach (Polygon ActiveCollisionPolygon in ListNewCollisionPolygon)
            {
                Vector2 Distance = (ActiveCollisionPolygon.Center - AnimationOrigin.Position) * Scale;

                ActiveCollisionPolygon.Offset(Position.X - ActiveCollisionPolygon.Center.X + Distance.X,
                                                            Position.Y - ActiveCollisionPolygon.Center.Y + Distance.Y);

                ActiveCollisionPolygon.Scale(Scale);

                CollisionBox.ListCollisionPolygon.Add(ActiveCollisionPolygon);
            }
        }

        public void DeleteCollisionBox(List<Polygon> ListNewCollisionPolygon)
        {
            foreach (Polygon ActiveCollisionPolygon in ListNewCollisionPolygon)
            {
                CollisionBox.ListCollisionPolygon.Remove(ActiveCollisionPolygon);
            }
        }

        public Rectangle GetCollisionSize()
        {
            Rectangle CollisionSize = new Rectangle(0, 0, 1, 1);
            foreach (var a in Collision.ListCollisionPolygon)
            {
            }

            return CollisionSize;
        }

        #region Animation class methods

        public override void OnMarkerTimelineSpawn(AnimationLayer ActiveLayer, MarkerTimeline ActiveMarker)
        {
            base.OnMarkerTimelineSpawn(ActiveLayer, ActiveMarker);

            ActiveMarker.UpdateAnimationObject(ActiveMarker.SpawnFrame);
        }

        protected override void OnLoopEnd()
        {
            LockAnimation = false;
            base.OnLoopEnd();
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                if (ActiveWeapon.GetAnimationType(ActiveMovementStance) == AnimationTypes.FullAnimation)
                {
                    ActiveWeapon.InitiateFollowingAttack(false, ActiveMovementStance, this);
                }
            }
        }

        protected override void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation)
        {
            RemovePartialAnimation(ActivePartialAnimation);
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                ActiveWeapon.OnPartialAnimationLoopEnd(ActivePartialAnimation, ActiveMovementStance, this);
            }
            foreach (WeaponBase ActiveWeapon in SecondaryWeapons.ActiveWeapons)
            {
                if (ActiveWeapon.CurrentAnimation == ActivePartialAnimation)
                {
                    ActiveWeapon.CurrentAnimation = null;
                    ActiveWeapon.InitiateFollowingAttack(true, ActiveMovementStance, this);
                }
                else
                {

                }
            }
        }

        public void ActivatePartialWeapon(WeaponBase ActiveWeapon, string AnimationName)
        {
            ActiveAddedWeapon = ActiveWeapon;
            ActiveWeapon.CurrentAnimation = AddPartialAnimation(AnimationName);
            ActiveWeapon.CurrentAnimation.UpdateKeyFrame(ActiveWeapon.CurrentAnimation.ActiveKeyFrame);
            ActiveAddedWeapon = null;
        }

        #endregion

        public WeaponBase CreateWeapon(string WeaponName)
        {
            WeaponBase NewWeapon = WeaponBase.CreateFromFile(Name, WeaponName, true, CurrentLayer.DicRequirement, CurrentLayer.DicEffect, CurrentLayer.DicAutomaticSkillTarget);
            NewWeapon.Load(Content);
            return NewWeapon;
        }

        public void ChangeLayer(Layer TargetLayer)
        {
            if (CurrentLayer != null && TargetLayer != null && CurrentLayer != TargetLayer)
            {
                for (int V = 0; V < CurrentLayer.GroundLevelCollision.ArrayVertex.Length - 1; V++)
                {
                    if (Position.X >= CurrentLayer.GroundLevelCollision.ArrayVertex[V].X && Position.X < CurrentLayer.GroundLevelCollision.ArrayVertex[V + 1].X)
                    {
                        Vector2 VertexLength = CurrentLayer.GroundLevelCollision.ArrayVertex[V + 1] - CurrentLayer.GroundLevelCollision.ArrayVertex[V];
                        float RealPosX = Position.X - CurrentLayer.GroundLevelCollision.ArrayVertex[V].X;
                        float ScaleX = RealPosX / VertexLength.X;
                        float GroundY = ScaleX * VertexLength.Y + CurrentLayer.GroundLevelCollision.ArrayVertex[V].Y;
                        float DistanceFromGround = GroundY - Position.Y;

                        for (int V2 = 0; V2 < TargetLayer.GroundLevelCollision.ArrayVertex.Length - 1; V2++)
                        {
                            if (Position.X >= TargetLayer.GroundLevelCollision.ArrayVertex[V2].X && Position.X < TargetLayer.GroundLevelCollision.ArrayVertex[V2 + 1].X)
                            {
                                Vector2 VertexLength2 = TargetLayer.GroundLevelCollision.ArrayVertex[V2 + 1] - TargetLayer.GroundLevelCollision.ArrayVertex[V2];
                                float RealPosX2 = Position.X - TargetLayer.GroundLevelCollision.ArrayVertex[V2].X;
                                float ScaleX2 = RealPosX2 / VertexLength2.X;
                                float GroundY2 = ScaleX2 * VertexLength2.Y + TargetLayer.GroundLevelCollision.ArrayVertex[V2].Y;

                                float NewY = GroundY2 - DistanceFromGround;
                                Position.Y = NewY;

                                CurrentLayer = TargetLayer;
                                return;
                            }
                        }
                    }
                }
            }

            CurrentLayer = TargetLayer;
        }

        public void UpdateAllWeaponsAngle(Vector2 Target)
        {
            int W = 0;
            foreach (WeaponBase ActiveWeapon in PrimaryWeapons.ActiveWeapons)
            {
                bool RotateTowardMouse = ActiveWeapon.CanRotateTowardMouse(ActiveMovementStance);

                float Angle = 0;
                if (RotateTowardMouse)
                {
                    Angle = (float)Math.Atan2(Target.Y, Target.X);
                }

                ActiveWeapon.WeaponAngle = Angle;

                if (Angle <= -MathHelper.PiOver2 || Angle >= MathHelper.PiOver2)
                {
                    ActiveSpriteEffects = SpriteEffects.None;
                }
                else
                {
                    ActiveSpriteEffects = SpriteEffects.FlipHorizontally;
                    Angle = MathHelper.Pi - Angle;
                }

                UpdatePrimaryWeaponAngle(Angle + MathHelper.Pi, W++);
            }

            W = 0;
            foreach (WeaponBase ActiveWeapon in SecondaryWeapons.ActiveWeapons)
            {
                bool RotateTowardMouse = ActiveWeapon.CanRotateTowardMouse(ActiveMovementStance);

                float Angle = 0;
                if (RotateTowardMouse)
                {
                    Angle = (float)Math.Atan2(Target.Y, Target.X);
                }

                ActiveWeapon.WeaponAngle = Angle;

                if (Angle <= -MathHelper.PiOver2 || Angle >= MathHelper.PiOver2)
                {
                    ActiveSpriteEffects = SpriteEffects.None;
                }
                else
                {
                    ActiveSpriteEffects = SpriteEffects.FlipHorizontally;
                    Angle = MathHelper.Pi - Angle;
                }

                UpdateSecondaryWeaponAngle(Angle, W++);
            }
        }

        public void UpdatePrimaryWeaponAngle(float Angle, int WeaponIndex)
        {
            WeaponBase ActiveWeapon = PrimaryWeapons.ActiveWeapons[WeaponIndex];

            VisibleTimeline WeaponSlotTimeline;
            DicActiveAnimationObject.TryGetValue("Weapon Slot " + (WeaponIndex + 1), out WeaponSlotTimeline);
            ActiveWeapon.UpdateWeaponAngle(Angle, ActiveMovementStance, WeaponSlotTimeline, this);
        }

        public void UpdateSecondaryWeaponAngle(float Angle, int WeaponIndex)
        {
            WeaponBase ActiveWeapon = SecondaryWeapons.ActiveWeapons[WeaponIndex];

            VisibleTimeline WeaponSlotTimeline;
            DicActiveAnimationObject.TryGetValue("Weapon Slot " + (WeaponIndex + 1), out WeaponSlotTimeline);
            ActiveWeapon.UpdateWeaponAngle(Angle, ActiveMovementStance, WeaponSlotTimeline, this);
        }
    }
}
