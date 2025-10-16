using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.Core.Editor
{
    public partial class TextScriptEditor : Form
    {
        Dictionary<string, CutsceneScript> DicScripts;

        public TextScriptEditor(string Text)
        {
            InitializeComponent();

            DicScripts = CutsceneScriptHolder.LoadAllScripts();
            txtTextScript.Text = Text;
        }

        private void txtTextScript_TextChanged(object sender, EventArgs e)
        {
            txtTextScriptHelper.Text = string.Join("\r\n", DicScripts.Keys);

            if (txtTextScript.Lines.Length == 0)
                return;

            int CurrentLineIndex = txtTextScript.GetLineFromCharIndex(txtTextScript.SelectionStart);
            string CurrentLine = txtTextScript.Lines[CurrentLineIndex];

            string LineContent = CurrentLine;

            int IndexOfScriptName = CurrentLine.IndexOf(",");
            if (IndexOfScriptName >= 0)
                LineContent = CurrentLine.Substring(0, IndexOfScriptName);

            if (DicScripts.ContainsKey(LineContent))
            {
                CutsceneScript ActiveScript = DicScripts[LineContent];

                List<string> Output = new List<string>();
                Output.Add(ActiveScript.Name);

                System.Reflection.PropertyInfo[] ScriptProperties = ActiveScript.GetType().GetProperties();
                for (int i = 0; i < ScriptProperties.Length; ++i)
                {
                    if (ScriptProperties[i].CanWrite)
                    {
                        if (ScriptProperties[i].PropertyType.IsArray)
                        {
                            object[] ArrayValue = (object[])ScriptProperties[i].GetValue(ActiveScript);
                            Output.Add(ScriptProperties[i].Name + " (Val1,Val2,Val3,...)");
                        }
                        else
                        {
                            Output.Add(ScriptProperties[i].Name);
                        }
                    }
                }
                txtTextScriptHelper.Text = string.Join(", ", Output);
            }
        }
    }
}
