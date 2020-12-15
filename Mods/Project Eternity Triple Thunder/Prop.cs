using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class RobotSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathUnitsTripleThunder, "Select Robot", false);
                if (Items != null)
                {
                    value = Items[0].Substring(0, Items[0].Length - 4).Substring(29);
                }
            }
            return value;
        }
    }

    public class VehicleSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathUnitsVehicleTripleThunder, "Select Vehicle", false);
                if (Items != null)
                {
                    value = Items[0].Substring(0, Items[0].Length - 5).Substring(38);
                }
            }
            return value;
        }
    }

    public class WeaponSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathTripleThunderWeapons, "Select every Weapons to use", true);

                if (Items != null)
                {
                    for (int W = Items.Count - 1; W >= 0; --W)
                    {
                        Items[W] = Items[W].Substring(0, Items[W].Length - 4).Substring(31);
                    }

                    value = Items;
                }
            }
            return value;
        }
    }

    public abstract class Prop
    {
        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The position of the prop"),
        DefaultValueAttribute("")]
        public Vector2 _Position;
        public float Depth;
        public readonly string Name;
        public readonly bool CanRunOnServer;
        public bool HasEnded;
        protected Layer Owner;
        protected FightingZone Map;

        public static Prop[] GetAllProps()
        {
            return new Prop[] { new RobotAIProp(), new VehicleProp(), new JumpPadProp(), new TeleportProp(), new NextLevelProp() };
        }

        public static Dictionary<string, Prop> GetAllPropsByName()
        {
            Dictionary<string, Prop> DicPropsByName = new Dictionary<string, Prop>();
            foreach (Prop ActiveProp in GetAllProps())
            {
                DicPropsByName.Add(ActiveProp.Name, ActiveProp);
            }

            return DicPropsByName;
        }

        protected Prop(string Name, bool CanRunOnServer = false)
        {
            this.Name = Name;
            this.CanRunOnServer = CanRunOnServer;
            HasEnded = false;
        }

        public void Load(BinaryReader BR, ContentManager Content, Layer Owner, FightingZone Map)
        {
            this.Owner = Owner;
            this.Map = Map;
            _Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            Depth = BR.ReadInt32();

            DoLoad(BR, Content);
        }

        protected abstract void DoLoad(BinaryReader BR, ContentManager Content);

        public void Save(BinaryWriter BW)
        {
            BW.Write(_Position.X);
            BW.Write(_Position.Y);
            BW.Write(Depth);

            DoSave(BW);
        }

        protected abstract void DoSave(BinaryWriter BW);

        public Prop Copy()
        {
            Prop NewProp = DoCopy();
            NewProp.Owner = Owner;

            return NewProp;
        }

        public abstract Prop DoCopy();

        public abstract void Update(GameTime gameTime);

        public abstract void BeginDraw(CustomSpriteBatch g);

        public abstract void Draw(CustomSpriteBatch g);

        public abstract void EndDraw(CustomSpriteBatch g);

        public override string ToString()
        {
            return Name;
        }
    }

    public class RobotAIProp : Prop
    {
        private int _Team;
        private string _AIPath;
        private string _RobotPath;
        private List<string> _ListWeapons;
        private string _ItemToDrop;
        private int _RespawnOnDeathLimit;
        private bool _KillRequiredToProgress;

        private RobotAnimation NewRobot;

        public RobotAIProp()
            : base("Robot AI")
        {
            _Team = 0;
            _AIPath = string.Empty;
            _RobotPath = string.Empty;
            _ListWeapons = new List<string>();
        }

        protected override void DoLoad(BinaryReader BR, ContentManager Content)
        {
            _Team = BR.ReadInt32();
            _AIPath = BR.ReadString();
            _RobotPath = BR.ReadString();

            int ListWeaponsCount = BR.ReadInt32();
            _ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
                _ListWeapons.Add(BR.ReadString());

            if (string.IsNullOrEmpty(_RobotPath))
                return;

            List<Weapon> ListExtraWeapon = new List<Weapon>();
            for (int W = 0; W < _ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new Weapon(_ListWeapons[W], Owner.DicRequirement, Owner.DicEffect));
            }

            NewRobot = new RobotAnimation(_RobotPath, Owner, _Position, _Team, new PlayerEquipment(), Owner.PlayerSFXGenerator, ListExtraWeapon);

            HasEnded = true;
            if (!string.IsNullOrEmpty(AIPath))
            {
                NewRobot.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(NewRobot, Owner, Map));
                NewRobot.RobotAI.Load(AIPath);
            }

            Owner.SpawnRobot(NewRobot);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_Team);
            BW.Write(_AIPath);
            BW.Write(_RobotPath);

            BW.Write(_ListWeapons.Count);
            for (int W = 0; W < _ListWeapons.Count; ++W)
                BW.Write(_ListWeapons[W]);
        }

        public override Prop DoCopy()
        {
            return new RobotAIProp();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            if (NewRobot != null)
            {
                NewRobot.BeginDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (NewRobot != null)
            {
                NewRobot.Position = Position;
                NewRobot.Update(new GameTime(new TimeSpan(), new TimeSpan()));
                NewRobot.Draw(g, Vector2.Zero);
            }
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            if (NewRobot != null)
            {
                NewRobot.EndDraw(g);
            }
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The spawning position of the Robot"),
        DefaultValueAttribute("")]
        public Vector2 Position { get { return _Position; } set { _Position = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The Team of the Robot"),
        DefaultValueAttribute("")]
        public int Team { get { return _Team; } set { _Team = value; } }

        [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute("")]
        public string AIPath { get { return _AIPath; } set { _AIPath = value; } }

        [Editor(typeof(RobotSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The robot path"),
        DefaultValueAttribute("")]
        public string RobotPath { get { return _RobotPath; } set { _RobotPath = value; } }

        [Editor(typeof(WeaponSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("All the Weapons used by the enemy"),
        DefaultValueAttribute("")]
        public List<string> ListWeapons { get { return _ListWeapons; } set { _ListWeapons = value; } }

        #endregion
    }

    public class VehicleProp : Prop
    {
        public int _Team;
        public string _AIPath;
        public string _VehiclePath;
        public List<string> _ListWeapons;

        public VehicleProp()
            : base("Vehicle")
        {
            _Team = 0;
            _AIPath = string.Empty;
            _VehiclePath = string.Empty;
            _ListWeapons = new List<string>();
        }

        protected override void DoLoad(BinaryReader BR, ContentManager Content)
        {
            _Team = BR.ReadInt32();
            _AIPath = BR.ReadString();
            _VehiclePath = BR.ReadString();

            int ListWeaponsCount = BR.ReadInt32();
            _ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
                _ListWeapons.Add(BR.ReadString());

            if (string.IsNullOrEmpty(_VehiclePath))
                return;

            List<Weapon> ListExtraWeapon = new List<Weapon>();
            for (int W = 0; W < _ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new Weapon(_ListWeapons[W], Owner.DicRequirement, Owner.DicEffect));
            }

            Vehicle NewVehicle = new Vehicle(_VehiclePath, Owner, _Position, _Team, new PlayerEquipment(), Owner.PlayerSFXGenerator, ListExtraWeapon);

            HasEnded = true;
            if (!string.IsNullOrEmpty(AIPath))
            {
                NewVehicle.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(NewVehicle, Owner, Map));
                NewVehicle.RobotAI.Load(AIPath);
            }

            Owner.SpawnVehicle(NewVehicle);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_Team);
            BW.Write(_AIPath);
            BW.Write(_VehiclePath);

            BW.Write(_ListWeapons.Count);
            for (int W = 0; W < _ListWeapons.Count; ++W)
                BW.Write(_ListWeapons[W]);
        }

        public override Prop DoCopy()
        {
            return new VehicleProp();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The spawning position of the Robot"),
        DefaultValueAttribute("")]
        public Vector2 Position { get { return _Position; } set { _Position = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The Team of the Robot"),
        DefaultValueAttribute("")]
        public int Team { get { return _Team; } set { _Team = value; } }

        [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute("")]
        public string AIPath { get { return _AIPath; } set { _AIPath = value; } }

        [Editor(typeof(VehicleSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The vehicle path"),
        DefaultValueAttribute("")]
        public string VehiclePath { get { return _VehiclePath; } set { _VehiclePath = value; } }

        [Editor(typeof(WeaponSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("All the Weapons used by the vehicle"),
        DefaultValueAttribute("")]
        public List<string> ListWeapons { get { return _ListWeapons; } set { _ListWeapons = value; } }

        #endregion
    }

    public class JumpPadProp : Prop
    {
        private SimpleAnimation JumpPadAnimation;
        private Polygon JumpPadPolygon;
        private float _JumpSpeed;

        public JumpPadProp()
            : base("Jump Pad")
        {
            _JumpSpeed = 15;
        }

        protected override void DoLoad(BinaryReader BR, ContentManager Content)
        {
            _JumpSpeed = BR.ReadSingle();

            float MinX;
            float MinY;
            float MaxX;
            float MaxY;

            if (Content != null)
            {
                JumpPadAnimation = new SimpleAnimation("Animations/Sprites/Jump Pad_strip4", Content);

                Rectangle CollisionBox = JumpPadAnimation.PositionRectangle;

                MinX = _Position.X - CollisionBox.Width / 2f;
                MinY = _Position.Y - CollisionBox.Height / 2f;
                MaxX = MinX + CollisionBox.Width;
                MaxY = MinY + CollisionBox.Height;
            }
            else
            {
                MinX = _Position.X - 64 / 2;
                MinY = _Position.Y - 25 / 2;
                MaxX = MinX + 64 / 2;
                MaxY = MinY + 25 / 2;
            }

            JumpPadPolygon = new Polygon();
            JumpPadPolygon.ArrayVertex = new Vector2[4];
            JumpPadPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            JumpPadPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
            JumpPadPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            JumpPadPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

            JumpPadPolygon.ComputePerpendicularAxis();
            JumpPadPolygon.ComputerCenter();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_JumpSpeed);
        }

        public override Prop DoCopy()
        {
            return new JumpPadProp();
        }

        public override void Update(GameTime gameTime)
        {
            JumpPadAnimation.Update(gameTime);

            foreach (RobotAnimation ActiveRobot in Owner.DicRobot.Values)
            {
                PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);
                foreach (CollisionPolygon EnemyCollision in ActiveRobot.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(JumpPadPolygon, EnemyCollision.ActivePolygon, ActiveRobot.Speed);

                    if (FinalCollisionResult.Distance < 0 || (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance))
                    {
                        FinalCollisionResult = CollisionResult;
                    }
                }

                if (FinalCollisionResult.Distance >= 0)
                {
                    ActiveRobot.Speed.Y = -_JumpSpeed;
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }
        public override void Draw(CustomSpriteBatch g)
        {
            if (JumpPadAnimation != null)
                JumpPadAnimation.Draw(g, _Position);
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
        }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The spawning position of the Robot"),
        DefaultValueAttribute("")]
        public float JumpSpeed { get { return _JumpSpeed; } set { _JumpSpeed = value; } }
    }

    public class TeleportProp : Prop
    {
        private SimpleAnimation TeleportAnimation;
        private Polygon TeleportPolygon;
        private int _LayerIndex;
        private Vector2 _TeleportOffset;
        private Vector2 _TeleportSpeed;

        public TeleportProp()
            : base("Teleport")
        {
        }

        protected override void DoLoad(BinaryReader BR, ContentManager Content)
        {
            if (Content != null)
            {
                TeleportAnimation = new SimpleAnimation("Animations/Sprites/efx_Warp_strip9_3", Content);

                Rectangle CollisionBox = TeleportAnimation.PositionRectangle;

                float MinX = _Position.X - CollisionBox.Width / 2f;
                float MinY = _Position.Y - CollisionBox.Height / 2f;
                float MaxX = MinX + CollisionBox.Width;
                float MaxY = MinY + CollisionBox.Height;

                TeleportPolygon = new Polygon();
                TeleportPolygon.ArrayVertex = new Vector2[4];
                TeleportPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
                TeleportPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
                TeleportPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
                TeleportPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

                TeleportPolygon.ComputePerpendicularAxis();
                TeleportPolygon.ComputerCenter();
            }

            _LayerIndex = BR.ReadInt32();
            _TeleportOffset = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _TeleportSpeed = new Vector2(BR.ReadSingle(), BR.ReadSingle());
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_LayerIndex);
            BW.Write(_TeleportOffset.X);
            BW.Write(_TeleportOffset.Y);
            BW.Write(_TeleportSpeed.X);
            BW.Write(_TeleportSpeed.Y);
        }

        public override Prop DoCopy()
        {
            return new TeleportProp();
        }

        public override void Update(GameTime gameTime)
        {
            TeleportAnimation.Update(gameTime);
            
            PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);

            foreach (RobotAnimation ActiveRobot in Owner.DicRobot.Values)
            {
                foreach (CollisionPolygon EnemyCollision in ActiveRobot.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(TeleportPolygon, EnemyCollision.ActivePolygon, ActiveRobot.Speed);

                    if (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance)
                    {
                        ActiveRobot.Speed += _TeleportSpeed;
                        ActiveRobot.Move(_TeleportOffset);
                        Owner.ChangeRobotLayer(ActiveRobot, _LayerIndex);
                    }
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (TeleportAnimation != null)
                TeleportAnimation.Draw(g, _Position);
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("Index of the layer to teleport to."),
        DefaultValueAttribute("")]
        public int LayerIndex { get { return _LayerIndex; } set { _LayerIndex = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("How many pixel to move when teleporting."),
        DefaultValueAttribute("")]
        public Vector2 TeleportOffset { get { return _TeleportOffset; } set { _TeleportOffset = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("Speed to add when teleporting."),
        DefaultValueAttribute("")]
        public Vector2 TeleportSpeed { get { return _TeleportSpeed; } set { _TeleportSpeed = value; } }

        #endregion
    }

    public class NextLevelProp : Prop
    {
        private SimpleAnimation TeleportAnimation;
        private Polygon TeleportPolygon;
        private string _NextLevelPath;

        public NextLevelProp()
            : base("Start Next Level", true)
        {
        }

        protected override void DoLoad(BinaryReader BR, ContentManager Content)
        {
            float MinX;
            float MinY;
            float MaxX;
            float MaxY;

            if (Content != null)
            {
                TeleportAnimation = new SimpleAnimation("Animations/Sprites/efx_Warp_strip9_3", Content);

                Rectangle CollisionBox = TeleportAnimation.PositionRectangle;

                MinX = _Position.X - CollisionBox.Width / 2f;
                MinY = _Position.Y - CollisionBox.Height / 2f;
                MaxX = MinX + CollisionBox.Width;
                MaxY = MinY + CollisionBox.Height;
            }
            else
            {
                MinX = _Position.X - 208 / 2;
                MinY = _Position.Y - 177 / 2;
                MaxX = MinX + 208 / 2;
                MaxY = MinY + 177 / 2;
            }

            TeleportPolygon = new Polygon();
            TeleportPolygon.ArrayVertex = new Vector2[4];
            TeleportPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            TeleportPolygon.ArrayVertex[1] = new Vector2(MaxX, MaxY);
            TeleportPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            TeleportPolygon.ArrayVertex[3] = new Vector2(MinX, MinY);

            TeleportPolygon.ComputePerpendicularAxis();
            TeleportPolygon.ComputerCenter();

            _NextLevelPath = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_NextLevelPath);
        }

        public override Prop DoCopy()
        {
            return new NextLevelProp();
        }

        public override void Update(GameTime gameTime)
        {
            if (TeleportAnimation != null)
            {
                TeleportAnimation.Update(gameTime);
            }

            if (Owner.IsOfflineOrServer)
            {
                PolygonCollisionResult FinalCollisionResult = new PolygonCollisionResult(Vector2.Zero, -1);

                foreach (RobotAnimation ActiveRobot in Owner.DicRobot.Values)
                {
                    foreach (CollisionPolygon EnemyCollision in ActiveRobot.ListCollisionPolygon)
                    {
                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(TeleportPolygon, EnemyCollision.ActivePolygon, ActiveRobot.Speed);

                        if (CollisionResult.Distance >= 0 && CollisionResult.Distance > FinalCollisionResult.Distance)
                        {
                            Owner.PrepareNextLevel(_NextLevelPath);
                            HasEnded = true;
                        }
                    }
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (TeleportAnimation != null)
                TeleportAnimation.Draw(g, _Position);
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("Path of the next level to load."),
        DefaultValueAttribute("")]
        public string NextLevelPath { get { return _NextLevelPath; } set { _NextLevelPath = value; } }

        #endregion
    }
}
