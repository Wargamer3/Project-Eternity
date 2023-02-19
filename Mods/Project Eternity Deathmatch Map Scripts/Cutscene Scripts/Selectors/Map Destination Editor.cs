using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class MapDestinationEditor : Form
    {
        DeathmatchParams Params;

        private readonly List<Vector2> ListTerrainChangeLocation;
        public DeathmatchMap ActiveMap { get { return (DeathmatchMap)BattleMapViewer.ActiveMap; } }

        public MapDestinationEditor(List<Vector2> ListTerrainChangeLocation)
        {
            InitializeComponent();

            Params = new DeathmatchParams(new BattleContext());

            this.ListTerrainChangeLocation = ListTerrainChangeLocation;
        }

        private void btnPreviewMap_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathDeathmatchMaps, "Select a Map", false));
        }

        private void BattleMapViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (BattleMapViewer.ActiveMap != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int FinalX = e.X / BattleMapViewer.ActiveMap.TileSize.X;
                    int FinalY = e.Y / BattleMapViewer.ActiveMap.TileSize.X;

                    if (FinalX < 0 || FinalX >= BattleMapViewer.ActiveMap.MapSize.X
                        || FinalY < 0 || FinalY >= BattleMapViewer.ActiveMap.MapSize.Y)
                    {
                        return;
                    }

                    for (int S = 0; S < ActiveMap.LayerManager[0].ListSingleplayerSpawns.Count; S++)
                    {
                        if (ActiveMap.LayerManager[0].ListSingleplayerSpawns[S].Position.X == FinalX && ActiveMap.LayerManager[0].ListSingleplayerSpawns[S].Position.Y == FinalY)
                        {
                            return;
                        }
                    }

                    ActiveMap.LayerManager[0].ListSingleplayerSpawns.Add(new BattleMapScreen.EventPoint(new Vector3(FinalX, FinalY, 0), ActiveMap.LayerManager[0].ListSingleplayerSpawns.Count.ToString(), 255, 255, 255));
                }
                else if (e.Button == MouseButtons.Right)
                {
                    int FinalX = e.X / BattleMapViewer.ActiveMap.TileSize.X;
                    int FinalY = e.Y / BattleMapViewer.ActiveMap.TileSize.X;

                    if (FinalX < 0 || FinalX >= BattleMapViewer.ActiveMap.MapSize.X
                        || FinalY < 0 || FinalY >= BattleMapViewer.ActiveMap.MapSize.Y)
                    {
                        return;
                    }

                    for (int S = 0; S < ActiveMap.LayerManager[0].ListSingleplayerSpawns.Count; S++)
                    {
                        if (ActiveMap.LayerManager[0].ListSingleplayerSpawns[S].Position.X == FinalX && ActiveMap.LayerManager[0].ListSingleplayerSpawns[S].Position.Y == FinalY)
                        {
                            ActiveMap.LayerManager[0].ListSingleplayerSpawns.RemoveAt(S);
                            return;
                        }
                    }
                }
            }
        }

        private void BattleMapViewer_MouseMove(object sender, MouseEventArgs e)
        {
            BattleMapViewer_MouseDown(sender, e);
        }

        private void BattleMapViewer_MouseUp(object sender, MouseEventArgs e)
        {

        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string MapLogicName = Items[0].Substring(0, Items[0].Length - 4).Substring(24);
            BattleMapViewer.Preload();
            DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, new GameModeInfo(), Params);
            NewMap.IsEditor = true;
            NewMap.ShowGrid = true;
            BattleMapViewer.ActiveMap = NewMap;
            NewMap.ListGameScreen = new List<GameScreen>();
            NewMap.Content = BattleMapViewer.content;
            NewMap.Load();
            NewMap.TogglePreview(false);
            NewMap.CursorPositionVisible = new Vector3(-1, -1, 0);

            BattleMapViewer.SetListMapScript(NewMap.ListMapScript);
            BattleMapViewer.Helper.OnSelect = (SelectedObject, RightClick) =>
            {
                if (RightClick && SelectedObject != null)
                {
                    BattleMapViewer.cmsScriptMenu.Show(BattleMapViewer, PointToClient(Cursor.Position));
                }
            };

            for (int S = BattleMapViewer.ActiveMap.ListMapScript.Count - 1; S >= 0; --S)
            {
                BattleMapViewer.Helper.InitScript(BattleMapViewer.ActiveMap.ListMapScript[S]);
            }

            for (int T = 0; T < ListTerrainChangeLocation.Count; T++)
            {
                Vector2 NewDestinationPoint = ListTerrainChangeLocation[T];
                ActiveMap.LayerManager[0].ListSingleplayerSpawns.Add(new BattleMapScreen.EventPoint(new Vector3(NewDestinationPoint.X, NewDestinationPoint.Y, 0), T.ToString(), 255, 255, 255));
            }
        }
    }
}
