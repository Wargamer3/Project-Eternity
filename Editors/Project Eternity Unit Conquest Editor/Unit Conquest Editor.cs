using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Editors.UnitConquestEditor
{
    public partial class UnitConquestEditor : BaseEditor
    {
        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;

        public UnitConquestEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
        }

        public UnitConquestEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
            }

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnits, GUIRootPathUnitsConquest }, "Units/Conquest/", new string[] { ".peu" }, typeof(UnitConquestEditor))
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
            BW.Write((Int32)txtMaterial.Value);
            BW.Write(cbMovementType.Text);
            BW.Write(cbArmourType.Text);
            BW.Write((Int32)txtGazCostPerTurn.Value);
            BW.Write((Int32)txtVisionRange.Value);

            BW.Write(txtWeapon1Name.Text);
            BW.Write(cbWeapon1PostMovement.Checked);
            BW.Write((Int32)txtWeapon1MinimumRange.Value);
            BW.Write((Int32)txtWeapon1MaximumRange.Value);

            BW.Write(txtWeapon2Name.Text);
            BW.Write(cbWeapon2PostMovement.Checked);
            BW.Write((Int32)txtWeapon2MinimumRange.Value);
            BW.Write((Int32)txtWeapon2MaximumRange.Value);

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
            UnitConquest LoadedUnit = new UnitConquest(Name, null, DicRequirement, DicEffect);

            this.Text = LoadedUnit.RelativePath + " - Project Eternity Conquest Unit Editor";

            cbMovementType.Text = LoadedUnit.ListTerrainChoices[0];
            txtHP.Value = LoadedUnit.MaxHP / 10;
            txtMovement.Value = LoadedUnit.MaxMovement;
            txtAmmo.Value = LoadedUnit.Ammo;
            txtGaz.Value = LoadedUnit.Gaz;
            txtMaterial.Value = LoadedUnit.Material;
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
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }
    }
}
