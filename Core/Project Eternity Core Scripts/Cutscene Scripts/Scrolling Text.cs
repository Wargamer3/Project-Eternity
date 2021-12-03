using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.Core.TextHelper;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptScrollingText : CutsceneActionScript
        {
            private string _Text;
            private float _TextSpeed;
            private string _SFXPath;
            private string _BackgroundPath;
            private TextAligns _TextAlign;
            private SpriteFont TextFont;
            private FMODSound ActiveSound;
            private Texture2D Background;
            private bool IsInit;
            private List<string> ListText;
            private float Progression;
            private float MaxProgression;
            int TextMaxWidthInPixel = 400;

            public ScriptScrollingText()
                : base(140, 70, "Scrolling Text", new string[] { "Start text" }, new string[] { "Text ended" })
            {
                _Text = "";
                _TextSpeed = 1;
                _SFXPath = "";
                _BackgroundPath = "";
                TextFont = null;
                Progression = 100;
                IsInit = false;
                ListText = new List<string>();
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsDrawn = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsInit)
                {
                    IsInit = true;

                    if (!string.IsNullOrEmpty(_SFXPath))
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/Maps/SFX/" + _SFXPath + ".mp3");
                        ActiveSound.Play();
                    }
                }
                else
                {
                    Progression -= _TextSpeed;
                    if (Progression < -530 - MaxProgression || InputHelper.InputConfirmPressed())
                    {
                        ExecuteEvent(this, 0);
                        IsEnded = true;
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                g.End();
                g.Begin(SpriteSortMode.Immediate, null);

                float XPos = 320;
                float YPos = 400 + Progression;

                TextHelper.DrawTextMultiline(g, ListText, _TextAlign, XPos, YPos, TextMaxWidthInPixel);

                g.End();
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            }

            public override void Load(BinaryReader BR)
            {
                Text = BR.ReadString().Replace('’', '\'');
                TextSpeed = BR.ReadSingle();
                TextAlign = (TextAligns)BR.ReadByte();
                SFXPath = BR.ReadString();
                BackgroundPath = BR.ReadString();

                if (Owner == null)
                {
                    return;
                }

                TextFont = Owner.Content.Load<SpriteFont>("Fonts/Arial12");

                string[] ArrayWorkingString = Text.Split('\n', '\r');

                for (int i = 0; i < ArrayWorkingString.Length; ++i)
                {
                    string WorkingString = ArrayWorkingString[i];
                    int CurrentChar = Math.Min(TextMaxWidthInPixel / 10, WorkingString.Length);

                    while (WorkingString.Length > 0)
                    {
                        string TestString = WorkingString.Substring(0, CurrentChar);

                        if (TextFont.MeasureString(TestString).X > TextMaxWidthInPixel)
                        {
                            --CurrentChar;
                            TestString = WorkingString.Substring(0, CurrentChar);
                            if (TextFont.MeasureString(TestString).X <= TextMaxWidthInPixel)
                            {
                                int LastSpace = TestString.LastIndexOf(' ') + 1;
                                if (LastSpace > 0)
                                {
                                    CurrentChar = LastSpace;
                                }

                                TestString = WorkingString.Substring(0, CurrentChar);
                                ListText.Add(TestString);
                                MaxProgression += TextFont.LineSpacing;
                                WorkingString = WorkingString.Remove(0, CurrentChar);
                                CurrentChar = 0;
                            }
                        }
                        else if (TextFont.MeasureString(TestString).X <= TextMaxWidthInPixel)
                        {
                            ++CurrentChar;
                            if (CurrentChar >= WorkingString.Length)
                            {
                                TestString = WorkingString.Substring(0, CurrentChar);
                                ListText.Add(TestString);
                                MaxProgression += TextFont.LineSpacing;
                                WorkingString = WorkingString.Remove(0, CurrentChar);
                                CurrentChar = 0;
                            }
                            else
                            {
                                TestString = WorkingString.Substring(0, CurrentChar);
                                if (TextFont.MeasureString(TestString).X > TextMaxWidthInPixel)
                                {
                                    int LastSpace = TestString.LastIndexOf(' ') + 1;
                                    if (LastSpace > 0)
                                    {
                                        CurrentChar = LastSpace;
                                    }

                                    TestString = WorkingString.Substring(0, CurrentChar);
                                    ListText.Add(TestString);
                                    MaxProgression += TextFont.LineSpacing;
                                    WorkingString = WorkingString.Remove(0, CurrentChar);
                                    CurrentChar = 0;
                                }
                            }
                        }
                    }
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(Text);
                BW.Write(TextSpeed);
                BW.Write((byte)_TextAlign);
                BW.Write(SFXPath);
                BW.Write(BackgroundPath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptScrollingText();
            }

            #region Properties

            [CategoryAttribute("Scrolling Text behavior"),
            Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
            public string Text
            {
                get
                {
                    return _Text;
                }
                set
                {
                    _Text = value;
                }
            }

            [CategoryAttribute("Scrolling Text behavior"),
            DescriptionAttribute("How many pixels the text will go down per image."),
            DefaultValueAttribute(1)]
            public float TextSpeed
            {
                get
                {
                    return _TextSpeed;
                }
                set
                {
                    _TextSpeed = value;
                }
            }

            [CategoryAttribute("Scrolling Text behavior"),
            DescriptionAttribute("Text Alignment")]
            public TextAligns TextAlign
            {
                get
                {
                    return _TextAlign;
                }
                set
                {
                    _TextAlign = value;
                }
            }

            [Editor(typeof(Selectors.SFXSelector), typeof(UITypeEditor)),
            CategoryAttribute("Scrolling Text behavior"),
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

            [Editor(typeof(Selectors.BackgroundSelector), typeof(UITypeEditor)),
            CategoryAttribute("Scrolling Text behavior"),
            DescriptionAttribute("The Background path"),
            DefaultValueAttribute(0)]
            public string BackgroundPath
            {
                get
                {
                    return _BackgroundPath;
                }
                set
                {
                    _BackgroundPath = value;
                }
            }

            #endregion
        }
    }
}
