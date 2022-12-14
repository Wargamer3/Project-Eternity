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
        public class ScriptPlayMapTheme : BattleMapScript
        {
            private string _BGMPath;
            private FMODSound ActiveSound;

            private bool IsInit;

            public ScriptPlayMapTheme()
                : this(null)
            {
                _BGMPath = "";
                IsInit = false;
            }

            public ScriptPlayMapTheme(BattleMap Map)
                : base(Map, 100, 50, "Play theme", new string[] { "Play" }, new string[] { "Theme Started" })
            {
                _BGMPath = "";
                IsInit = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsInit)
                {
                    IsInit = true;

                    ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/Maps/BGM/" + _BGMPath + ".mp3");
                    ActiveSound.SetLoop(true);
                    ActiveSound.PlayAsBGM();
                    GameScreen.FMODSystem.sndActiveBGMName = _BGMPath;
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
                BGMPath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(BGMPath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptPlayMapTheme(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.BGMSelector), typeof(UITypeEditor)),
            CategoryAttribute("BGM behavior"),
            DescriptionAttribute("The BGM path"),
            DefaultValueAttribute(0)]
            public string BGMPath
            {
                get
                {
                    return _BGMPath;
                }
                set
                {
                    _BGMPath = value;
                }
            }

            #endregion
        }
    }
}
