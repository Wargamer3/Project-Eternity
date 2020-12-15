using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptStartAnimation : BattleMapScript
        {
            private AnimatedSprite AnimationSprite;

            private string _AnimationPath;
            private float _AnimationSpeed;
            private Microsoft.Xna.Framework.Vector2 _AnimationPosition;

            public ScriptStartAnimation()
                : this(null)
            {
                _AnimationPath = "";
            }

            public ScriptStartAnimation(BattleMap Map)
                : base(Map, 100, 50, "Start Animation", new string[] { "Play" }, new string[] { "Animation Ended" })
            {
                _AnimationPath = "";
                _AnimationSpeed = 15f;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsDrawn = true;

                AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath, new Microsoft.Xna.Framework.Vector2(_AnimationPosition.X * Map.TileSize.X, _AnimationPosition.Y * Map.TileSize.Y), _AnimationSpeed);
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
    }
}
