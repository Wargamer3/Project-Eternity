using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetTilesetPreset;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class ProjectEternitySorcererStreetTilesetPresetEditor : ProjectEternityTilesetPresetEditor
    {
        public class SorcererStreetTilesetPresetHelper : ITilesetPresetHelper
        {
            public SorcererStreetTilesetPresetHelper()
            {
            }

            public void EditTerrainTypes()
            {
                new ProjectEternitySorcererStreetTerrainsEditor().ShowDialog();
            }

            public string[] GetTerrainTypes()
            {
                SorcererStreetTerrainHolder TerrainHolder = new SorcererStreetTerrainHolder();
                TerrainHolder.LoadData();
                return TerrainHolder.ListTerrainType.ToArray();
            }

            public TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new SorcererStreetTilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new SorcererStreetTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
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

        public ProjectEternitySorcererStreetTilesetPresetEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternitySorcererStreetTilesetPresetEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathMapTilesetPresetsSorcererStreet, EditorHelper.GUIRootPathMapTilesetPresets }, "Sorcerer Street/Tilesets Presets/", new string[] { ".pet" }, typeof(ProjectEternitySorcererStreetTilesetPresetEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new SorcererStreetTilesetPresetHelper();
        }
    }
}
