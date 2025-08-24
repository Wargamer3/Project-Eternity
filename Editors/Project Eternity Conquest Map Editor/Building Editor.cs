using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content.Builder;
using ProjectEternity.GameScreens.ConquestMapScreen;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Editors.UnitHubEditor
{
    public partial class UnitBuilderEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Unit, OriginalUnit };

        private ItemSelectionChoices ItemSelectionChoice;

        public UnitBuilderEditor()
        {
            InitializeComponent();
        }

        public UnitBuilderEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                cboTerrainType.SelectedIndex = 0;
                SaveItem(FilePath, FilePath);
            }

            viewerMapSprite.Preload();
            viewerBattleSprite.Preload();

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathBuildingsConquest, GUIRootPathBuildings }, "Buildings/Conquest/", new string[] { ".peb" }, typeof(UnitBuilderEditor))
            };
            
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);


            BW.Write(lstUnits.Items.Count);
            for (int U = 0; U < lstUnits.Items.Count; ++U)
            {
                BW.Write(lstUnits.Items[U].ToString());
            }

            BW.Write((byte)cboTerrainType.SelectedIndex);
            BW.Write(ckCapture.Checked);
            BW.Write((byte)txtVision.Value);
            BW.Write((int)txtHealth.Value);
            BW.Write(ckResupply.Checked);

            FS.Close();
            BW.Close();
        }

        private void LoadUnit(string BuildingPath)
        {
            ConquestTerrainHolder TerrainHolder = new ConquestTerrainHolder();
            TerrainHolder.LoadData();

            cboTerrainType.Items.Clear();
            for (int i = 0; i < TerrainHolder.ListConquestTerrainType.Count; ++i)
            {
                cboTerrainType.Items.Add(TerrainHolder.ListConquestTerrainType[i].TerrainName);
            }

            string FilePath = BuildingPath.Substring(0, BuildingPath.Length - 4).Substring(27);
            BuildingConquest NewUnit = new BuildingConquest(FilePath, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            for (int U = 0; U < NewUnit.ListUnitToSpawn.Count; ++U)
            {
                lstUnits.Items.Add(NewUnit.ListUnitToSpawn[U]);
            }

            cboTerrainType.SelectedIndex = NewUnit.TerrainType;
            ckCapture.Checked = NewUnit.CanBeCaptured;
            txtVision.Value = NewUnit.VisionRange;
            txtHealth.Value = NewUnit.HP;
            ckResupply.Checked = NewUnit.Resupply;

            if (File.Exists("Content/Buildings/Conquest/Map Sprites/" + NewUnit.RelativePath + ".xnb"))
            {
                viewerMapSprite.ChangeTexture("Buildings/Conquest/Map Sprites/" + NewUnit.RelativePath);
            }

            if (File.Exists("Content/Buildings/Conquest/Menu Sprites/" + NewUnit.RelativePath + ".xnb"))
            {
                viewerBattleSprite.ChangeTexture("Buildings/Conquest/Unit Sprites/" + NewUnit.RelativePath);
            }
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitsConquest));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex < 0)
                return;

            lstUnits.Items.RemoveAt(lstUnits.SelectedIndex);
        }

        private void btnSelectOrginalUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.OriginalUnit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnits));
        }

        private void btnImportMapSprite_Click(object sender, EventArgs e)
        {
            var SpriteFileDialog = new OpenFileDialog()
            {
                FileName = "Select a sprite to import",
                Filter = "Sprite files (*.png)|*.png",
                Title = "Open sprite file"
            };

            if (SpriteFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = SpriteFileDialog.FileName;
                var fileName = SpriteFileDialog.SafeFileName;
                var Builder = new ContentBuilder();
                Builder.Add(filePath, fileName.Substring(0, fileName.Length - 4), "TextureImporter", "TextureProcessor");
                string buildError = Builder.Build();

                string NewSpriteFileName = Path.GetFileNameWithoutExtension(FilePath);
                string MapSpriteFolder = "Content/Buildings/Conquest/Map Sprites";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(27);
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, MapSpriteFolder + "/" + NewSpriteFileFolder);

                viewerMapSprite.ChangeTexture("Buildings/Conquest/Map Sprites/" + NewSpriteFileFolder + " /" + NewSpriteFileName);
            }
        }

        private void btnImportMenuSprite_Click(object sender, EventArgs e)
        {
            var SpriteFileDialog = new OpenFileDialog()
            {
                FileName = "Select a sprite to import",
                Filter = "Sprite files (*.png)|*.png",
                Title = "Open sprite file"
            };

            if (SpriteFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = SpriteFileDialog.FileName;
                var fileName = SpriteFileDialog.SafeFileName;
                var Builder = new ContentBuilder();
                Builder.Add(filePath, fileName.Substring(0, fileName.Length - 4), "TextureImporter", "TextureProcessor");
                string buildError = Builder.Build();

                string NewSpriteFileName = Path.GetFileNameWithoutExtension(FilePath);
                string MapSpriteFolder = "Content/Buildings/Conquest/Menu Sprite";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(23);
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, MapSpriteFolder + "/" + NewSpriteFileFolder);

                viewerBattleSprite.ChangeTexture("Buildings/Conquest/Menu Sprite/" + NewSpriteFileFolder + " /" + NewSpriteFileName);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(23);

                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Unit:
                        lstUnits.Items.Add(Name);
                        break;
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }
    }
}
