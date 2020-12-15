using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens;

namespace ProjectEternity
{
    public class FightSprite
    {
        public int SpriteIndex;
        public Vector2 Position;
        public Vector2 PositionNext;
        public Rectangle Origin;
        public Vector2 RotationPoint;
        public Vector2 Speed;
        public float Angle;
        public List<FightKeyframe> ListKeyframe;
        protected float AnimationSpeed;
        private float AnimationIndex;
        public float Lifetime;
        public FightSprite(int SpriteIndex, Vector2 Position, float Lifetime, float Angle, Rectangle Origin, float AnimationSpeed, List<FightKeyframe> ListKeyframe)
        {
            this.SpriteIndex = SpriteIndex;
            this.Position = Position;
            this.PositionNext = Vector2.Zero;
            this.Lifetime = Lifetime;
            this.Speed = Vector2.Zero;
            this.ListKeyframe = ListKeyframe;
            this.Angle = Angle;
            this.Origin = Origin;
            this.AnimationSpeed = AnimationSpeed;
            this.AnimationIndex = 0;
            this.RotationPoint = new Vector2(Origin.Width / 2, Origin.Height / 2);
        }
        protected FightSprite() { }//Used to freely create child class.
        public virtual void Update()
        {
            if (AnimationSpeed != 0)
            {
                AnimationIndex += AnimationSpeed;
                Origin.X = ((int)AnimationIndex) * Origin.Width;
            }
            if (PositionNext != Vector2.Zero)
            {
                if (Position.X != PositionNext.X)
                {
                    Position.X += Speed.X;
                    if (Position.X >= PositionNext.X - Speed.X && Position.X <= PositionNext.X + Speed.X)
                        Position.X = PositionNext.X;
                }
                if (Position.Y != PositionNext.Y)
                {
                    Position.Y += Speed.Y;
                    if (Position.Y >= PositionNext.Y - Speed.Y && Position.Y <= PositionNext.Y + Speed.Y)
                        Position.Y = PositionNext.Y;
                }
            }
        }
        public virtual void OnDestroy() { }
    };

    public sealed class FightMissile : FightSprite
    {
        float ParticuleAngle;
        public FightMissile(int SpriteIndex, Vector2 Position, List<FightKeyframe> ListKeyframe, float Angle = 0)
        {
            this.SpriteIndex = SpriteIndex;
            this.Position = Position;
            this.Lifetime = 4.8f;
            this.Angle = Angle;
            this.ParticuleAngle = MathHelper.ToDegrees(Angle);
            this.AnimationSpeed = 0;
            this.Origin = new Rectangle(0, 0, 54, 30);
            this.ListKeyframe = ListKeyframe;
            this.RotationPoint = new Vector2(Origin.Width / 2, Origin.Height / 2);
        }
        public override void Update()
        {
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), 5, MathHelper.ToRadians(140 - ParticuleAngle + FightScreen.Random.Next(40) * 2)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), 5, MathHelper.ToRadians(140 - ParticuleAngle + FightScreen.Random.Next(40) * 2)));
            FightScreen.ListFightSprite.Add(new FightSmoke(2, new Vector2(Position.X, Position.Y - 8)));
            base.Update();
        }
        public override void OnDestroy()
        {
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 2) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 3) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 4) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 5) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 6) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 7) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 8) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 9) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 10) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 2) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 3) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 4) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 5) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 6) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 7) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 8) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 9) - 5 + FightScreen.Random.Next(10)));
            FightScreen.ListFightSprite.Add(new FightParticule(1, new Vector2(Position.X, Position.Y + 8), FightScreen.Random.Next(5) + 2, MathHelper.ToRadians(36 * 10) - 5 + FightScreen.Random.Next(10)));
        }
    }

    public sealed class FightParticule : FightSprite
    {
        public FightParticule(int SpriteIndex, Vector2 Position, float Speed, float Angle)
        {
            this.SpriteIndex = SpriteIndex;
            this.Position = Position;
            this.PositionNext = Vector2.Zero;
            this.Lifetime = 0.5f;
            this.Angle = 0;
            this.AnimationSpeed = 0;
            this.Origin = new Rectangle(0, 0, 18, 18);
            this.Speed = new Vector2((float)Math.Cos(Angle) * Speed, (float)Math.Sin(Angle) * Speed);
            ListKeyframe = new List<FightKeyframe>();
            this.RotationPoint = new Vector2(Origin.Width / 2, Origin.Height / 2);
        }
        public override void Update()
        {
            Position += Speed;
        }
    }

    public sealed class FightSmoke : FightSprite
    {
        public FightSmoke(int SpriteIndex, Vector2 Position)
        {
            this.SpriteIndex = SpriteIndex;
            this.Position = Position;
            this.PositionNext = Vector2.Zero;
            this.Lifetime = 1f;
            this.Angle = 0;
            this.AnimationSpeed = 0.3f;
            this.Origin = new Rectangle(0, 0, 64, 64);
            ListKeyframe = new List<FightKeyframe>();
            this.RotationPoint = new Vector2(Origin.Width / 2, Origin.Height / 2);
        }
        public override void Update()
        {
            Position.Y -= 1f;
            base.Update();
        }
    }

    public struct FightKeyframe
    {
        public float TimelinePosition;
        public Vector2 Speed;
        public float RelativeSpeed;
        public Vector2 Position;
        public FightKeyframe(float TimelinePosition, Vector2 Position, Vector2 Speed)
        {
            this.TimelinePosition = TimelinePosition;
            this.Position = Position;
            this.Speed = Speed;
            this.RelativeSpeed = 0;
        }
        public FightKeyframe(float TimelinePosition, Vector2 Position, float RelativeSpeed)
        {
            this.TimelinePosition = TimelinePosition;
            this.Position = Position;
            this.Speed = Vector2.Zero;
            this.RelativeSpeed = RelativeSpeed;
        }
    };

    public struct FightTimelineKeyframe
    {
        public float TimelinePosition;
        public Vector2 CameraPos;
        public Vector2 CameraSpeed;
        public int TextIndex;
        public FightSprite FightSprite;
        public FightTimelineKeyframe(float TimelinePosition, Vector2 CameraPos, Vector2 CameraSpeed, int TextIndex, FightSprite Sprite)
        {
            this.TimelinePosition = TimelinePosition;
            this.CameraPos = CameraPos;
            this.CameraSpeed = CameraSpeed;
            this.TextIndex = TextIndex;
            this.FightSprite = Sprite;
        }
    }

    public class FightScreen : GameScreen
    {
        Texture2D sprRectangle;
        float TimelinePosition;
        const float TimelineSpeed = 1 / 50.0f;//Speed at which the timeline move.
        float TimelineLength;//Maximum lifetime of the screen.
        public static List<FightSprite> ListFightSprite;//List of FightSprite to be used.
        List<Texture2D> ListTexture;//List of sprites to be used.
        List<FightTimelineKeyframe> ListTimelineKeyframe;//Key momments of the timeline where actions are started.
        Vector2 CameraPos;
        Vector2 CameraPosNext;
        Vector2 CameraSpeed;
        int TextIndex;//Index of the text to draw.
        string[] TextLines;//Text to draw during the fight.
        public static Random Random = new Random();
        public SpriteFont fntArial;

        public FightScreen(float TimelineLength)
            : base()
        {
            FightScreen.ListFightSprite = new List<FightSprite>();
            ListTexture = new List<Texture2D>();
            ListTimelineKeyframe = new List<FightTimelineKeyframe>();
            CameraPos = Vector2.Zero;
            CameraSpeed = Vector2.Zero;
            this.TimelineLength = TimelineLength;
        }
        public override void Load()
        {
            fntArial = Content.Load<SpriteFont>("Fonts/Arial");
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            ListTexture.Add(Content.Load<Texture2D>("Battle/Missile"));
            ListTexture.Add(Content.Load<Texture2D>("Battle/Particule"));
            ListTexture.Add(Content.Load<Texture2D>("Battle/BombeFume"));
            ListTexture.Add(Content.Load<Texture2D>("Visual Novel/Takeru"));
            ListTexture.Add(Content.Load<Texture2D>("Visual Novel/Boss"));
            #region Create left player.
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(0, new Vector2(1000, 0), new Vector2(4.2f, 0), 0, new FightSprite(4, new Vector2(0, 100), -1, 0, new Rectangle(0, 0, ListTexture[4].Width, ListTexture[4].Height), 0, new List<FightKeyframe>())));
            List<FightKeyframe> ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(0, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(0.6f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.2f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(1.8f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(2.4f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.0f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(3.6f, new Vector2(1400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(4.2f, new Vector2(1600, 100), 6));
            ListFightSprite.Add(new FightMissile(0, new Vector2(0, Random.Next(20) * 10), ListKeyframe));
            #endregion
            #region Create right player.
            ListFightSprite.Add(new FightSprite(3, new Vector2(1400, 100), -1, 0, new Rectangle(0, 0, ListTexture[3].Width, ListTexture[3].Height), 0, new List<FightKeyframe>()));
            //Create the conter.
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListKeyframe = new List<FightKeyframe>();
            ListKeyframe.Add(new FightKeyframe(6.7f, new Vector2(1200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.3f, new Vector2(1000, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(7.9f, new Vector2(800, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(8.5f, new Vector2(600, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.1f, new Vector2(400, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(9.7f, new Vector2(200, FightScreen.Random.Next(20) * 10), 6));
            ListKeyframe.Add(new FightKeyframe(10.3f, new Vector2(0, FightScreen.Random.Next(20) * 10), 6));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 0, new FightMissile(0, new Vector2(1400, Random.Next(20) * 10), ListKeyframe, MathHelper.ToRadians(180))));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, new Vector2(-90, 0), new Vector2(-4.2f, 0), 0, null));
            #endregion
            //Set the dialog.
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(5, Vector2.Zero, Vector2.Zero, 1, null));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6, Vector2.Zero, Vector2.Zero, 2, null));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(6.7f, Vector2.Zero, Vector2.Zero, 3, null));
            ListTimelineKeyframe.Add(new FightTimelineKeyframe(5, Vector2.Zero, Vector2.Zero, 1, null));
            TextIndex = 0;
            TextLines = new string[] { "TAKERU!!!!!", "Ahhh, that hurts", "My turn now", "DIEEE" };
        }
        public override void Update(GameTime gameTime)
        {
            //Update the timeline.
            for (int i = 0; i < ListTimelineKeyframe.Count; i++)
            {//If the timeline hit a Key Frame.
                if (TimelinePosition >= ListTimelineKeyframe[i].TimelinePosition - TimelineSpeed && TimelinePosition <= ListTimelineKeyframe[i].TimelinePosition + TimelineSpeed)
                {//Move the camera.
                    //If the next camera position isn't null.
                    if (ListTimelineKeyframe[i].CameraPos != Vector2.Zero)
                    {//Set the speed and next position.
                        CameraSpeed = ListTimelineKeyframe[i].CameraSpeed;
                        CameraPosNext = ListTimelineKeyframe[i].CameraPos;
                    }
                    //If the next TextIndex isn't null.
                    if (ListTimelineKeyframe[i].TextIndex != 0)
                    {//Set the text index.
                        TextIndex = ListTimelineKeyframe[i].TextIndex;
                    }
                    //If the next FightSprite isn't null.
                    if (ListTimelineKeyframe[i].FightSprite != null)
                    {//Add it to the instances list.
                        ListFightSprite.Add(ListTimelineKeyframe[i].FightSprite);
                    }
                    //Remove the Key Frame so it won't be used again.
                    ListTimelineKeyframe.RemoveAt(i--);
                }
            }
            //Update FightSprite list.
            for (int i = 0; i < ListFightSprite.Count; i++)
            {//Check for Keyframes.
                for (int F = 0; F < ListFightSprite[i].ListKeyframe.Count; F++)
                {//If the timeline hit a Key Frame.
                    if (TimelinePosition >= ListFightSprite[i].ListKeyframe[F].TimelinePosition - TimelineSpeed && TimelinePosition <= ListFightSprite[i].ListKeyframe[F].TimelinePosition + TimelineSpeed)
                    {//Update the next position.
                        ListFightSprite[i].PositionNext = ListFightSprite[i].ListKeyframe[F].Position;
                        //If the next speed is null.
                        if (ListFightSprite[i].ListKeyframe[F].Speed == Vector2.Zero)
                        {//Calculate it by using the relative speed parameter and the difference between the current position and the next position.
                            Vector2 Variation = new Vector2(ListFightSprite[i].PositionNext.X - ListFightSprite[i].Position.X, ListFightSprite[i].PositionNext.Y - ListFightSprite[i].Position.Y);
                            //If horizontal movement.
                            if (Variation.Y == 0)
                                ListFightSprite[i].Speed = new Vector2(Math.Sign(Variation.X) * ListFightSprite[i].ListKeyframe[F].RelativeSpeed, 0);
                            //If vertical movement.
                            else if (Variation.X == 0)
                                ListFightSprite[i].Speed = new Vector2(0, Math.Sign(Variation.Y) * ListFightSprite[i].ListKeyframe[F].RelativeSpeed);
                            //Normal movement.
                            else
                            {//Normalize the variation so the relative speed can be used.
                                Variation.Normalize();
                                ListFightSprite[i].Speed = Variation * ListFightSprite[i].ListKeyframe[F].RelativeSpeed;
                            }
                        }
                        else//Else, if the speed is not null, asign it.
                            ListFightSprite[i].Speed = ListFightSprite[i].ListKeyframe[F].Speed;
                        //Remove the Key Frame so it won't be used again.
                        ListFightSprite[i].ListKeyframe.RemoveAt(F--);
                    }
                }
                //Update the sprite.
                ListFightSprite[i].Update();
                if (ListFightSprite[i].Lifetime != -1)
                {
                    ListFightSprite[i].Lifetime -= TimelineSpeed;
                    //Destroy the sprite.
                    if (ListFightSprite[i].Lifetime <= 0)
                    {
                        ListFightSprite[i].OnDestroy();
                        ListFightSprite.RemoveAt(i--);
                    }
                }
            }
            //If the camera didn'T reach its target position.
            if (CameraPos != CameraPosNext)
            {
                CameraPos += CameraSpeed;
                //Check if the position is inside a specific range.
                if (CameraPos.X >= CameraPosNext.X - Math.Abs(CameraSpeed.X) && CameraPos.X <= CameraPosNext.X + Math.Abs(CameraSpeed.X))
                {
                    CameraPos = CameraPosNext;
                }
            }
            //Update the timeline position.
            TimelinePosition += TimelineSpeed;
            //Remove the screen if the animation has ended.
            if (TimelinePosition >= TimelineLength)
                RemoveScreen(this);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            //Move everything relative to the camera position.
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(new Vector3(-CameraPos.X, -CameraPos.Y, 0)));
            //Draw the FightSprite
            for (int i = 0; i < ListFightSprite.Count; i++)
            {
                g.Draw(ListTexture[ListFightSprite[i].SpriteIndex], ListFightSprite[i].Position, ListFightSprite[i].Origin, Color.White, ListFightSprite[i].Angle, ListFightSprite[i].RotationPoint, 1f, SpriteEffects.None, 0);
            }
            //Replace the drawing surface to a normal position.
            g.End();
            g.Begin();
            //Draw the text.
            g.Draw(sprRectangle, new Rectangle(0, Constants.Height - 150, Constants.Width, 150), Color.DarkBlue);
            g.DrawString(fntArial, TextLines[TextIndex], new Vector2(10, Constants.Height - 150), Color.White);
        }
    }
}
