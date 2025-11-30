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

            LoadBuilding(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathBuildingsConquest, EditorHelper.GUIRootPathBuildings }, "Conquest/Buildings/", new string[] { ".peb" }, typeof(UnitBuilderEditor))
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
            BW.Write((int)txtCreditPerTurn.Value);
            BW.Write(ckResupply.Checked);

            BW.Write((byte)txtMapSpriteFPS.Value);
            BW.Write((byte)txtMapSpriteFramesPerRow.Value);
            BW.Write((byte)txtMapSpriteNumberOfRow.Value);

            BW.Write((byte)txtMenuSpriteFPS.Value);
            BW.Write((byte)txtMenuSpriteFramesPerRow.Value);
            BW.Write((byte)txtMenuSpriteNumberOfRow.Value);

            FS.Close();
            BW.Close();
        }

        private void LoadBuilding(string BuildingPath)
        {
            ConquestTerrainHolder TerrainHolder = new ConquestTerrainHolder();
            TerrainHolder.LoadData();

            cboTerrainType.Items.Clear();
            for (int i = 0; i < TerrainHolder.ListConquestTerrainType.Count; ++i)
            {
                cboTerrainType.Items.Add(TerrainHolder.ListConquestTerrainType[i].TerrainName);
            }

            string FilePath = BuildingPath.Substring(0, BuildingPath.Length - 4).Substring(27);
            BuildingConquest NewBuilding = new BuildingConquest(FilePath, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            for (int U = 0; U < NewBuilding.ListUnitToSpawn.Count; ++U)
            {
                lstUnits.Items.Add(NewBuilding.ListUnitToSpawn[U]);
            }

            cboTerrainType.SelectedIndex = NewBuilding.TerrainType;
            ckCapture.Checked = NewBuilding.CanBeCaptured;
            txtVision.Value = NewBuilding.VisionRange;
            txtHealth.Value = NewBuilding.MaxHP;
            txtCreditPerTurn.Value = NewBuilding.CreditPerTurn;
            ckResupply.Checked = NewBuilding.Resupply;

            txtMapSpriteFPS.Value = NewBuilding.MapSpriteFramesPerSecond;
            txtMapSpriteFramesPerRow.Value = NewBuilding.MapSpriteFramesPerLine;
            txtMapSpriteNumberOfRow.Value = NewBuilding.MapSpriteNumberOfLines;

            txtMenuSpriteFPS.Value = NewBuilding.MenuSpriteFramesPerSecond;
            txtMenuSpriteFramesPerRow.Value = NewBuilding.MenuSpriteFramesPerLine;
            txtMenuSpriteNumberOfRow.Value = NewBuilding.MenuSpriteNumberOfLines;

                if (File.Exists("Content/Conquest/Buildings/Map Sprites/" + NewBuilding.RelativePath + ".xnb"))
            {
                viewerMapSprite.ChangeTexture("Conquest/Buildings/Map Sprites/" + NewBuilding.RelativePath);
            }

            if (File.Exists("Content/Conquest/Buildings/Menu Sprites/" + NewBuilding.RelativePath + ".xnb"))
            {
                viewerBattleSprite.ChangeTexture("Conquest/Buildings/Unit Sprites/" + NewBuilding.RelativePath);
            }
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnitsConquest));
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
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnits));
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
                fileName = fileName.Substring(0, fileName.Length - 4);
                var Builder = new ContentBuilder();
                Builder.Add(filePath, fileName, "TextureImporter", "TextureProcessor");
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
                fileName = fileName.Substring(0, fileName.Length - 4);
                var Builder = new ContentBuilder();
                Builder.Add(filePath, fileName, "TextureImporter", "TextureProcessor");
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
