using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using FMOD;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptChangeMapTheme : BattleMapScript
        {
            private string _ThemePath;

            private FMODSound NewTheme;

            public ScriptChangeMapTheme()
                : this(null)
            {
            }

            public ScriptChangeMapTheme(BattleMap Map)
                : base(Map, 100, 50, "Change Map Theme", new string[] { "Change Theme" }, new string[] { "Theme Changed" })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                NewTheme = new FMODSound(GameScreen.FMODSystem, "Content/Maps/BGM/" + _ThemePath);
                NewTheme.SetLoop(true);
                SoundSystem.ReleaseSound(Map.sndBattleTheme);
                Map.sndBattleTheme = NewTheme;
                Map.sndBattleThemePath = _ThemePath;

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                ThemePath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(ThemePath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptChangeMapTheme(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.BGMSelector), typeof(UITypeEditor)),
            CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The theme path"),
            DefaultValueAttribute(0)]
            public string ThemePath
            {
                get
                {
                    return _ThemePath;
                }
                set
                {
                    _ThemePath = value;
                }
            }

            #endregion
        }
    }
}
