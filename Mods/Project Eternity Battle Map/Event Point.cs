using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EventPoint
    {
        public Vector3 Position;
        public string Tag;
        public byte ColorRed;
        public byte ColorGreen;
        public byte ColorBlue;
        public bool HasBeenUsed;

        public EventPoint(EventPoint Copy)
            : this(Copy.Position, Copy.Tag, Copy.ColorRed, Copy.ColorGreen, Copy.ColorBlue)
        {
        }

        public EventPoint(Vector3 Position, string Tag, byte ColorRed, byte ColorGreen, byte ColorBlue)
        {
            this.Position = Position;
            this.Tag = Tag;
            this.ColorRed = ColorRed;
            this.ColorGreen = ColorGreen;
            this.ColorBlue = ColorBlue;
        }

        public EventPoint(BinaryReader BR)
        {
            Position = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            Tag = BR.ReadString();
        }

        public virtual void Save(BinaryWriter BW)
        {
            BW.Write(Position.X);
            BW.Write(Position.Y);
            BW.Write(Position.Z);
            BW.Write(Tag);
        }

        public virtual void Draw(CustomSpriteBatch g, Vector2 Position)
        {
        }
    }

    public class PlayerEventPoint : EventPoint
    {
        public string LeaderTypeName;
        public string LeaderName;
        public string LeaderPilot;
        public string WingmanATypeName;
        public string WingmanAName;
        public string WingmanAPilot;
        public string WingmanBTypeName;
        public string WingmanBName;
        public string WingmanBPilot;

        public PlayerEventPoint(EventPoint Copy)
            : base(Copy)
        {
        }

        public PlayerEventPoint(BinaryReader BR)
            : base(BR)
        {
        }

        public PlayerEventPoint(Vector3 Position, string Tag, byte ColorRed, byte ColorGreen, byte ColorBlue)
            : base(Position, Tag, ColorRed, ColorGreen, ColorBlue)
        {
        }
    }

    public class MapSwitchPoint : EventPoint
    {
        private string _SwitchText;

        public MapSwitchPoint(Vector3 Position, string Tag, byte ColorRed, byte ColorGreen, byte ColorBlue)
            : base(Position, Tag, ColorRed, ColorGreen, ColorBlue)
        {
            SwitchMapPath = "";
            SwitchText = "";
        }

        public MapSwitchPoint(EventPoint Copy)
            : base(Copy)
        {
            SwitchMapPath = "";
            SwitchText = "";
        }

        public MapSwitchPoint(BinaryReader BR)
            : base(BR)
        {
            SwitchMapPath = BR.ReadString();
            SwitchText = BR.ReadString();
            OtherMapEntryPoint = new Point(BR.ReadInt32(), BR.ReadInt32());
            LayerIndex = BR.ReadInt32();
        }

        public override void Save(BinaryWriter BW)
        {
            base.Save(BW);

            BW.Write(SwitchMapPath);
            BW.Write(SwitchText);
            BW.Write(OtherMapEntryPoint.X);
            BW.Write(OtherMapEntryPoint.Y);
            BW.Write(LayerIndex);
        }

        #region Properties

        [Editor(typeof(Selectors.MapSelector), typeof(UITypeEditor)), 
        CategoryAttribute("Map Switch Attributes"),
        DescriptionAttribute(".")]
        public string SwitchMapPath { get; set; }

        [CategoryAttribute("Map Switch Attributes"),
        DescriptionAttribute(".")]
        public string SwitchText { get { return _SwitchText; } set { _SwitchText = value; } }

        [CategoryAttribute("Map Switch Attributes"),
        DescriptionAttribute(".")]
        public Point OtherMapEntryPoint { get; set; }

        [CategoryAttribute("Map Switch Attributes"),
        DescriptionAttribute(".")]
        public int LayerIndex { get; set; }

        #endregion
    }

    public class TeleportPoint : EventPoint
    {
        public TeleportPoint(Vector3 Position, string Tag, byte ColorRed, byte ColorGreen, byte ColorBlue)
            : base(Position, Tag, ColorRed, ColorGreen, ColorBlue)
        {
        }

        public TeleportPoint(EventPoint Copy)
            : base(Copy)
        {
        }

        public TeleportPoint(BinaryReader BR)
            : base(BR)
        {
            OtherMapEntryPoint = new Point(BR.ReadInt32(), BR.ReadInt32());
            OtherMapEntryLayer = BR.ReadInt32();
        }

        public override void Save(BinaryWriter BW)
        {
            base.Save(BW);

            BW.Write(OtherMapEntryPoint.X);
            BW.Write(OtherMapEntryPoint.Y);
            BW.Write(OtherMapEntryLayer);
        }

        #region Properties

        [CategoryAttribute("Map Switch Attributes"),
        DescriptionAttribute(".")]
        public Point OtherMapEntryPoint { get; set; }
        public int OtherMapEntryLayer { get; set; }

        #endregion
    }
}
