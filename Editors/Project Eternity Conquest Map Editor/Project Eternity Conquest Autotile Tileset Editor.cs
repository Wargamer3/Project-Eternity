using System;
using System.IO;
using System.Linq;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using static ProjectEternity.Editors.TilesetEditor.ProjectEternityTilesetPresetEditor;
using static ProjectEternity.GameScreens.BattleMapScreen.TilesetPreset;
using static ProjectEternity.GameScreens.ConquestMapScreen.ConquestTilesetPreset;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestAutotileTilesetPresetEditor : ProjectEternityAutotileTilesetPresetEditor
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

            public TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new ConquestTilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new ConquestTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            }

            public void OnTerrainSelected(Terrain SelectedTerrain)
            {
                throw new NotImplementedException();
            }

            public string GetEditorPath()
            {
                return GUIRootPathMapAutotilesImages;
            }
        }

        public ProjectEternityConquestAutotileTilesetPresetEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityConquestAutotileTilesetPresetEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathMapAutotilesPresetsConquest, GUIRootPathMapAutotilesPresets, GUIRootPathMapAutotiles }, "Maps/Autotiles Presets/Conquest/", new string[] { ".peat" }, typeof(ProjectEternityConquestAutotileTilesetPresetEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new ConquesTilesetPresetHelper();
        }
    }
}
