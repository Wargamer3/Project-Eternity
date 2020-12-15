using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scene;

namespace ProjectEternity.Editors.SceneEditor
{
    public  partial class SceneEditor : BaseEditor
    {
        private SceneScreen LoadedScene;
        private int CurrentFrame;
        public SceneProperties PropertiesDialog;

        public SceneEditor()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                PropertiesDialog = new SceneProperties();

                ScenePreviewViewer.Owner = this;
                SceneTimelineViewer.Owner = this;
            }
        }

        public SceneEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(10);//10 Scene Events
                BW.Write(0);//Empty Scene Event 1
                BW.Write(0);//Empty Scene Event 2
                BW.Write(0);//Empty Scene Event 3
                BW.Write(0);//Empty Scene Event 4
                BW.Write(0);//Empty Scene Event 5
                BW.Write(0);//Empty Scene Event 6
                BW.Write(0);//Empty Scene Event 7
                BW.Write(0);//Empty Scene Event 8
                BW.Write(0);//Empty Scene Event 9
                BW.Write(0);//Empty Scene Event 10

                FS.Close();
                BW.Close();
            }

            LoadScene(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathScenes }, "Scenes/", new string[] { ".pes" }, typeof(SceneEditor))
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);
            
            LoadedScene.MaxSceneEvent = (int)PropertiesDialog.txtMaxSceneEvent.Value;
            LoadedScene.Save();

            FS.Close();
            BW.Close();
        }

        private void LoadScene(string ScenePath)
        {
            string Name = ScenePath.Substring(0, ScenePath.Length - 4).Substring(15);
            this.Text = Name + " - Project Eternity Scene Editor";

            LoadedScene = new SceneScreen(Name);
            LoadedScene.Load();

            PropertiesDialog.txtMaxSceneEvent.Value = LoadedScene.MaxSceneEvent;

            ScenePreviewViewer.Preload(LoadedScene);
            SceneTimelineViewer.Preload(LoadedScene);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmProperties_Click(object sender, EventArgs e)
        {
            decimal OldValue = PropertiesDialog.txtMaxSceneEvent.Value;

            PropertiesDialog.ShowDialog();
            if (PropertiesDialog.ShowDialog() == DialogResult.OK && PropertiesDialog.txtMaxSceneEvent.Value != OldValue)
            {
                LoadedScene.MaxSceneEvent = (int)PropertiesDialog.txtMaxSceneEvent.Value;
                if (PropertiesDialog.txtMaxSceneEvent.Value < OldValue)
                {
                    for (int i = LoadedScene.MaxSceneEvent; i < OldValue; ++i)
                    {
                        LoadedScene.DicSceneEventByFrame[i].Clear();
                        LoadedScene.DicSceneEventByFrame.Remove(i);
                    }
                }
                else if (PropertiesDialog.txtMaxSceneEvent.Value > OldValue)
                {
                    for (int i = (int)OldValue; i < LoadedScene.MaxSceneEvent; ++i)
                    {
                        LoadedScene.DicSceneEventByFrame.Add(i, new List<SceneEvent>());
                    }
                }
            }
        }

        private void tsmNewItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem Sender = (ToolStripMenuItem)sender;
            SceneEvent SelectedTimeline = (SceneEvent)Sender.Tag;

            LoadedScene.DicSceneEventByFrame[CurrentFrame].Add(SelectedTimeline);
        }

        private void SceneEditor_Shown(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, SceneEvent> Event in SceneScreen.LoadAllSceneEvents())
            {
                ToolStripMenuItem tsmNewItem = new ToolStripMenuItem(Event.Value.SceneEventType);
                tsmNewItem.Tag = Event.Value;
                tsmNewItem.Click += tsmNewItem_Click;
                cmsSceneEvents.Items.Add(tsmNewItem);
            }
        }

        private void tcSceneEvents_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > 0)
            {
                e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            }

            e.Graphics.DrawString(tcSceneEvents.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 2, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
        }

        private void tcSceneEvents_MouseClick(object sender, MouseEventArgs e)
        {//Looping through the controls.
            for (int i = 0; i < tcSceneEvents.TabPages.Count; i++)
            {
                Rectangle r = tcSceneEvents.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    tcSceneEvents.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        private void tcSceneEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcSceneEvents.SelectedIndex == 0)
            {
                ScenePreviewViewer.ActiveEvent = null;
            }
            else
            {
                ScenePreviewViewer.ActiveEvent = (SceneEvent)tcSceneEvents.SelectedTab.Tag;
            }
        }
    }
}
