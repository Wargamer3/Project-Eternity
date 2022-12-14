using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public unsafe class ExplosionCutscene : Cutscene
    {
        public class ScriptStartAnimation : CutsceneActionScript
        {
            private AnimatedSprite AnimationSprite;

            private string _AnimationPath;
            private float _AnimationSpeed;
            private Microsoft.Xna.Framework.Vector2 _AnimationPosition;
            private BattleMap Map;

            public ScriptStartAnimation(BattleMap Map)
                : base(100, 50, "Start Animation", new string[] { "Play" }, new string[] { "Animation Ended" })
            {
                _AnimationPath = "";
                _AnimationSpeed = 15f;
                this.Map = Map;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsDrawn = true;

                AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath,
                    new Microsoft.Xna.Framework.Vector2(
                        _AnimationPosition.X * Map.TileSize.X + Map.TileSize.X * 0.5f,
                        _AnimationPosition.Y * Map.TileSize.Y + Map.TileSize.Y * 0.5f), _AnimationSpeed);
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (AnimationSprite != null)
                {
                    AnimationSprite.Update(gameTime);
                    if (AnimationSprite.AnimationEnded)
                    {
                        IsEnded = true;
                        ExecuteEvent(this, 0);
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                if (AnimationSprite != null)
                    AnimationSprite.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                AnimationPath = BR.ReadString();
                AnimationSpeed = BR.ReadSingle();
                AnimationPosition = new Microsoft.Xna.Framework.Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(AnimationPath);
                BW.Write(AnimationSpeed);
                BW.Write(AnimationPosition.X);
                BW.Write(AnimationPosition.Y);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptStartAnimation(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.BitmapAnimationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Animation behavior"),
            DescriptionAttribute("The Animation path."),
            DefaultValueAttribute(0)]
            public string AnimationPath
            {
                get
                {
                    return _AnimationPath;
                }
                set
                {
                    _AnimationPath = value;
                }
            }

            [CategoryAttribute("Animation behavior"),
            DescriptionAttribute("The Animation speed.")]
            public float AnimationSpeed
            {
                get
                {
                    return _AnimationSpeed;
                }
                set
                {
                    _AnimationSpeed = value;
                }
            }

            [CategoryAttribute("Animation behavior"),
            DescriptionAttribute("The Animation position.")]
            public Microsoft.Xna.Framework.Vector2 AnimationPosition
            {
                get
                {
                    return _AnimationPosition;
                }
                set
                {
                    _AnimationPosition = value;
                }
            }

            #endregion
        }

        public ExplosionCutscene(OnCutsceneEndedDelegate OnCutsceneEnded, BattleMap Map, Squad SquadToDestroy)
            : base(OnCutsceneEnded, new System.Collections.Generic.Dictionary<string, CutsceneScript>())
        {
            RequireFocus = false;
            RequireDrawFocus = false;
            CutscenePath = "";

            ScriptCutsceneBehavior CutsceneBehavior = new ScriptCutsceneBehavior();
            ScriptStartAnimation StartAnimation = new ScriptStartAnimation(Map);
            ScriptingScriptHolder.ScriptPlaySFX PlaySFX = new ScriptingScriptHolder.ScriptPlaySFX();

            PlaySFX.ExecuteEvent = ExecuteEvent;
            PlaySFX.SFXPath = "Explosion";
            StartAnimation.ExecuteEvent = ExecuteEvent;
            StartAnimation.AnimationPosition = new Vector2(SquadToDestroy.Position.X - Map.CameraPosition.X, SquadToDestroy.Position.Y - Map.CameraPosition.Y);
            StartAnimation.AnimationPath = "Explosion_strip12";
            StartAnimation.AnimationSpeed = 15f;

            CutsceneBehavior.ArrayEvents[0].Add(new EventInfo(0, 0));
            CutsceneBehavior.ArrayEvents[0].Add(new EventInfo(1, 0));
            CutsceneBehavior.ExecuteEvent = ExecuteEvent;

            ListCutsceneBehavior.Add(CutsceneBehavior);
            AddActionScript(StartAnimation);
            AddActionScript(PlaySFX);
        }

        public override void Load()
        {
        }
    }

    public unsafe class CenterOnSquadCutscene : Cutscene
    {
        public class ScriptCenterCamera : CutsceneActionScript
        {
            private Vector3 _CursorPosition;
            private BattleMap Map;

            public ScriptCenterCamera(BattleMap Map)
                : base(100, 50, "Center Camera", new string[] { "Center" }, new string[] { "Camera Centered" })
            {
                this.Map = Map;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                bool IsFinished = true;
                if (Map.CursorPosition.X < _CursorPosition.X)
                {
                    Map.CursorPosition.X++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X > _CursorPosition.X)
                {
                    Map.CursorPosition.X--;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y < _CursorPosition.Y)
                {
                    Map.CursorPosition.Y++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y > _CursorPosition.Y)
                {
                    Map.CursorPosition.Y--;
                    IsFinished = false;
                }

                //Update the camera if needed.
                if (Map.CursorPosition.X - Map.CameraPosition.X - 3 < 0 && Map.CameraPosition.X >  -3)
                {
                    --Map.CameraPosition.X;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X - Map.CameraPosition.X >= Map.ScreenSize.X / 2 && Map.CameraPosition.X + Map.ScreenSize.X < Map.MapSize.X + 3)
                {
                    ++Map.CameraPosition.X;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y - Map.CameraPosition.Y - 3 < 0 && Map.CameraPosition.Y > -3)
                {
                    --Map.CameraPosition.Y;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y - Map.CameraPosition.Y >= Map.ScreenSize.Y / 2 && Map.CameraPosition.Y + Map.ScreenSize.Y < Map.MapSize.Y + 3)
                {
                    ++Map.CameraPosition.Y;
                    IsFinished = false;
                }

                if (IsFinished)
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                CursorPosition = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(CursorPosition.X);
                BW.Write(CursorPosition.Y);
                BW.Write(CursorPosition.Z);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCenterCamera(Map);
            }

            #region Properties

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Position to center cursor on."),
            DefaultValueAttribute(0)]
            public Vector3 CursorPosition
            {
                get
                {
                    return _CursorPosition;
                }
                set
                {
                    _CursorPosition = value;
                }
            }

            #endregion
        }

        public CenterOnSquadCutscene(OnCutsceneEndedDelegate OnCutsceneEnded, BattleMap Map, Vector3 Position)
            : base(OnCutsceneEnded, new System.Collections.Generic.Dictionary<string, CutsceneScript>())
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            CutscenePath = "";

            ScriptCutsceneBehavior CutsceneBehavior = new ScriptCutsceneBehavior();
            ScriptCenterCamera CenterCamera = new ScriptCenterCamera(Map);

            CenterCamera.ExecuteEvent = ExecuteEvent;
            CenterCamera.CursorPosition = Position;

            CutsceneBehavior.ArrayEvents[0].Add(new EventInfo(0, 0));
            CutsceneBehavior.ExecuteEvent = ExecuteEvent;

            ListCutsceneBehavior.Add(CutsceneBehavior);
            AddActionScript(CenterCamera);
        }

        public override void Load()
        {
        }
    }
}
