using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptReadFromTextFile : CutsceneActionScript
        {
            private string _TextPath;

            public ScriptReadFromTextFile()
                : base(140, 70, "Read From File", new string[] { "Execute" }, new string[] { "Script Executed" })
            {
            }
            public void CastToMyType<T>(T hackToInferNeededType, object givenObject) where T : class
            {
                var newObject = givenObject as T;
            }

            public override void ExecuteTrigger(int Index)
            {
                Dictionary<string, CutsceneScript> DicScripts = LoadAllScripts();
                string[] Content = new string[] { "" };
                foreach (string Line in Content)
                {
                    string[] LineContent = Line.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string ScriptName = LineContent[0];
                    CutsceneScript LineScript = DicScripts[ScriptName];
                    System.Reflection.PropertyInfo[] ScriptProperties = LineScript.GetType().GetProperties();
                    for (int i = 0; i < ScriptProperties.Length; ++i)
                    {
                        ScriptProperties[i].SetValue(LineScript, Convert.ChangeType(LineContent[i + 1], ScriptProperties[i].PropertyType));
                    }
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TextPath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TextPath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptReadFromTextFile();
            }

            #region Properties

            [CategoryAttribute("Script behavior"),
            DescriptionAttribute("Path to script text."),
            DefaultValueAttribute("")]
            public string TextPath
            {
                get
                {
                    return _TextPath;
                }
                set
                {
                    _TextPath = value;
                }
            }

            #endregion
        }
    }
}
