using System;
using System.IO;
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
                return ConquestMap.AllTerrains;
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
