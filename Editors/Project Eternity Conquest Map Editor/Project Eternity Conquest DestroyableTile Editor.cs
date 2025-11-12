using System;
using System.IO;
using System.Linq;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using static ProjectEternity.Editors.TilesetEditor.ProjectEternityTilesetPresetEditor;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestDestroyableTileEditor : ProjectEternityDestroyableTileEditor
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
                throw new NotImplementedException();
            }

            public TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                throw new NotImplementedException();
            }

            public DestructibleTilesetPreset LoadDestructiblePreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new DestructibleTilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public TilesetPresetInformation CreateDestructiblePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new TilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            }

            public void OnTerrainSelected(Terrain SelectedTerrain)
            {
                throw new NotImplementedException();
            }

            public string GetEditorPath()
            {
                return EditorHelper.GUIRootPathMapDestroyableTilesImages;
            }
        }

        public ProjectEternityConquestDestroyableTileEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityConquestDestroyableTileEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathMapDestroyableTilesPresetsConquest, EditorHelper.GUIRootPathMapDestroyableTilesPresets }, "Conquest/Destroyable Tiles Presets/", new string[] { ".pedt" }, typeof(ProjectEternityConquestDestroyableTileEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new ConquesTilesetPresetHelper();
        }
    }
}
