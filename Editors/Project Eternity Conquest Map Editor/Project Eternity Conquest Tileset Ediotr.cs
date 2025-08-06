using System;
using System.IO;
using System.Linq;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestTilesetPresetEditor : ProjectEternityTilesetPresetEditor
    {
        public class ConquesTilesetPresetHelper : ITilesetPresetHelper
        {
            public ConquesTilesetPresetHelper()
            {
            }

            public void EditTerrainTypes()
            {
                new ProjectEternityConquestTerrainsAndMoveTypesEditor().ShowDialog();
            }

            public string[] GetTerrainTypes()
            {
                ConquestTerrainHolder TerrainHolder = new ConquestTerrainHolder();
                TerrainHolder.LoadData();
                return TerrainHolder.ListConquestTerrainType.Select(x => x.TerrainName).ToArray();
            }

            public Terrain.TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new Terrain.TilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public void OnTerrainSelected(Terrain SelectedTerrain)
            {
                throw new NotImplementedException();
            }
        }

        public ProjectEternityConquestTilesetPresetEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityConquestTilesetPresetEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathMapTilesetPresetsConquest, GUIRootPathMapTilesetPresets, GUIRootPathMapTilesets }, "Tileset Presets/Conquest/", new string[] { ".pet" }, typeof(ProjectEternityConquestTilesetPresetEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new ConquesTilesetPresetHelper();
        }
    }
}
