using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimMapEditor
{
    public partial class ProjectEternityLifeSimTilesetPresetEditor : ProjectEternityTilesetPresetEditor
    {
        public class LifeSimTilesetPresetHelper : ITilesetPresetHelper
        {
            public LifeSimTilesetPresetHelper()
            {
            }

            public void EditTerrainTypes()
            {
                new ProjectEternityLifeSimTerrainsEditor().ShowDialog();
            }

            public string[] GetTerrainTypes()
            {
                LifeSimTerrainHolder TerrainHolder = new LifeSimTerrainHolder();
                TerrainHolder.LoadData();
                return TerrainHolder.GetTerrainTypes();
            }

            public TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new TilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new TilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            }

            public DestructibleTilesetPreset LoadDestructiblePreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                throw new NotImplementedException();
            }

            public TilesetPresetInformation CreateDestructiblePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                throw new NotImplementedException();
            }

            public void OnTerrainSelected(Terrain SelectedTerrain)
            {
                throw new NotImplementedException();
            }

            public string GetEditorPath()
            {
                return EditorHelper.GUIRootPathMapTilesetImages;
            }
        }

        public ProjectEternityLifeSimTilesetPresetEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityLifeSimTilesetPresetEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadTileset(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathMapTilesetPresetsLifeSim, EditorHelper.GUIRootPathMapTilesetPresets }, "Life Sim/Tilesets Presets/", new string[] { ".pet" }, typeof(ProjectEternityLifeSimTilesetPresetEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new LifeSimTilesetPresetHelper();
        }
    }
}
