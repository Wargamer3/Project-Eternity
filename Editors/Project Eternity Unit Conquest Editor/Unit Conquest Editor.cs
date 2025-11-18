using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Conquest;
using Microsoft.Xna.Framework.Content.Builder;

namespace ProjectEternity.Editors.UnitConquestEditor
{
    public partial class UnitConquestEditor : BaseEditor
    {
        public UnitConquestEditor()
        {
            InitializeComponent();
        }

        public UnitConquestEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathUnits, EditorHelper.GUIRootPathUnitsConquest }, "Conquest/Units/", new string[] { ".peu" }, typeof(UnitConquestEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write((Int32)txtHP.Value);
            BW.Write((Int32)txtMovement.Value);
            BW.Write((Int32)txtAmmo.Value);
            BW.Write((Int32)txtGaz.Value);
            BW.Write((Int32)txtCost.Value);
            BW.Write(cbMovementType.Text);
            BW.Write(cbArmourType.Text);
            BW.Write((Int32)txtGazCostPerTurn.Value);
            BW.Write((Int32)txtVisionRange.Value);

            BW.Write(txtWeapon1Name.Text);
            BW.Write(cbWeapon1PostMovement.Checked);
            BW.Write((byte)txtWeapon1MinimumRange.Value);
            BW.Write((byte)txtWeapon1MaximumRange.Value);

            BW.Write(txtWeapon2Name.Text);
            BW.Write(cbWeapon2PostMovement.Checked);
            BW.Write((byte)txtWeapon2MinimumRange.Value);
            BW.Write((byte)txtWeapon2MaximumRange.Value);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Unit.</param>
        private void LoadUnit(string UnitPath)
        {
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(23);
            UnitConquest LoadedUnit = new UnitConquest(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect);

            this.Text = LoadedUnit.RelativePath + " - Project Eternity Conquest Unit Editor";

            cbMovementType.SelectedItem = LoadedUnit.MovementType;
            cbArmourType.SelectedItem = LoadedUnit.ArmourType;
            txtHP.Value = LoadedUnit.MaxHP;
            txtMovement.Value = LoadedUnit.MaxMovement;
            txtAmmo.Value = LoadedUnit.MaxAmmo;
            txtGaz.Value = LoadedUnit.MaxGaz;
            txtCost.Value = LoadedUnit.Price;
            txtGazCostPerTurn.Value = LoadedUnit.GazCostPerTurn;
            txtVisionRange.Value = LoadedUnit.VisionRange;

            txtWeapon1Name.Text = LoadedUnit.Weapon1Name;
            cbWeapon1PostMovement.Checked = LoadedUnit.Weapon1PostMovement;
            txtWeapon1MinimumRange.Value = LoadedUnit.Weapon1MinimumRange;
            txtWeapon1MaximumRange.Value = LoadedUnit.Weapon1MaximumRange;

            txtWeapon2Name.Text = LoadedUnit.Weapon2Name;
            cbWeapon2PostMovement.Checked = LoadedUnit.Weapon2PostMovement;
            txtWeapon2MinimumRange.Value = LoadedUnit.Weapon2MinimumRange;
            txtWeapon2MaximumRange.Value = LoadedUnit.Weapon2MaximumRange;

            if (File.Exists("Content\\Units\\Conquest\\Map Sprite\\" + LoadedUnit.RelativePath + ".xnb"))
            {
                viewerMapSprite.ChangeTexture("Units\\Conquest\\Map Sprite\\" + LoadedUnit.RelativePath);
            }

            if (File.Exists("Content\\Units\\Conquest\\Unit Sprite\\" + LoadedUnit.RelativePath + ".xnb"))
            {
                viewerBattleSprite.ChangeTexture("Units\\Conquest\\Unit Sprite\\" + LoadedUnit.RelativePath);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
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
                string MapSpriteFolder = "Content\\Units\\Conquest\\Map Sprite";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(23);
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, MapSpriteFolder + "\\" + NewSpriteFileFolder);

                viewerMapSprite.ChangeTexture("Units\\Conquest\\Map Sprite\\" + NewSpriteFileFolder + " \\" + NewSpriteFileName);
            }
        }

        private void btnImportBattleSprite_Click(object sender, EventArgs e)
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
                string MapSpriteFolder = "Content\\Units\\Conquest\\Unit Sprite";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(23);
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, MapSpriteFolder + "\\" + NewSpriteFileFolder);

                viewerBattleSprite.ChangeTexture("Units\\Conquest\\Unit Sprite\\" + NewSpriteFileFolder + " \\" + NewSpriteFileName);
            }
        }
    }
}
