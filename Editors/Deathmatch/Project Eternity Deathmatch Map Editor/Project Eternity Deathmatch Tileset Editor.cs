using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.DeathmatchMapEditor
{
    public partial class ProjectEternityDeathmatchTilesetPresetEditor : ProjectEternityTilesetPresetEditor
    {
        public class DeathmatchTilesetPresetHelper : ITilesetPresetHelper
        {
            public DeathmatchTilesetPresetHelper()
            {
            }

            public void EditTerrainTypes()
            {
                throw new NotImplementedException();
            }

            public string[] GetTerrainTypes()
            {
                return new string[]
                {
                    "Land",
                    "Sea",
                    "Air",
                    "Space",
                };
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

        public ProjectEternityDeathmatchTilesetPresetEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityDeathmatchTilesetPresetEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathMapTilesetPresetsDeathmatch, EditorHelper.GUIRootPathMapTilesetPresets, EditorHelper.GUIRootPathMapTilesets }, "Deathmatch/Tilesets Presets/", new string[] { ".pet" }, typeof(ProjectEternityTilesetPresetEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new DeathmatchTilesetPresetHelper();
        }
    }
}
