using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.AIEditor
{
    public  partial class AIEditor : BaseEditor
    {
        public AIContainer ActiveAI { get { return aiViewer.AI; } set { aiViewer.AI = value; } }

        private CheckBox cbShowExecutionOrder;

        public AIEditor()
        {
            InitializeComponent();

            #region cbDrawScripts

            //Init the DisplayUnselectedLayers button (as it can't be done with the tool box)
            cbShowExecutionOrder = new CheckBox();
            cbShowExecutionOrder.Text = "Show Execution Order";
            cbShowExecutionOrder.AutoSize = false;
            //Link a CheckedChanged event to a method.
            cbShowExecutionOrder.Checked = true;
            cbShowExecutionOrder.CheckedChanged += new EventHandler(tsmShowExecutionOrder_Click);
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowExecutionOrder.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost s = new ToolStripControlHost(cbShowExecutionOrder);
            s.AutoSize = false;
            s.Width = 180;
            mnuToolBar.Items.Add(s);

            #endregion

            foreach (KeyValuePair<string, List<AIScript>> ActiveScripts in AIScriptHolder.GetAllScriptsByCategory())
            {
                ListBox NewScriptListBox = new ListBox();
                aiViewer.tsmDeleteScript.Click += tsmDeleteScript_Click;

                NewScriptListBox.Dock = DockStyle.Fill;
                NewScriptListBox.FormattingEnabled = true;
                NewScriptListBox.Location = new Point(0, 0);
                NewScriptListBox.Name = "lst" + ActiveScripts.Key;
                NewScriptListBox.Size = new Size(248, 243);
                NewScriptListBox.TabIndex = 4;
                NewScriptListBox.DoubleClick += new EventHandler(lstChoices_DoubleClick);
                NewScriptListBox.Items.AddRange(ActiveScripts.Value.ToArray());

                TabPage NewScriptTab = new TabPage();
                NewScriptTab.Controls.Add(NewScriptListBox);
                NewScriptTab.Location = new Point(4, 22);
                NewScriptTab.Name = "tab" + ActiveScripts.Key;
                NewScriptTab.Size = new Size(248, 243);
                NewScriptTab.TabIndex = 4;
                NewScriptTab.Text = ActiveScripts.Key;
                NewScriptTab.UseVisualStyleBackColor = true;

                tabControl1.Controls.Add(NewScriptTab);
            }
        }

        public AIEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                //Create the Part file.
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Empty Script list.

                FS.Close();
                BW.Close();
            }

            LoadAI(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAIs }, "AIs/", new string[] { ".peai" }, typeof(AIEditor))
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            ActiveAI.Save(ItemPath);
        }

        private void LoadAI(string AIPath)
        {
            string Name = AIPath.Substring(0, AIPath.Length - 5).Substring(12);
            this.Text = Name + " - Project Eternity AI Editor";

            ActiveAI = new AIContainer();
            aiViewer.AIPath = Name;
        }

        private void tsmShowExecutionOrder_Click(object sender, EventArgs e)
        {
            aiViewer.ShowExecutionOrder = cbShowExecutionOrder.Checked;
            aiViewer.DrawScripts();
        }

        #region Scripting

        private void SelectScript(AIScript SelectedScript)
        {
            pgScriptProperties.SelectedObject = SelectedScript;
        }

        private void lstChoices_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is ListBox))
                return;
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            AIScript NewScript = ((AIScript)((ListBox)sender).SelectedItem).CopyScript();
            aiViewer.InitScript(NewScript);
            ActiveAI.ListScript.Add(NewScript);
            aiViewer.DrawScripts();
        }

        private void tsmDeleteScript_Click(object sender, EventArgs e)
        {
            aiViewer.DeleteScript();
        }

        #endregion

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void AIEditor_Shown(object sender, EventArgs e)
        {
            if (AIScriptHolder.DicAIScripts.Count == 0)
            {
                AIScriptHolder.DicAIScripts = AIScriptHolder.LoadAllScripts();
            }
            aiViewer.Init(SelectScript);
        }
    }
}
