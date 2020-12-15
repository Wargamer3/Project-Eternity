using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptStartAnimationRelative : DeathmatchMapScript
        {
            private AnimatedSprite AnimationSprite;

            private string _AnimationPath;
            private float _AnimationSpeed;

            private uint _RelativeSpawnUnitID;
            private Point _RelativeSpawnPosition;

            public ScriptStartAnimationRelative()
                : this(null)
            {
                _AnimationPath = "";
                _AnimationSpeed = 0.5f;

                _RelativeSpawnUnitID = 0;
            }

            public ScriptStartAnimationRelative(DeathmatchMap Map)
                : base(Map, 100, 50, "Start Animation Relative", new string[] { "Play" }, new string[] { "Animation Ended" })
            {
                _AnimationPath = "";
                _AnimationSpeed = 0.5f;

                _RelativeSpawnUnitID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsDrawn = true;

                AnimationSprite = null;

                for (int P = 0; P < Map.ListPlayer.Count && AnimationSprite == null; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && AnimationSprite == null; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID != _RelativeSpawnUnitID)
                            continue;

                        AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath,
                            new Microsoft.Xna.Framework.Vector2((Map.ListPlayer[P].ListSquad[U].X + _RelativeSpawnPosition.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                                (Map.ListPlayer[P].ListSquad[U].Y + _RelativeSpawnPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y),
                            _AnimationSpeed);
                        break;
                    }
                }
                if (AnimationSprite == null)
                {
                    IsEnded = true;
                    ExecuteEvent(this, 0);
                }
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
                RelativeSpawnUnitID = BR.ReadUInt32();
                RelativeSpawnPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(AnimationPath);
                BW.Write(AnimationSpeed);
                BW.Write(RelativeSpawnUnitID);
                BW.Write(RelativeSpawnPosition.X);
                BW.Write(RelativeSpawnPosition.Y);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptStartAnimationRelative(Map);
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

            [CategoryAttribute("Animation Attributes"),
            DescriptionAttribute(".")]
            public UInt32 RelativeSpawnUnitID
            {
                get
                {
                    return _RelativeSpawnUnitID;
                }
                set
                {
                    _RelativeSpawnUnitID = value;
                }
            }

            [CategoryAttribute("Animation Attributes"),
            DescriptionAttribute(".")]
            public Point RelativeSpawnPosition
            {
                get
                {
                    return _RelativeSpawnPosition;
                }
                set
                {
                    _RelativeSpawnPosition = value;
                }
            }

            #endregion
        }
    }
}
