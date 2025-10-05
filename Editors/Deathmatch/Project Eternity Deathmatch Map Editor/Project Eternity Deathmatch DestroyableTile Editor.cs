using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.TilesetEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.Editors.TilesetEditor.ProjectEternityTilesetPresetEditor;

namespace ProjectEternity.Editors.DeathmatchMapEditor
{
    public partial class ProjectEternityDeathmatchProjectEternityDestroyableTileEditor : ProjectEternityDestroyableTileEditor
    {
        public class DeathmatcDestroyableTiletHelper : ITilesetPresetHelper
        {
            public DeathmatcDestroyableTiletHelper()
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
                return GUIRootPathMapAutotilesImages;
            }
        }

        public ProjectEternityDeathmatchProjectEternityDestroyableTileEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityDeathmatchProjectEternityDestroyableTileEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathMaDestroyableTilesPresetsDeathmatch, GUIRootPathMaDestroyableTilesPresets }, "Maps/Destroyable Tiles Presets/Deathmatch/", new string[] { ".pedt" }, typeof(ProjectEternityDeathmatchProjectEternityDestroyableTileEditor), true)
            };

            return Info;
        }

        protected override void InitHelper()
        {
            Helper = new DeathmatcDestroyableTiletHelper();
        }
    }
}
