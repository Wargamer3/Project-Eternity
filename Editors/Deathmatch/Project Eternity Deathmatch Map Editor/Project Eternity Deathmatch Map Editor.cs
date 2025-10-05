using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Editors.DeathmatchMapEditor
{
    public partial class ProjectEternityDeathmatchEditor : ProjectEternityMapEditor
    {
        private static DeathmatchParams Params;

        public ProjectEternityDeathmatchEditor()
            : base()
        {
            InitializeComponent();

            KeyPreview = true;

            if (Params == null)
            {
                Params = new DeathmatchParams(new BattleContext());
            }
        }

        public ProjectEternityDeathmatchEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                DeathmatchMap NewMap = new DeathmatchMap(FilePath, new GameModeInfo(), ProjectEternityDeathmatchEditor.Params);
                BattleMapViewer.ActiveMap = NewMap;
                NewMap.LayerManager.ListLayer.Add(new MapLayer(NewMap, 0));

                SaveItem(FilePath, FilePath);
            }

            LoadMap(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathMaps, GUIRootPathDeathmatchMaps }, "Maps/Deathmatch/", new string[] { ".pem" }, typeof(ProjectEternityDeathmatchEditor))
            };

            return Info;
        }
        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(24);

            BattleMapViewer.Preload();
            DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, new GameModeInfo(), Params);
            Helper = new DeathmatchMapHelper(NewMap);
            InitMap(NewMap);

            this.Text = MapLogicName + " - Project Eternity Deathmatch Map Editor";
        }
    }
}
