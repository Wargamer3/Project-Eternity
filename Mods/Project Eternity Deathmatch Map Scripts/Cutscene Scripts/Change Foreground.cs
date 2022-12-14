using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.Scripts.Selectors;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptChangeForeground : DeathmatchMapScript
        {
            private List<string> _ListForegroundsPath;
            private List<AnimationBackground> ListForegrounds;

            public ScriptChangeForeground()
                : this(null)
            {
                IsEnded = false;
            }

            public ScriptChangeForeground(DeathmatchMap Map)
                : base(Map, 100, 50, "Change Foreground", new string[] { "Change" }, new string[] { "Change Completed" })
            {
                IsEnded = false;

                ListForegrounds = new List<AnimationBackground>();
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.LayerManager.ListLayer[0].LayerGrid.ReplaceForegrounds(ListForegrounds);
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                int ListForegroundsPathCount = BR.ReadInt32();
                for (int F = 0; F < ListForegroundsPathCount; F++)
                {
                    _ListForegroundsPath.Add(BR.ReadString());
                }

                if (Map != null)
                {
                    for (int F = 0; F < _ListForegroundsPath.Count; F++)
                    {
                        ListForegrounds.Add(AnimationBackground.LoadAnimationBackground(_ListForegroundsPath[F], Map.Content, GameScreen.GraphicsDevice));
                    }
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_ListForegroundsPath.Count);
                for (int F = 0; F < _ListForegroundsPath.Count; F++)
                {
                    BW.Write(_ListForegroundsPath[F]);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptChangeForeground(Map);
            }

            #region Properties


            [Editor(typeof(MapBackgroundSelector), typeof(UITypeEditor)),
            CategoryAttribute("Tileset"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public List<string> ListForegroundsPath
            {
                get
                {
                    return _ListForegroundsPath;
                }
                set
                {
                    _ListForegroundsPath = value;
                }
            }
            
            #endregion
        }
    }
}
