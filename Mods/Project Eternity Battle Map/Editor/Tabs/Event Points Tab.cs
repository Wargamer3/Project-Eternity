using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class EventPointsTab : IMapEditorTab
    {
        private TabPage tabEventPoints;
        private PropertyGrid pgEventPoints;
        private GroupBox gbSingleplayerSpawns;
        private CheckBox btnSpawnAlly;
        private CheckBox btnSpawnNeutral;
        private CheckBox btnSpawnEnemy;
        private CheckBox btnSpawnPlayer;
        private GroupBox gbOtherEvents;
        private CheckBox btnEventSpawn;
        private Label lblEventSpawns;
        private GroupBox gbMultiplayerSpawns;
        private Label lblDeathmatch;
        private CheckBox btnSpawnDM;
        private Button btnRemoveDeathmatchTeam;
        private Button btnAddDeathmatchTeam;
        private ComboBox cbDeadthmatch;
        private CheckBox btnMapSwitches;
        private Label lblMapSwitches;
        private CheckBox btnTeleporters;
        private Label lblTeleporters;

        private EventPoint ActiveSpawn;
        
        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            this.tabEventPoints = new TabPage();
            this.pgEventPoints = new PropertyGrid();
            this.gbSingleplayerSpawns = new GroupBox();
            this.btnSpawnAlly = new CheckBox();
            this.btnSpawnNeutral = new CheckBox();
            this.btnSpawnEnemy = new CheckBox();
            this.btnSpawnPlayer = new CheckBox();
            this.gbOtherEvents = new GroupBox();
            this.btnTeleporters = new CheckBox();
            this.lblTeleporters = new Label();
            this.btnMapSwitches = new CheckBox();
            this.lblMapSwitches = new Label();
            this.btnEventSpawn = new CheckBox();
            this.lblEventSpawns = new Label();
            this.gbMultiplayerSpawns = new GroupBox();
            this.btnRemoveDeathmatchTeam = new Button();
            this.btnAddDeathmatchTeam = new Button();
            this.lblDeathmatch = new Label();
            this.btnSpawnDM = new CheckBox();
            this.cbDeadthmatch = new ComboBox();

            tabEventPoints.SuspendLayout();

            // 
            // tabEventPoints
            // 
            this.tabEventPoints.Controls.Add(this.pgEventPoints);
            this.tabEventPoints.Controls.Add(this.gbSingleplayerSpawns);
            this.tabEventPoints.Controls.Add(this.gbOtherEvents);
            this.tabEventPoints.Controls.Add(this.gbMultiplayerSpawns);
            this.tabEventPoints.Location = new System.Drawing.Point(4, 22);
            this.tabEventPoints.Name = "tabEventPoints";
            this.tabEventPoints.Padding = new Padding(3);
            this.tabEventPoints.Size = new System.Drawing.Size(325, 497);
            this.tabEventPoints.TabIndex = 0;
            this.tabEventPoints.Text = "Event points";
            this.tabEventPoints.UseVisualStyleBackColor = true;
            // 
            // pgEventPoints
            // 
            this.pgEventPoints.Location = new System.Drawing.Point(6, 280);
            this.pgEventPoints.Name = "pgEventPoints";
            this.pgEventPoints.PropertySort = PropertySort.Categorized;
            this.pgEventPoints.Size = new System.Drawing.Size(228, 211);
            this.pgEventPoints.TabIndex = 14;
            this.pgEventPoints.ToolbarVisible = false;
            // 
            // gbSingleplayerSpawns
            // 
            this.gbSingleplayerSpawns.Controls.Add(this.btnSpawnAlly);
            this.gbSingleplayerSpawns.Controls.Add(this.btnSpawnNeutral);
            this.gbSingleplayerSpawns.Controls.Add(this.btnSpawnEnemy);
            this.gbSingleplayerSpawns.Controls.Add(this.btnSpawnPlayer);
            this.gbSingleplayerSpawns.Location = new System.Drawing.Point(7, 6);
            this.gbSingleplayerSpawns.Name = "gbSingleplayerSpawns";
            this.gbSingleplayerSpawns.Size = new System.Drawing.Size(226, 86);
            this.gbSingleplayerSpawns.TabIndex = 13;
            this.gbSingleplayerSpawns.TabStop = false;
            this.gbSingleplayerSpawns.Text = "Singleplayer Spawns";
            // 
            // btnSpawnAlly
            // 
            this.btnSpawnAlly.Appearance = Appearance.Button;
            this.btnSpawnAlly.BackColor = System.Drawing.Color.Lime;
            this.btnSpawnAlly.Location = new System.Drawing.Point(165, 19);
            this.btnSpawnAlly.Name = "btnSpawnAlly";
            this.btnSpawnAlly.Size = new System.Drawing.Size(47, 47);
            this.btnSpawnAlly.TabIndex = 15;
            this.btnSpawnAlly.Text = "A";
            this.btnSpawnAlly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSpawnAlly.UseVisualStyleBackColor = false;
            this.btnSpawnAlly.CheckedChanged += new System.EventHandler(this.btnSpawnAlly_CheckedChanged);
            // 
            // btnSpawnNeutral
            // 
            this.btnSpawnNeutral.Appearance = Appearance.Button;
            this.btnSpawnNeutral.BackColor = System.Drawing.Color.Yellow;
            this.btnSpawnNeutral.Location = new System.Drawing.Point(112, 19);
            this.btnSpawnNeutral.Name = "btnSpawnNeutral";
            this.btnSpawnNeutral.Size = new System.Drawing.Size(47, 47);
            this.btnSpawnNeutral.TabIndex = 14;
            this.btnSpawnNeutral.Text = "N";
            this.btnSpawnNeutral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSpawnNeutral.UseVisualStyleBackColor = false;
            this.btnSpawnNeutral.CheckedChanged += new System.EventHandler(this.btnSpawnNeutral_CheckedChanged);
            // 
            // btnSpawnEnemy
            // 
            this.btnSpawnEnemy.Appearance = Appearance.Button;
            this.btnSpawnEnemy.BackColor = System.Drawing.Color.Red;
            this.btnSpawnEnemy.Location = new System.Drawing.Point(59, 19);
            this.btnSpawnEnemy.Name = "btnSpawnEnemy";
            this.btnSpawnEnemy.Size = new System.Drawing.Size(47, 47);
            this.btnSpawnEnemy.TabIndex = 13;
            this.btnSpawnEnemy.Text = "E";
            this.btnSpawnEnemy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSpawnEnemy.UseVisualStyleBackColor = false;
            this.btnSpawnEnemy.CheckedChanged += new System.EventHandler(this.btnSpawnEnemy_CheckedChanged);
            // 
            // btnSpawnPlayer
            // 
            this.btnSpawnPlayer.Appearance = Appearance.Button;
            this.btnSpawnPlayer.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSpawnPlayer.Location = new System.Drawing.Point(6, 19);
            this.btnSpawnPlayer.Name = "btnSpawnPlayer";
            this.btnSpawnPlayer.Size = new System.Drawing.Size(47, 47);
            this.btnSpawnPlayer.TabIndex = 12;
            this.btnSpawnPlayer.Text = "P";
            this.btnSpawnPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSpawnPlayer.UseVisualStyleBackColor = false;
            this.btnSpawnPlayer.CheckedChanged += new System.EventHandler(this.btnSpawnPlayer_CheckedChanged);
            // 
            // gbOtherEvents
            // 
            this.gbOtherEvents.Controls.Add(this.btnTeleporters);
            this.gbOtherEvents.Controls.Add(this.lblTeleporters);
            this.gbOtherEvents.Controls.Add(this.btnMapSwitches);
            this.gbOtherEvents.Controls.Add(this.lblMapSwitches);
            this.gbOtherEvents.Controls.Add(this.btnEventSpawn);
            this.gbOtherEvents.Controls.Add(this.lblEventSpawns);
            this.gbOtherEvents.Location = new System.Drawing.Point(7, 189);
            this.gbOtherEvents.Name = "gbOtherEvents";
            this.gbOtherEvents.Size = new System.Drawing.Size(226, 85);
            this.gbOtherEvents.TabIndex = 12;
            this.gbOtherEvents.TabStop = false;
            this.gbOtherEvents.Text = "Other";
            // 
            // btnTeleporters
            // 
            this.btnTeleporters.Appearance = Appearance.Button;
            this.btnTeleporters.BackColor = System.Drawing.Color.Firebrick;
            this.btnTeleporters.Location = new System.Drawing.Point(168, 32);
            this.btnTeleporters.Name = "btnTeleporters";
            this.btnTeleporters.Size = new System.Drawing.Size(47, 47);
            this.btnTeleporters.TabIndex = 14;
            this.btnTeleporters.Text = "T";
            this.btnTeleporters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnTeleporters.UseVisualStyleBackColor = false;
            this.btnTeleporters.CheckedChanged += new System.EventHandler(this.btnTeleporters_CheckedChanged);
            // 
            // lblTeleporters
            // 
            this.lblTeleporters.AutoSize = true;
            this.lblTeleporters.Location = new System.Drawing.Point(165, 16);
            this.lblTeleporters.Name = "lblTeleporters";
            this.lblTeleporters.Size = new System.Drawing.Size(60, 13);
            this.lblTeleporters.TabIndex = 15;
            this.lblTeleporters.Text = "Teleporters";
            // 
            // btnMapSwitches
            // 
            this.btnMapSwitches.Appearance = Appearance.Button;
            this.btnMapSwitches.BackColor = System.Drawing.Color.Moccasin;
            this.btnMapSwitches.Location = new System.Drawing.Point(89, 32);
            this.btnMapSwitches.Name = "btnMapSwitches";
            this.btnMapSwitches.Size = new System.Drawing.Size(47, 47);
            this.btnMapSwitches.TabIndex = 12;
            this.btnMapSwitches.Text = "S";
            this.btnMapSwitches.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnMapSwitches.UseVisualStyleBackColor = false;
            this.btnMapSwitches.CheckedChanged += new System.EventHandler(this.btnMapSwitches_CheckedChanged);
            // 
            // lblMapSwitches
            // 
            this.lblMapSwitches.AutoSize = true;
            this.lblMapSwitches.Location = new System.Drawing.Point(86, 16);
            this.lblMapSwitches.Name = "lblMapSwitches";
            this.lblMapSwitches.Size = new System.Drawing.Size(74, 13);
            this.lblMapSwitches.TabIndex = 13;
            this.lblMapSwitches.Text = "Map Switches";
            // 
            // btnEventSpawn
            // 
            this.btnEventSpawn.Appearance = Appearance.Button;
            this.btnEventSpawn.BackColor = System.Drawing.Color.DarkViolet;
            this.btnEventSpawn.Location = new System.Drawing.Point(6, 32);
            this.btnEventSpawn.Name = "btnEventSpawn";
            this.btnEventSpawn.Size = new System.Drawing.Size(47, 47);
            this.btnEventSpawn.TabIndex = 11;
            this.btnEventSpawn.Text = "O";
            this.btnEventSpawn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnEventSpawn.UseVisualStyleBackColor = false;
            this.btnEventSpawn.CheckedChanged += new System.EventHandler(this.btnEventSpawn_CheckedChanged);
            // 
            // lblEventSpawns
            // 
            this.lblEventSpawns.AutoSize = true;
            this.lblEventSpawns.Location = new System.Drawing.Point(6, 16);
            this.lblEventSpawns.Name = "lblEventSpawns";
            this.lblEventSpawns.Size = new System.Drawing.Size(74, 13);
            this.lblEventSpawns.TabIndex = 11;
            this.lblEventSpawns.Text = "Event spawns";
            // 
            // gbMultiplayerSpawns
            // 
            this.gbMultiplayerSpawns.Controls.Add(this.btnRemoveDeathmatchTeam);
            this.gbMultiplayerSpawns.Controls.Add(this.btnAddDeathmatchTeam);
            this.gbMultiplayerSpawns.Controls.Add(this.lblDeathmatch);
            this.gbMultiplayerSpawns.Controls.Add(this.btnSpawnDM);
            this.gbMultiplayerSpawns.Controls.Add(this.cbDeadthmatch);
            this.gbMultiplayerSpawns.Location = new System.Drawing.Point(7, 98);
            this.gbMultiplayerSpawns.Name = "gbMultiplayerSpawns";
            this.gbMultiplayerSpawns.Size = new System.Drawing.Size(226, 85);
            this.gbMultiplayerSpawns.TabIndex = 1;
            this.gbMultiplayerSpawns.TabStop = false;
            this.gbMultiplayerSpawns.Text = "Multiplayer Spawns";
            // 
            // btnRemoveDeathmatchTeam
            // 
            this.btnRemoveDeathmatchTeam.Location = new System.Drawing.Point(145, 56);
            this.btnRemoveDeathmatchTeam.Name = "btnRemoveDeathmatchTeam";
            this.btnRemoveDeathmatchTeam.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveDeathmatchTeam.TabIndex = 13;
            this.btnRemoveDeathmatchTeam.Text = "Remove";
            this.btnRemoveDeathmatchTeam.UseVisualStyleBackColor = true;
            this.btnRemoveDeathmatchTeam.Click += new System.EventHandler(this.btnRemoveDeathmatchTeam_Click);
            // 
            // btnAddDeathmatchTeam
            // 
            this.btnAddDeathmatchTeam.Location = new System.Drawing.Point(62, 56);
            this.btnAddDeathmatchTeam.Name = "btnAddDeathmatchTeam";
            this.btnAddDeathmatchTeam.Size = new System.Drawing.Size(75, 23);
            this.btnAddDeathmatchTeam.TabIndex = 12;
            this.btnAddDeathmatchTeam.Text = "Add";
            this.btnAddDeathmatchTeam.UseVisualStyleBackColor = true;
            this.btnAddDeathmatchTeam.Click += new System.EventHandler(this.btnAddDeathmatchTeam_Click);
            // 
            // lblDeathmatch
            // 
            this.lblDeathmatch.AutoSize = true;
            this.lblDeathmatch.Location = new System.Drawing.Point(6, 16);
            this.lblDeathmatch.Name = "lblDeathmatch";
            this.lblDeathmatch.Size = new System.Drawing.Size(65, 13);
            this.lblDeathmatch.TabIndex = 11;
            this.lblDeathmatch.Text = "Deathmatch";
            // 
            // btnSpawnDM
            // 
            this.btnSpawnDM.Appearance = Appearance.Button;
            this.btnSpawnDM.BackColor = System.Drawing.Color.Turquoise;
            this.btnSpawnDM.Location = new System.Drawing.Point(9, 32);
            this.btnSpawnDM.Name = "btnSpawnDM";
            this.btnSpawnDM.Size = new System.Drawing.Size(47, 47);
            this.btnSpawnDM.TabIndex = 11;
            this.btnSpawnDM.Text = "1";
            this.btnSpawnDM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSpawnDM.UseVisualStyleBackColor = false;
            this.btnSpawnDM.CheckedChanged += new EventHandler(this.btnSpawnDM_CheckedChanged);
            this.btnSpawnDM.MouseClick += new MouseEventHandler(this.btnSpawnDM_MouseClick);
            this.btnSpawnDM.MouseDown += new MouseEventHandler(this.btnSpawnDM_MouseDown);
            this.btnSpawnDM.MouseMove += new MouseEventHandler(this.btnSpawnDM_MouseMove);
            // 
            // cbDeadthmatch
            // 
            this.cbDeadthmatch.FormattingEnabled = true;
            this.cbDeadthmatch.Location = new System.Drawing.Point(9, 58);
            this.cbDeadthmatch.Name = "cbDeadthmatch";
            this.cbDeadthmatch.Size = new System.Drawing.Size(47, 21);
            this.cbDeadthmatch.TabIndex = 0;
            this.cbDeadthmatch.SelectedIndexChanged += new EventHandler(this.cbDeadthmatch_SelectedIndexChanged);

            this.tabEventPoints.ResumeLayout(false);

            return tabEventPoints;
        }

        public void OnMapLoaded()
        {
            if (ActiveMap.ListMultiplayerColor.Count > 0)
            {
                for (int C = 0; C < ActiveMap.ListMultiplayerColor.Count; C++)
                {
                    cbDeadthmatch.Items.Add(C + 1);
                }

                btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(ActiveMap.ListMultiplayerColor[0].R, ActiveMap.ListMultiplayerColor[0].G, ActiveMap.ListMultiplayerColor[0].B);
            }
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

            if (e.Button == MouseButtons.Left)
            {
                HandleEventPoint(GridX, GridY, ActiveSpawn);
            }
            else if (e.Button == MouseButtons.Right)
            {
                HandleEventPoint(GridX, GridY, null);
            }
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
            tslInformation.Text += " Left click to place a new spawn point";
            tslInformation.Text += " Right click to remove a spawn point";
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();
        }

        private void HandleEventPoint(int X, int Y, EventPoint Spawn)
        {//If there is an active Spawn and a map loaded.
            if (ActiveMap.TileSize.X != 0)
            {
                if (btnTeleporters.Checked)
                {
                    NewTeleporterPoint(X, Y, Spawn);
                }
                else if (btnMapSwitches.Checked)
                {
                    NewMapSwitchPoint(X, Y, Spawn);
                }
                else if (btnSpawnDM.Checked)
                {
                    NewSpawnMultiplayer(X, Y, Spawn);
                }
                else if (btnSpawnPlayer.Checked || btnSpawnAlly.Checked || btnSpawnEnemy.Checked || btnSpawnNeutral.Checked)
                {
                    NewSpawnSingleplayer(X, Y, Spawn);
                }
            }
        }

        private void NewSpawnSingleplayer(int X, int Y, EventPoint Spawn)
        {
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, BattleMapViewer.SelectedListLayerIndex);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListCampaignSpawns.Count; S++)
            {//If it exist.
                if (TopLayer.ListCampaignSpawns[S].Position.X == X && TopLayer.ListCampaignSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    TopLayer.ListCampaignSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        TopLayer.ListCampaignSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListCampaignSpawns.Add(Spawn);
            }
        }

        private void NewSpawnMultiplayer(int X, int Y, EventPoint Spawn)
        {
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, BattleMapViewer.SelectedListLayerIndex);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListMultiplayerSpawns.Count; S++)
            {//If it exist.
                if (TopLayer.ListMultiplayerSpawns[S].Position.X == X && TopLayer.ListMultiplayerSpawns[S].Position.Y == Y && (Spawn == null || TopLayer.ListMultiplayerSpawns[S].Tag == Spawn.Tag))
                {
                    //Delete it.
                    TopLayer.ListMultiplayerSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        TopLayer.ListMultiplayerSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListMultiplayerSpawns.Add(Spawn);
            }
        }

        private void NewMapSwitchPoint(int X, int Y, EventPoint Spawn)
        {
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
            MapSwitchPoint OldEventPoint = null;

            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListMapSwitchPoint.Count; S++)
            {//If it exist.
                if (TopLayer.ListMapSwitchPoint[S].Position.X == X && TopLayer.ListMapSwitchPoint[S].Position.Y == Y)
                {
                    OldEventPoint = TopLayer.ListMapSwitchPoint[S];
                }
            }

            if (Spawn != null)
            {
                if (OldEventPoint == null)
                {
                    MapSwitchPoint NewMapSwitchPoint = new MapSwitchPoint(Spawn);
                    NewMapSwitchPoint.Position = new Vector3(X, Y, BattleMapViewer.SelectedListLayerIndex);
                    TopLayer.ListMapSwitchPoint.Add(NewMapSwitchPoint);
                    pgEventPoints.SelectedObject = NewMapSwitchPoint;
                }
                else
                {
                    pgEventPoints.SelectedObject = OldEventPoint;
                }
            }
            else if (OldEventPoint != null)
            {
                TopLayer.ListMapSwitchPoint.Remove(OldEventPoint);
            }
        }

        private void NewTeleporterPoint(int X, int Y, EventPoint Spawn)
        {
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
            TeleportPoint OldEventPoint = null;

            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListTeleportPoint.Count; S++)
            {//If it exist.
                if (TopLayer.ListTeleportPoint[S].Position.X == X && TopLayer.ListTeleportPoint[S].Position.Y == Y)
                {
                    OldEventPoint = TopLayer.ListTeleportPoint[S];
                }
            }

            if (Spawn != null)
            {
                if (OldEventPoint == null)
                {
                    TeleportPoint NewMapSwitchPoint = new TeleportPoint(Spawn);
                    NewMapSwitchPoint.Position = new Vector3(X, Y, BattleMapViewer.SelectedListLayerIndex);
                    TopLayer.ListTeleportPoint.Add(NewMapSwitchPoint);
                    pgEventPoints.SelectedObject = NewMapSwitchPoint;
                }
                else
                {
                    pgEventPoints.SelectedObject = OldEventPoint;
                }
            }
            else if (OldEventPoint != null)
            {
                TopLayer.ListTeleportPoint.Remove(OldEventPoint);
            }
        }

        #region Selection spawn changes

        private void ResetSpawn(CheckBox Sender)
        {
            ActiveSpawn = null;
            if (Sender != btnSpawnPlayer)
                btnSpawnPlayer.Checked = false;
            if (Sender != btnSpawnEnemy)
                btnSpawnEnemy.Checked = false;
            if (Sender != btnSpawnDM)
                btnSpawnDM.Checked = false;
            if (Sender != btnEventSpawn)
                btnEventSpawn.Checked = false;
            if (Sender != btnMapSwitches)
                btnMapSwitches.Checked = false;
            if (Sender != btnTeleporters)
                btnTeleporters.Checked = false;
        }

        private void btnSpawnPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnPlayer.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnPlayer);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnPlayer.Text, btnSpawnPlayer.BackColor.R, btnSpawnPlayer.BackColor.G, btnSpawnPlayer.BackColor.B);
            }
        }

        private void btnSpawnEnemy_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnEnemy.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnEnemy);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnEnemy.Text, btnSpawnEnemy.BackColor.R, btnSpawnEnemy.BackColor.G, btnSpawnEnemy.BackColor.B);
            }
        }

        private void btnSpawnNeutral_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnNeutral.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnNeutral);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnNeutral.Text, btnSpawnNeutral.BackColor.R, btnSpawnNeutral.BackColor.G, btnSpawnNeutral.BackColor.B);
            }
        }

        private void btnSpawnAlly_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnAlly.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnAlly);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnAlly.Text, btnSpawnAlly.BackColor.R, btnSpawnAlly.BackColor.G, btnSpawnAlly.BackColor.B);
            }
        }

        private void btnSpawnDM_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CheckBoxSender = (CheckBox)sender;
            //Reset the Spawn buttons
            ResetSpawn((CheckBox)sender);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, CheckBoxSender.Text, CheckBoxSender.BackColor.R, CheckBoxSender.BackColor.G, CheckBoxSender.BackColor.B);
        }

        #endregion

        #region Multiplayer

        private void btnSpawnDM_MouseMove(object sender, MouseEventArgs e)
        {//If left clicked and moving, open the team selector.
            if (e.Button == MouseButtons.Left)
                cbDeadthmatch.DroppedDown = true;
        }

        private void btnSpawnDM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {//If left click and over the little black cursor at the bottom right, open the team selector.
                if (e.X > btnSpawnDM.Width - 10 && e.Y > btnSpawnDM.Height - 10)
                    cbDeadthmatch.DroppedDown = true;
            }
        }

        private void btnSpawnDM_MouseDown(object sender, MouseEventArgs e)
        {//If right clicked, open a new color Dialog.
            if (e.Button == MouseButtons.Right)
            {
                ColorDialog CD = new ColorDialog();
                if (CD.ShowDialog() == DialogResult.OK)
                {//Change the button color and the color in the list at the same time with the returned color.
                    btnSpawnDM.BackColor = CD.Color;
                    int MPColorIndex = Math.Max(0, cbDeadthmatch.SelectedIndex);
                    ActiveMap.ListMultiplayerColor[MPColorIndex] = Color.FromNonPremultiplied(CD.Color.R, CD.Color.G, CD.Color.B, 255);
                    if (btnSpawnDM.Checked)
                    {
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.R;
                        ActiveSpawn.ColorGreen = btnSpawnDM.BackColor.G;
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.B;
                    }

                    foreach (BaseMapLayer ActiveLayer in Helper.GetLayersAndSubLayers())
                    {
                        for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                        {
                            if (ActiveLayer.ListMultiplayerSpawns[S].Tag == btnSpawnDM.Text)
                            {
                                ActiveLayer.ListMultiplayerSpawns[S].ColorRed = CD.Color.R;
                                ActiveLayer.ListMultiplayerSpawns[S].ColorGreen = CD.Color.G;
                                ActiveLayer.ListMultiplayerSpawns[S].ColorBlue = CD.Color.B;
                            }
                        }
                    }
                }
            }
        }

        //A new team is selected.
        private void cbDeadthmatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSpawnDM.Text = cbDeadthmatch.Text;//Give the button the selected text.
            btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].R,
                                                                 ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].G,
                                                                 ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].B);//Give the button the selected color.
            btnSpawnDM.Checked = true;//Press the button.
            //Update the ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnDM.Text, btnSpawnDM.BackColor.R, btnSpawnDM.BackColor.G, btnSpawnDM.BackColor.B);
        }

        private void btnAddDeathmatchTeam_Click(object sender, EventArgs e)
        {
            Color[] ArrayColorChoices = new Color[] { Color.Turquoise, Color.White, Color.SteelBlue, Color.Silver, Color.SandyBrown, Color.Salmon, Color.Purple, Color.PaleGreen, Color.Orange, Color.Gold, Color.ForestGreen, Color.Firebrick, Color.Chartreuse, Color.Beige, Color.DeepPink, Color.DarkMagenta };
            ActiveMap.ListMultiplayerColor.Add(ArrayColorChoices[Math.Min(ArrayColorChoices.Length - 1, ActiveMap.ListMultiplayerColor.Count)]);
            cbDeadthmatch.Items.Add(ActiveMap.ListMultiplayerColor.Count);
        }

        private void btnRemoveDeathmatchTeam_Click(object sender, EventArgs e)
        {
            if (cbDeadthmatch.SelectedIndex >= 0)
            {
                ActiveMap.ListMultiplayerColor.RemoveAt(cbDeadthmatch.SelectedIndex);
                cbDeadthmatch.Items.RemoveAt(cbDeadthmatch.SelectedIndex);
            }
        }

        #endregion

        private void btnEventSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEventSpawn.Checked)
            {
                ResetSpawn((CheckBox)sender);
                ActiveSpawn = new EventPoint(Vector3.Zero, "O", Color.DarkViolet.R, Color.DarkViolet.G, Color.DarkViolet.B);
            }
        }

        private void btnMapSwitches_CheckedChanged(object sender, EventArgs e)
        {
            //Reset the Spawn buttons
            ResetSpawn(btnMapSwitches);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnMapSwitches.Text, btnMapSwitches.BackColor.R, btnMapSwitches.BackColor.G, btnMapSwitches.BackColor.B);
        }

        private void btnTeleporters_CheckedChanged(object sender, EventArgs e)
        {
            //Reset the Spawn buttons
            ResetSpawn(btnTeleporters);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnTeleporters.Text, btnTeleporters.BackColor.R, btnTeleporters.BackColor.G, btnTeleporters.BackColor.B);
        }
    }
}
