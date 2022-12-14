using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using FMOD;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptPlaySFX : CutsceneActionScript
        {
            private string _SFXPath;
            private FMODSound ActiveSound;
            private bool IsInit;

            public ScriptPlaySFX()
                : base(100, 50, "Play SFX", new string[] { "Play" }, new string[] { "SFX Ended" })
            {
                _SFXPath = "";
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

                    ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + _SFXPath + ".mp3");
                    ActiveSound.Play();
                }
                else
                {
                    if (ActiveSound != null)
                    {
                        if (!ActiveSound.IsPlaying())
                        {
                            SoundSystem.ReleaseSound(ActiveSound);
                            ActiveSound = null;
                            ExecuteEvent(this, 0);
                            IsEnded = true;
                        }
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                SFXPath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(SFXPath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptPlaySFX();
            }

            #region Properties

            [Editor(typeof(Selectors.SFXSelector), typeof(UITypeEditor)),
            CategoryAttribute("SFX behavior"),
            DescriptionAttribute("The SFX path"),
            DefaultValueAttribute(0)]
            public string SFXPath
            {
                get
                {
                    return _SFXPath;
                }
                set
                {
                    _SFXPath = value;
                }
            }

            #endregion
        }
    }
}
