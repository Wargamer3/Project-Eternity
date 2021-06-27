using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptReadFromText : CutsceneActionScript
        {
            private string _Text;
            private List<CutsceneActionScript> ListTextScriptToTrigger;

            public ScriptReadFromText()
                : base(140, 70, "Read From Text", new string[] { "Execute" }, new string[] { "Script Executed" })
            {
                ListTextScriptToTrigger = new List<CutsceneActionScript>();
            }

            public override void ExecuteTrigger(int Index)
            {
                foreach (CutsceneActionScript ActiveScript in ListTextScriptToTrigger)
                {
                    for (int T = 0; T < ActiveScript.NameTriggers.Length; ++T)
                    {
                        ActiveScript.ExecuteTrigger(T);
                    }
                }

                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                bool IsActive = false;

                foreach (CutsceneActionScript ActiveScript in ListTextScriptToTrigger)
                {
                    if (Owner.CheckIfScriptIsActive(ActiveScript))
                    {
                        IsActive = true;
                        break;
                    }
                }

                if (!IsActive)
                {
                    IsEnded = true;

                    ExecuteEvent(this, 0);
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void AfterCutsceneLoad()
            {
                Dictionary<string, CutsceneScript> DicScripts;
                if (Owner == null)
                {
                    DicScripts = LoadAllScripts();
                }
                else
                {
                    DicScripts = Owner.DicCutsceneScript;
                }
                string[] Content = _Text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string PreviousEvent = null;
                CutsceneActionScript LastActionScript = null;
                foreach (string Line in Content)
                {
                    string[] LineContent = Line.Split(new string[1] { ", " }, StringSplitOptions.None);
                    string ScriptName = LineContent[0];
                    CutsceneScript LineScript = DicScripts[ScriptName].CopyScript(Owner);
                    System.Reflection.PropertyInfo[] ScriptProperties = LineScript.GetType().GetProperties();

                    int i = 1;
                    foreach (System.Reflection.PropertyInfo Property in ScriptProperties)
                    {
                        if (Property.CanWrite)
                        {
                            string Value = "";
                            if (i < LineContent.Length)
                                Value = LineContent[i];

                            string[] ArrayLineContent = Value.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (Property.PropertyType­.IsArray)
                            {
                                Property.SetValue(LineScript, Convert.ChangeType(ArrayLineContent, Property.PropertyType, CultureInfo.InvariantCulture));
                            }
                            else if (Property.PropertyType­.IsEnum)
                            {
                                object EnumValue = Enum.Parse(Property.PropertyType, Value);
                                Property.SetValue(LineScript, Convert.ChangeType(EnumValue, Property.PropertyType, CultureInfo.InvariantCulture));
                            }
                            else if (Value.StartsWith("{"))//Structs like Vector2
                            {
                                object PropertReference = Property.GetValue(LineScript);
                                string[] ArrayLineContentField = Value.Split(new string[] { "{", ":", " ", "}" }, StringSplitOptions.RemoveEmptyEntries);

                                for (int j = 0; j < ArrayLineContentField.Length; j += 2)
                                {
                                    System.Reflection.FieldInfo FieldToSet = PropertReference.GetType().GetField(ArrayLineContentField[j]);
                                    FieldToSet.SetValue(PropertReference, Convert.ChangeType(ArrayLineContentField[j + 1], FieldToSet.FieldType, CultureInfo.InvariantCulture));
                                }

                                Property.SetValue(LineScript, PropertReference);
                            }
                            else
                            {
                                Property.SetValue(LineScript, Convert.ChangeType(Value, Property.PropertyType, CultureInfo.InvariantCulture));
                            }

                            ++i;
                        }
                    }

                    CutsceneActionScript LineActionScript = LineScript as CutsceneActionScript;

                    if (LineActionScript != null)
                    {
                        int CurrentScriptCount = 0;
                        if (Owner != null)
                        {
                            LineActionScript.ExecuteEvent = Owner.ExecuteEvent;
                            LineActionScript.GetDataContainerByID = Owner.GetDataContainerByID;
                            CurrentScriptCount = Owner.DicActionScript.Count;
                            Owner.AddActionScript(LineActionScript);
                        }

                        if (PreviousEvent == null)
                        {
                            ListTextScriptToTrigger.Add(LineActionScript);
                        }
                        else
                        {
                            int EventIndex = Array.IndexOf(LastActionScript.NameEvents, PreviousEvent);
                            LastActionScript.ArrayEvents[EventIndex].Add(new EventInfo(CurrentScriptCount, 0));
                        }

                        PreviousEvent = null;
                        if (LineContent.Length > i)
                        {
                            PreviousEvent = LineContent[i];
                            LastActionScript = LineActionScript;
                        }
                        else
                        {
                            LastActionScript = null;
                        }
                    }
                    else
                    {
                        CutsceneDataContainer LineDataContainer = LineScript as CutsceneDataContainer;
                        if (LineDataContainer != null && Owner != null)
                        {
                            Owner.ListDataContainer.Add(LineDataContainer);
                        }
                    }
                }
            }

            public override void Load(BinaryReader BR)
            {
                _Text = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_Text);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptReadFromText();
            }

            #region Properties


            [Editor(typeof(Selectors.TextScriptSelector), typeof(UITypeEditor)),
            CategoryAttribute("Script behavior"),
            DescriptionAttribute("Text to use."),
            DefaultValueAttribute("")]
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

            #endregion
        }
    }
}
