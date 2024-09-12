using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class ScriptsTab : IMapEditorTab
    {
        private TabPage tabScripting;
        private SplitContainer ScriptingContainer;
        private TabControl tabScirptingList;
        private PropertyGrid pgScriptProperties;
        private TabPage tbEvents;
        private ListBox lstEvents;
        private TabPage tbTriggers;
        private ListBox lstTriggers;
        private TabPage tbConditions;
        private ListBox lstConditions;
        private ToolStripMenuItem tsmDeleteScript;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            this.tabScripting = new TabPage();
            this.tbConditions = new TabPage();
            this.lstConditions = new ListBox();
            this.ScriptingContainer = new SplitContainer();
            this.tabScirptingList = new TabControl();
            this.tbEvents = new TabPage();
            this.lstEvents = new ListBox();
            this.tbTriggers = new TabPage();
            this.lstTriggers = new ListBox();
            this.pgScriptProperties = new PropertyGrid();

            tabScripting.SuspendLayout();

            // 
            // tbConditions
            // 
            this.tbConditions.Controls.Add(this.lstConditions);
            this.tbConditions.Location = new System.Drawing.Point(4, 22);
            this.tbConditions.Name = "tbConditions";
            this.tbConditions.Size = new System.Drawing.Size(307, 211);
            this.tbConditions.TabIndex = 0;
            this.tbConditions.Text = "Conditions";
            this.tbConditions.UseVisualStyleBackColor = true;
            // 
            // lstConditions
            // 
            this.lstConditions.FormattingEnabled = true;
            this.lstConditions.Location = new System.Drawing.Point(0, 0);
            this.lstConditions.Name = "lstConditions";
            this.lstConditions.Size = new System.Drawing.Size(222, 199);
            this.lstConditions.TabIndex = 0;
            this.lstConditions.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);
            // 
            // tabScripting
            // 
            this.tabScripting.Controls.Add(this.ScriptingContainer);
            this.tabScripting.Location = new System.Drawing.Point(4, 22);
            this.tabScripting.Name = "tabScripting";
            this.tabScripting.Padding = new Padding(3);
            this.tabScripting.Size = new System.Drawing.Size(325, 497);
            this.tabScripting.TabIndex = 3;
            this.tabScripting.Text = "Scripting";
            this.tabScripting.UseVisualStyleBackColor = true;
            // 
            // ScriptingContainer
            // 
            this.ScriptingContainer.BorderStyle = BorderStyle.Fixed3D;
            this.ScriptingContainer.Dock = DockStyle.Fill;
            this.ScriptingContainer.Location = new System.Drawing.Point(3, 3);
            this.ScriptingContainer.Name = "ScriptingContainer";
            this.ScriptingContainer.Orientation = Orientation.Horizontal;
            // 
            // ScriptingContainer.Panel1
            // 
            this.ScriptingContainer.Panel1.Controls.Add(this.tabScirptingList);
            // 
            // ScriptingContainer.Panel2
            // 
            this.ScriptingContainer.Panel2.Controls.Add(this.pgScriptProperties);
            this.ScriptingContainer.Size = new System.Drawing.Size(319, 491);
            this.ScriptingContainer.SplitterDistance = 241;
            this.ScriptingContainer.TabIndex = 7;
            // 
            // tabScirptingList
            // 
            this.tabScirptingList.Controls.Add(this.tbEvents);
            this.tabScirptingList.Controls.Add(this.tbConditions);
            this.tabScirptingList.Controls.Add(this.tbTriggers);
            this.tabScirptingList.Dock = DockStyle.Fill;
            this.tabScirptingList.Location = new System.Drawing.Point(0, 0);
            this.tabScirptingList.Multiline = true;
            this.tabScirptingList.Name = "tabScirptingList";
            this.tabScirptingList.SelectedIndex = 0;
            this.tabScirptingList.Size = new System.Drawing.Size(315, 237);
            this.tabScirptingList.SizeMode = TabSizeMode.FillToRight;
            this.tabScirptingList.TabIndex = 0;
            this.tabScirptingList.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);

            // 
            // pgScriptProperties
            // 
            this.pgScriptProperties.Dock = DockStyle.Fill;
            this.pgScriptProperties.Location = new System.Drawing.Point(0, 0);
            this.pgScriptProperties.Name = "pgScriptProperties";
            this.pgScriptProperties.PropertySort = PropertySort.Categorized;
            this.pgScriptProperties.Size = new System.Drawing.Size(315, 242);
            this.pgScriptProperties.TabIndex = 0;
            this.pgScriptProperties.ToolbarVisible = false;
            // 
            // tbEvents
            // 
            this.tbEvents.Controls.Add(this.lstEvents);
            this.tbEvents.Location = new System.Drawing.Point(4, 22);
            this.tbEvents.Name = "tbEvents";
            this.tbEvents.Size = new System.Drawing.Size(307, 211);
            this.tbEvents.TabIndex = 2;
            this.tbEvents.Text = "Events";
            this.tbEvents.UseVisualStyleBackColor = true;
            // 
            // lstEvents
            // 
            this.lstEvents.Dock = DockStyle.Fill;
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.Location = new System.Drawing.Point(0, 0);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(307, 211);
            this.lstEvents.TabIndex = 0;
            this.lstEvents.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);
            // 
            // tbTriggers
            // 
            this.tbTriggers.Controls.Add(this.lstTriggers);
            this.tbTriggers.Location = new System.Drawing.Point(4, 22);
            this.tbTriggers.Name = "tbTriggers";
            this.tbTriggers.Size = new System.Drawing.Size(307, 211);
            this.tbTriggers.TabIndex = 1;
            this.tbTriggers.Text = "Triggers";
            this.tbTriggers.UseVisualStyleBackColor = true;
            // 
            // lstTriggers
            // 
            this.lstTriggers.Dock = DockStyle.Fill;
            this.lstTriggers.FormattingEnabled = true;
            this.lstTriggers.Location = new System.Drawing.Point(0, 0);
            this.lstTriggers.Name = "lstTriggers";
            this.lstTriggers.Size = new System.Drawing.Size(307, 211);
            this.lstTriggers.TabIndex = 0;
            this.lstTriggers.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);

            tabScripting.ResumeLayout();

            return tabScripting;
        }

        public void OnMapLoaded()
        {
            BattleMapViewer.SetListMapScript(ActiveMap.ListMapScript);
            BattleMapViewer.ScriptHelper.OnSelect = (SelectedObject, RightClick) =>
            {
                if (RightClick && SelectedObject != null)
                {
                    BattleMapViewer.cmsScriptMenu.Show(BattleMapViewer, BattleMapViewer.PointToClient(Cursor.Position));
                }
                else
                {
                    pgScriptProperties.SelectedObject = SelectedObject;
                }
            };

            for (int S = ActiveMap.ListMapScript.Count - 1; S >= 0; --S)
            {
                BattleMapViewer.ScriptHelper.InitScript(ActiveMap.ListMapScript[S]);
            }

            lstEvents.Items.AddRange(ActiveMap.DicMapEvent.Values.ToArray());
            lstConditions.Items.AddRange(ActiveMap.DicMapCondition.Values.ToArray());
            lstTriggers.Items.AddRange(ActiveMap.DicMapTrigger.Values.ToArray());

            this.tsmDeleteScript = new ToolStripMenuItem();
            // 
            // cmsScriptMenu
            // 
            BattleMapViewer.cmsScriptMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmDeleteScript});

            // 
            // tsmDeleteScript
            //
            this.tsmDeleteScript.Name = "tsmDeleteScript";
            this.tsmDeleteScript.Size = new System.Drawing.Size(140, 22);
            this.tsmDeleteScript.Text = "Delete Script";
            this.tsmDeleteScript.Click += tsmDeleteScript_Click;
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        public void OnMouseMove(MouseEventArgs e, int MouseX, int MouseY)
        {
            int MaxX, MaxY;
            BattleMapViewer.ScriptHelper.MoveScript(e.Location, out MaxX, out MaxY);

            BattleMapViewer.UpdateDimensions(MaxX, MaxY);
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
            BattleMapViewer.ScriptHelper.Select(e.Location);
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
            BattleMapViewer.ScriptHelper.Scripting_MouseUp(e.Location, (e.Button & MouseButtons.Left) == MouseButtons.Left, (e.Button & MouseButtons.Right) == MouseButtons.Right);
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.ScriptHelper.DrawScripts();
        }

        private void tsmDeleteScript_Click(object sender, EventArgs e)
        {
            BattleMapViewer.ScriptHelper.DeleteSelectedScript();
        }

        private void lstScriptChoices_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is ListBox))
                return;
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            BattleMapViewer.ScriptHelper.CreateScript((MapScript)((ListBox)sender).SelectedItem);
        }
    }
}
