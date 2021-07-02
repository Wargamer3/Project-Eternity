using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Normal;
using ProjectEternity.Core.Item;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.Editors.AttackEditor;
using ProjectEternity.Core.Units;

namespace ProjectEternity.Editors.UnitNormalEditor
{
    public partial class UnitNormalEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Pilot, Animations, Abilities };

        private ItemSelectionChoices ItemSelectionChoice;
        private Attacks frmAttacks;
        private DetailsEditor frmDetails;
        private UnitSizeEditor frmUnitSizeEditor;

        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;

        public UnitNormalEditor()
        {
            InitializeComponent();
            FilePath = null;

            frmAttacks = new Attacks();
            frmDetails = new DetailsEditor();
            frmUnitSizeEditor = new UnitSizeEditor();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            txtName.Text = "";
            txtPrice.Text = "0";
            txtDescription.Text = "";

            txtBaseHP.Text = "0";
            txtBaseEN.Text = "0";
            txtBaseArmor.Text = "0";
            txtBaseMobility.Text = "0";
            txtBaseMovement.Text = "0";
            txtPartsSlots.Value = 1;

            lstPilots.Items.Clear();

            lstAnimations.Items.Add(new ListViewItem("Default"));
            lstAnimations.Items.Add(new ListViewItem("Hit"));
            lstAnimations.Items.Add(new ListViewItem("Hit To Default"));
            lstAnimations.Items.Add(new ListViewItem("Move Foward"));
            lstAnimations.Items.Add(new ListViewItem("Move Foward To Default"));
            lstAnimations.Items.Add(new ListViewItem("Move Backward"));
            lstAnimations.Items.Add(new ListViewItem("Move Backward To Default"));
            lstAnimations.Items.Add(new ListViewItem("Destroyed"));
            lstAnimations.Items.Add(new ListViewItem("Shield"));
            lstAnimations.Items.Add(new ListViewItem("Shield To Default"));
            lstAnimations.Items.Add(new ListViewItem("Parry Start"));
            lstAnimations.Items.Add(new ListViewItem("Parry End"));
            lstAnimations.Items.Add(new ListViewItem("Shoot Down Firing"));
            lstAnimations.Items.Add(new ListViewItem("Shoot Down Shot"));
        }

        public UnitNormalEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                lstAnimations.Items[0].Tag = "";
                lstAnimations.Items[1].Tag = "";
                lstAnimations.Items[2].Tag = "";
                lstAnimations.Items[3].Tag = "";
                lstAnimations.Items[4].Tag = "";
                lstAnimations.Items[5].Tag = "";
                lstAnimations.Items[6].Tag = "";
                lstAnimations.Items[7].Tag = "";
                lstAnimations.Items[8].Tag = "";
                lstAnimations.Items[9].Tag = "";
                lstAnimations.Items[10].Tag = "";
                lstAnimations.Items[11].Tag = "";
                lstAnimations.Items[12].Tag = "";
                lstAnimations.Items[13].Tag = "";
                SaveItem(FilePath, FilePath);
            }

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnitsNormal, GUIRootPathUnits }, "Units/Normal/", new string[] { ".peu", ".txt" }, typeof(UnitNormalEditor)),
                new EditorInfo(new string[] { GUIRootPathUnitsNormalMapSprites }, "Units/Normal/Map Sprite/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathUnitsNormalUnitSprites }, "Units/Normal/Unit Sprite/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);
            BW.Write(txtName.Text);
            BW.Write(frmDetails.txtMapSprite.Text);
            BW.Write(frmDetails.txtUnitSprite.Text);
            BW.Write(frmDetails.txtTags.Text);
            BW.Write(txtDescription.Text);
            BW.Write(Convert.ToInt32(txtPrice.Text));

            BW.Write((int)txtEXP.Value);
            BW.Write(Convert.ToInt32(txtBaseHP.Text));
            BW.Write(Convert.ToInt32(txtBaseEN.Text));
            BW.Write(Convert.ToInt32(txtBaseArmor.Text));
            BW.Write(Convert.ToInt32(txtBaseMobility.Text));
            BW.Write(Convert.ToSingle(txtBaseMovement.Text));
            BW.Write(frmAttacks.AttackUpgradesValueIndex);
            BW.Write(frmAttacks.AttackUpgradesCostIndex);

            Dictionary<string, int> DicTerrainValue = new Dictionary<string, int>();
            DicTerrainValue.Add(UnitStats.TerrainAir, 0);
            DicTerrainValue.Add(UnitStats.TerrainLand, 0);
            DicTerrainValue.Add(UnitStats.TerrainSea, 0);
            DicTerrainValue.Add(UnitStats.TerrainSpace, 0);
            if (cbTerrainAir.SelectedIndex >= 0)
            {
                DicTerrainValue[UnitStats.TerrainAir] = cbTerrainAir.SelectedIndex;
            }
            if (cbTerrainLand.SelectedIndex >= 0)
            {
                DicTerrainValue[UnitStats.TerrainLand] = cbTerrainLand.SelectedIndex;
            }
            if (cbTerrainSea.SelectedIndex >= 0)
            {
                DicTerrainValue[UnitStats.TerrainSea] = cbTerrainSea.SelectedIndex;
            }
            if (cbTerrainSpace.SelectedIndex >= 0)
            {
                DicTerrainValue[UnitStats.TerrainSpace] = cbTerrainSpace.SelectedIndex;
            }

            BW.Write(DicTerrainValue.Count);
            foreach (KeyValuePair<string, int> TerrainValue in DicTerrainValue)
            {
                BW.Write(TerrainValue.Key);
                BW.Write(TerrainValue.Value);
            }

            List<string> UnitMovements = new List<string>();
            if (cboMovementAir.Checked)
                UnitMovements.Add(UnitStats.TerrainAir);
            if (cboMovementLand.Checked)
                UnitMovements.Add(UnitStats.TerrainLand);
            if (cboMovementSea.Checked)
                UnitMovements.Add(UnitStats.TerrainSea);
            if (cboMovementSpace.Checked)
                UnitMovements.Add(UnitStats.TerrainSpace);
            if (cboMovementUnderground.Checked)
                UnitMovements.Add(UnitStats.TerrainUnderground);
            if (cboMovementUnderwater.Checked)
                UnitMovements.Add(UnitStats.TerrainUnderwater);

            BW.Write(UnitMovements.Count);
            for (int i = 0; i < UnitMovements.Count; i++)
                BW.Write(UnitMovements[i]);

            if (rbSizeLLL.Checked)
                BW.Write("LLL");
            else if (rbSizeLL.Checked)
                BW.Write("LL");
            else if (rbSizeL.Checked)
                BW.Write("L");
            else if (rbSizeM.Checked)
                BW.Write("M");
            else if (rbSizeS.Checked)
                BW.Write("S");
            else if (rbSizeSS.Checked)
                BW.Write("SS");

            if (frmUnitSizeEditor.rbNone.Checked)
            {
                BW.Write(1);
                BW.Write(1);
                BW.Write(true);
            }
            else if (frmUnitSizeEditor.rbSizeOnly.Checked)
            {
                BW.Write((int)frmUnitSizeEditor.txtWidth.Value);
                BW.Write((int)frmUnitSizeEditor.txtWidth.Value);
                for (int X = 0; X < frmUnitSizeEditor.txtWidth.Value; X++)
                {
                    for (int Y = 0; Y < frmUnitSizeEditor.txtHeight.Value; Y++)
                    {
                        BW.Write(true);
                    }
                }
            }
            else if (frmUnitSizeEditor.rbCustomSizeBox.Checked)
            {
                BW.Write((int)frmUnitSizeEditor.txtWidth.Value);
                BW.Write((int)frmUnitSizeEditor.txtWidth.Value);
                for (int X = 0; X < frmUnitSizeEditor.txtWidth.Value; X++)
                {
                    for (int Y = 0; Y < frmUnitSizeEditor.txtHeight.Value; Y++)
                    {
                        BW.Write(frmUnitSizeEditor.ListUnitSize[X][Y]);
                    }
                }
            }

            //Write Pilots whitelist.
            BW.Write(lstPilots.Items.Count);
            for (int P = 0; P < lstPilots.Items.Count; ++P)
                BW.Write((string)lstPilots.Items[P]);

            //Attacks.
            BW.Write(frmAttacks.ListAttack.Count);
            for (int A = 0; A < frmAttacks.ListAttack.Count; ++A)
            {
                BW.Write(frmAttacks.ListAttack[A].IsExternal);
                BW.Write(frmAttacks.ListAttack[A].RelativePath);

                if (!frmAttacks.ListAttack[A].IsExternal)
                {
                    ProjectEternityAttackEditor ActiveAttack = (ProjectEternityAttackEditor)frmAttacks.lstAttack.Items[A];
                    ActiveAttack.SaveItem(BW);
                }

                BW.Write(frmAttacks.ListAttack[A].Animations.Count);
                for (int An = 0; An < frmAttacks.ListAttack[A].Animations.Count; ++An)
                {
                    frmAttacks.ListAttack[A].Animations[An].Save(BW);
                }
            }

            //Animations.
            BW.Write(lstAnimations.Items.Count);
            for (int A = 0; A < lstAnimations.Items.Count; ++A)
                BW.Write((string)lstAnimations.Items[A].Tag);

            //Abilities.
            BW.Write(lstAbilities.Items.Count);
            for (int A = 0; A < lstAbilities.Items.Count; ++A)
                BW.Write((string)lstAbilities.Items[A]);

            //Part slots.
            BW.Write((int)txtPartsSlots.Value);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Unit.</param>
        private void LoadUnit(string UnitPath)
        {
            Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(21);
            UnitNormal LoadedUnit = new UnitNormal(Name, null, DicRequirement, DicEffect);

            frmAttacks.UnitName = Name;

            #region Unit Animations
            
            if (string.IsNullOrEmpty(LoadedUnit.Animations.Default.AnimationName))
                lstAnimations.Items[0].Tag = Name + "/Default";
            else

                lstAnimations.Items[0].Tag = LoadedUnit.Animations.Default.AnimationName;
            if (string.IsNullOrEmpty(LoadedUnit.Animations.Hit.AnimationName))
                lstAnimations.Items[1].Tag = "Default Animations/Hit";
            else
                lstAnimations.Items[1].Tag = LoadedUnit.Animations.Hit.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.HitToDefault.AnimationName))
                lstAnimations.Items[2].Tag = "Default Animations/Hit To Default";
            else
                lstAnimations.Items[2].Tag = LoadedUnit.Animations.HitToDefault.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.MoveFoward.AnimationName))
                lstAnimations.Items[3].Tag = "Default Animations/Move Foward";
            else
                lstAnimations.Items[3].Tag = LoadedUnit.Animations.MoveFoward.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.MoveFowardToDefault.AnimationName))
                lstAnimations.Items[4].Tag = "Default Animations/Move Foward to Default";
            else
                lstAnimations.Items[4].Tag = LoadedUnit.Animations.MoveFowardToDefault.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.MoveBackward.AnimationName))
                lstAnimations.Items[5].Tag = "Default Animations/Move Backward";
            else
                lstAnimations.Items[5].Tag = LoadedUnit.Animations.MoveBackward.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.MoveBackwardToDefault.AnimationName))
                lstAnimations.Items[6].Tag = "Default Animations/Move Backward To Default";
            else
                lstAnimations.Items[6].Tag = LoadedUnit.Animations.MoveBackwardToDefault.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.Destroyed.AnimationName))
                lstAnimations.Items[7].Tag = "Default Animations/Destroyed";
            else
                lstAnimations.Items[7].Tag = LoadedUnit.Animations.Destroyed.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.Shield.AnimationName))
                lstAnimations.Items[8].Tag = "Default Animations/Shield";
            else
                lstAnimations.Items[8].Tag = LoadedUnit.Animations.Shield.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.ShieldToDefault.AnimationName))
                lstAnimations.Items[9].Tag = "Default Animations/Shield To Default";
            else
                lstAnimations.Items[9].Tag = LoadedUnit.Animations.ShieldToDefault.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.ParryStart.AnimationName))
                lstAnimations.Items[10].Tag = "Default Animations/Parry Start";
            else
                lstAnimations.Items[10].Tag = LoadedUnit.Animations.ParryStart.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.ParryEnd.AnimationName))
                lstAnimations.Items[11].Tag = "Default Animations/Parry End";
            else
                lstAnimations.Items[11].Tag = LoadedUnit.Animations.ParryEnd.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.ShootDownFiring.AnimationName))
                lstAnimations.Items[12].Tag = "Default Animations/Shoot Down Firing";
            else
                lstAnimations.Items[12].Tag = LoadedUnit.Animations.ShootDownFiring.AnimationName;

            if (string.IsNullOrEmpty(LoadedUnit.Animations.ShootDownShot.AnimationName))
                lstAnimations.Items[13].Tag = "Default Animations/Shoot Down Shot";
            else
                lstAnimations.Items[13].Tag = LoadedUnit.Animations.ShootDownShot.AnimationName;

            #endregion

            //Pilot whitelist.
            for (int P = 0; P < LoadedUnit.ListCharacterIDWhitelist.Count; P++)
            {
                lstPilots.Items.Add(LoadedUnit.ListCharacterIDWhitelist[P]);
            }

            frmAttacks.SetAttacks(LoadedUnit.ListAttack);
            frmAttacks.cbUpgradeCost.SelectedIndex = (int)LoadedUnit.UnitStat.AttackUpgradesCost;
            frmAttacks.cbUpgradeValues.SelectedIndex = (int)LoadedUnit.UnitStat.AttackUpgradesSpeed;

            frmDetails.txtMapSprite.Text = LoadedUnit.SpriteMapPath;
            frmDetails.txtUnitSprite.Text = LoadedUnit.SpriteUnitPath;
            frmDetails.txtTags.Text = LoadedUnit.UnitTags;

            #region Abilities

            for (int A = 0; A < LoadedUnit.ArrayUnitAbility.Length; A++)
            {
                lstAbilities.Items.Add(LoadedUnit.ArrayUnitAbility[A].Name);
            }

            #endregion

            txtPartsSlots.Value = LoadedUnit.ArrayParts.Length;

            this.Text = LoadedUnit.RelativePath + " - Project Eternity Unit Editor";

            #region Load controls

            txtName.Text = LoadedUnit.ItemName;
            txtPrice.Text = LoadedUnit.Price.ToString();
            txtEXP.Value = LoadedUnit.UnitStat.EXPValue;
            txtDescription.Text = LoadedUnit.Description;

            txtBaseHP.Text = LoadedUnit.MaxHP.ToString();
            txtBaseEN.Text = LoadedUnit.MaxEN.ToString();
            txtBaseArmor.Text = LoadedUnit.Armor.ToString();
            txtBaseMobility.Text = LoadedUnit.Mobility.ToString();
            txtBaseMovement.Text = LoadedUnit.MaxMovement.ToString();

            List<char> Grades = new List<char> { '-', 'S', 'A', 'B', 'C', 'D' };
            cbTerrainAir.SelectedIndex = LoadedUnit.DicTerrainValue[UnitStats.TerrainAir];
            cbTerrainLand.SelectedIndex = LoadedUnit.DicTerrainValue[UnitStats.TerrainLand];
            cbTerrainSea.SelectedIndex = LoadedUnit.DicTerrainValue[UnitStats.TerrainSea];
            cbTerrainSpace.SelectedIndex = LoadedUnit.DicTerrainValue[UnitStats.TerrainSpace];

            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                cboMovementAir.Checked = true;
            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
                cboMovementLand.Checked = true;
            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
                cboMovementSea.Checked = true;
            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
                cboMovementSpace.Checked = true;
            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainUnderground))
                cboMovementUnderground.Checked = true;
            if (LoadedUnit.ListTerrainChoices.Contains(UnitStats.TerrainUnderwater))
                cboMovementUnderwater.Checked = true;

            if (LoadedUnit.Size == "LLL")
                rbSizeLLL.Checked = true;
            else if (LoadedUnit.Size == "LL")
                rbSizeLL.Checked = true;
            else if (LoadedUnit.Size == "L")
                rbSizeL.Checked = true;
            else if (LoadedUnit.Size == "M")
                rbSizeM.Checked = true;
            else if (LoadedUnit.Size == "S")
                rbSizeS.Checked = true;
            else if (LoadedUnit.Size == "SS")
                rbSizeSS.Checked = true;

            if (LoadedUnit.UnitStat.ArrayMapSize.GetLength(0) == 1 && LoadedUnit.UnitStat.ArrayMapSize.GetLength(1) == 1)
            {
                frmUnitSizeEditor.rbNone.Checked = true;
            }
            else
            {
                frmUnitSizeEditor.txtWidth.Value = LoadedUnit.UnitStat.ArrayMapSize.GetLength(0);
                frmUnitSizeEditor.txtHeight.Value = LoadedUnit.UnitStat.ArrayMapSize.GetLength(1);
                bool AllTrue = true;

                for (int X = 0; X < LoadedUnit.UnitStat.ArrayMapSize.GetLength(0); X++)
                {
                    for (int Y = 0; Y < LoadedUnit.UnitStat.ArrayMapSize.GetLength(1); Y++)
                    {
                        if (!LoadedUnit.UnitStat.ArrayMapSize[X, Y])
                        {
                            AllTrue = false;
                        }
                        frmUnitSizeEditor.ListUnitSize[X][Y] = LoadedUnit.UnitStat.ArrayMapSize[X, Y];
                    }
                }

                if (AllTrue)
                {
                    frmUnitSizeEditor.rbSizeOnly.Checked = true;
                }
                else
                {
                    frmUnitSizeEditor.rbCustomSizeBox.Checked = true;
                }
            }

            #endregion
        }

        private void btnAddPilot_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Pilot;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        private void btnRemovePilot_Click(object sender, EventArgs e)
        {
            if (lstPilots.SelectedIndex == -1)
                return;

            int Index = lstPilots.SelectedIndex;
            lstPilots.Items.RemoveAt(Index);

            //SelectedIndex = -1 at this point
            if (Index < lstPilots.Items.Count)
                lstPilots.SelectedIndex = Index;
            else if (lstPilots.Items.Count > 0)
                lstPilots.SelectedIndex = Index - 1;
        }

        private void btnEditMapSize_Click(object sender, EventArgs e)
        {
            frmUnitSizeEditor.ShowDialog();
        }

        private void btnEditAttacks_Click(object sender, EventArgs e)
        {
            frmAttacks.ShowDialog();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmExport_Click(object sender, EventArgs e)
        {
            IniFile ExportIniFile = new IniFile();
            ExportIniFile.AddValue("Unit Stats", "Description", txtDescription.Text);
            ExportIniFile.AddValue("Unit Stats", "Price", txtPrice.Text);
            ExportIniFile.AddValue("Unit Stats", "EXP", txtEXP.Text);
            ExportIniFile.AddValue("Unit Stats", "BaseHP", txtBaseHP.Text);
            ExportIniFile.AddValue("Unit Stats", "BaseEN", txtBaseEN.Text);
            ExportIniFile.AddValue("Unit Stats", "BaseArmor", txtBaseArmor.Text);
            ExportIniFile.AddValue("Unit Stats", "BaseMobility", txtBaseMobility.Text);
            ExportIniFile.AddValue("Unit Stats", "BaseMovement", txtBaseMovement.Text);
            ExportIniFile.AddValue("Unit Stats", "AttackUpgradesValueIndex", frmAttacks.AttackUpgradesValueIndex.ToString());
            ExportIniFile.AddValue("Unit Stats", "AttackUpgradesCostIndex", frmAttacks.AttackUpgradesCostIndex.ToString());

            if (cbTerrainAir.SelectedIndex >= 0)
            {
                ExportIniFile.AddValue("Unit Terrain", UnitStats.TerrainAir, cbTerrainAir.Text);
            }
            if (cbTerrainLand.SelectedIndex >= 0)
            {
                ExportIniFile.AddValue("Unit Terrain", UnitStats.TerrainLand, cbTerrainLand.Text);
            }
            if (cbTerrainSea.SelectedIndex >= 0)
            {
                ExportIniFile.AddValue("Unit Terrain", UnitStats.TerrainSea, cbTerrainSea.Text);
            }
            if (cbTerrainSpace.SelectedIndex >= 0)
            {
                ExportIniFile.AddValue("Unit Terrain", UnitStats.TerrainSpace, cbTerrainSpace.Text);
            }

            List<string> UnitMovements = new List<string>();
            if (cboMovementAir.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainAir, "");
            if (cboMovementLand.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainLand, "");
            if (cboMovementSea.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainSea, "");
            if (cboMovementSpace.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainSpace, "");
            if (cboMovementUnderground.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainUnderground, "");
            if (cboMovementUnderwater.Checked)
                ExportIniFile.AddValue("Unit Movements", UnitStats.TerrainUnderwater, "");

            if (rbSizeLLL.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "LLL");
            else if (rbSizeLL.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "LL");
            else if (rbSizeL.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "L");
            else if (rbSizeM.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "M");
            else if (rbSizeS.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "S");
            else if (rbSizeSS.Checked)
                ExportIniFile.AddValue("Unit Stats", "Size", "SS");

            if (frmUnitSizeEditor.rbNone.Checked)
            {
                ExportIniFile.AddValue("Unit Stats", "Size Mask Width", "1");
                ExportIniFile.AddValue("Unit Stats", "Size Mask Height", "1");
                ExportIniFile.AddValue("Size Mask", "Pos X0Y0", "true");
            }
            else if (frmUnitSizeEditor.rbSizeOnly.Checked)
            {
                ExportIniFile.AddValue("Unit Stats", "Size Mask Width", frmUnitSizeEditor.txtWidth.Value.ToString());
                ExportIniFile.AddValue("Unit Stats", "Size Mask Height", frmUnitSizeEditor.txtHeight.Value.ToString());
                for (int X = 0; X < frmUnitSizeEditor.txtWidth.Value; X++)
                {
                    for (int Y = 0; Y < frmUnitSizeEditor.txtHeight.Value; Y++)
                    {
                        ExportIniFile.AddValue("Size Mask", "Pos X" + X + "Y" + Y, "true");
                    }
                }
            }
            else if (frmUnitSizeEditor.rbCustomSizeBox.Checked)
            {
                ExportIniFile.AddValue("Unit Stats", "Size Mask Width", frmUnitSizeEditor.txtWidth.Value.ToString());
                ExportIniFile.AddValue("Unit Stats", "Size Mask Height", frmUnitSizeEditor.txtHeight.Value.ToString());
                for (int X = 0; X < frmUnitSizeEditor.txtWidth.Value; X++)
                {
                    for (int Y = 0; Y < frmUnitSizeEditor.txtHeight.Value; Y++)
                    {
                        ExportIniFile.AddValue("Size Mask", "Pos X" + X + "Y" + Y, frmUnitSizeEditor.ListUnitSize[X][Y].ToString());
                    }
                }
            }

            //Write Pilots whitelist.
            for (int P = 0; P < lstPilots.Items.Count; ++P)
                ExportIniFile.AddValue("Pilot Whitelist", "Pilot #" + P, (string)lstPilots.Items[P]);

            //Attacks.
            for (int A = 0; A < frmAttacks.ListAttack.Count; ++A)
            {
                ExportIniFile.AddValue("Attacks", "Attack " + A, frmAttacks.ListAttack[A].RelativePath);
                for (int An = 0; An < frmAttacks.ListAttack[A].Animations.Count; ++An)
                {
                    ExportIniFile.AddValue("Attack Animations", "Attack " + A + " Path " + An, frmAttacks.ListAttack[A].Animations[0].Animations[An].AnimationName);
                }
            }

            //Animations.
            for (int A = 0; A < lstAnimations.Items.Count; ++A)
                ExportIniFile.AddValue("Animations", "Path " + A, (string)lstAnimations.Items[A].Tag);

            //Abilities.
            for (int A = 0; A < lstAbilities.Items.Count; ++A)
                ExportIniFile.AddValue("Abilities", "Path " + A, (string)lstAbilities.Items[A]);

            ExportIniFile.AddValue("Unit Stats", "Parts Slots", txtPartsSlots.Value.ToString());

            SaveFileDialog SaveDialog = new SaveFileDialog();
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportIniFile.SaveToFile(SaveDialog.FileName);
            }
        }

        private void tsmDetails_Click(object sender, EventArgs e)
        {
            frmDetails.ShowDialog();
        }

        private void lstAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAnimations.SelectedIndices.Count > 0)
            {
                txtAnimationName.Text = (string)lstAnimations.Items[lstAnimations.SelectedIndices[0]].Tag;
            }
        }

        private void btnSelectAnimation_Click(object sender, EventArgs e)
        {
            if (lstAnimations.SelectedIndices.Count > 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Animations;
                ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAnimations));
            }
        }

        #region Abilities

        private void btnAddAbility_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Abilities;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitAbilities));
        }

        private void btnRemoveAbility_Click(object sender, EventArgs e)
        {
            if (lstAbilities.SelectedIndex != -1)
                lstAbilities.Items.RemoveAt(lstAbilities.SelectedIndex);
        }

        private void btnMoveUpAbility_Click(object sender, EventArgs e)
        {
            string AbilityName;
            int Index;

            if (lstAbilities.SelectedIndex > 0)
            {
                AbilityName = (string)lstAbilities.Items[lstAbilities.SelectedIndex];
                Index = lstAbilities.SelectedIndex - 1;

                lstAbilities.Items.RemoveAt(lstAbilities.SelectedIndex);
                //Selected Index is now -1.
                lstAbilities.Items.Insert(Index, AbilityName);

                lstAbilities.SelectedIndex = Index;
            }
        }

        private void btnMoveDownAbility_Click(object sender, EventArgs e)
        {
            string AbilityName;
            int Index;

            if (lstAbilities.SelectedIndex >= 0 && lstAbilities.SelectedIndex < lstAbilities.Items.Count - 1)
            {
                AbilityName = (string)lstAbilities.Items[lstAbilities.SelectedIndex];
                Index = lstAbilities.SelectedIndex + 1;

                lstAbilities.Items.RemoveAt(lstAbilities.SelectedIndex);
                //Selected Index is now -1.
                lstAbilities.Items.Insert(Index, AbilityName);

                lstAbilities.SelectedIndex = Index;
            }
        }

        #endregion

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Pilot:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Characters") + 11);
                        if (Name != null)
                        {
                            if (lstPilots.Items.Contains(Name))
                            {
                                MessageBox.Show("This pilot is already listed.\r\n" + Name);
                                return;
                            }
                            lstPilots.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.Abilities:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Abilities") + 10);
                        if (Name != null)
                        {
                            if (lstAbilities.Items.Contains(Name))
                            {
                                MessageBox.Show("This ability is already listed.\r\n" + Name);
                                return;
                            }
                            lstAbilities.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.Animations:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(19);
                        lstAnimations.Items[lstAnimations.SelectedIndices[0]].Tag = Name;
                        txtAnimationName.Text = Name;
                        break;
                }
            }
        }
    }
}
