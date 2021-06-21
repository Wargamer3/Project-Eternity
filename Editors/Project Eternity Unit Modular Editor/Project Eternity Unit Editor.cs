using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Modular;

namespace ProjectEternity.Editors.UnitModularEditor
{
    public partial class ProjectEternityUnitModularEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Pilot, Head, Torso, LeftArm, RightArm, Legs };

        private ItemSelectionChoices ItemSelectionChoice;

        private List<Character> ListPilot;

        private List<PartHead> ListHead;
        private List<PartTorso> ListTorso;
        private List<PartArm> ListLeftArm;
        private List<PartArm> ListRightArm;
        private List<PartLegs> ListLegs;

        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;

        public ProjectEternityUnitModularEditor()
        {
            InitializeComponent();

            ListPilot = new List<Character>();

            ListHead = new List<PartHead>();
            ListTorso = new List<PartTorso>();
            ListLeftArm = new List<PartArm>();
            ListRightArm = new List<PartArm>();
            ListLegs = new List<PartLegs>();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            ResetControls();
        }

        public ProjectEternityUnitModularEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { "Units" }, "Units/Modular/", new string[] { ".peu" }, typeof(ProjectEternityUnitModularEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            Int32 Price = Convert.ToInt32(txtPrice.Text);
            string Description = txtDescription.Text;

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);
            BW.Write(Description);
            BW.Write(Price);
            
            //Write Pilots whitelist.
            BW.Write(lstPilots.Items.Count);
            for (int P = 0; P < lstPilots.Items.Count; P++)
                BW.Write((string)lstPilots.Items[P]);

            BW.Write(lstHead.Items.Count);
            for (int H = 0; H < lstHead.Items.Count; H++)
                BW.Write((string)lstHead.Items[H]);

            BW.Write(lstTorso.Items.Count);
            for (int T = 0; T < lstTorso.Items.Count; T++)
                BW.Write((string)lstTorso.Items[T]);

            BW.Write(lstLeftArm.Items.Count);
            for (int A = 0; A < lstLeftArm.Items.Count; A++)
                BW.Write((string)lstLeftArm.Items[A]);

            BW.Write(lstRightArm.Items.Count);
            for (int A = 0; A < lstRightArm.Items.Count; A++)
                BW.Write((string)lstRightArm.Items[A]);

            BW.Write(lstLegs.Items.Count);
            for (int L = 0; L < lstLegs.Items.Count; L++)
                BW.Write((string)lstLegs.Items[L]);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Unit.</param>
        private void LoadUnit(string PartPath)
        {
            string Name = PartPath.Substring(0, PartPath.Length - 4).Substring(22);
            UnitModular LoadedUnit = new UnitModular(Name, null);

            //Pilot whitelist.
            for (int P = 0; P < LoadedUnit.ListCharacterIDWhitelist.Count; P++)
            {
                lstPilots.Items.Add(LoadedUnit.ListCharacterIDWhitelist[P]);
            }

            #region Load Parts Unit

            for (int Head = 0; Head < LoadedUnit.Parts.ListHead.Count; Head++)
            {
                lstHead.Items.Add(LoadedUnit.Parts.ListHead[Head]);
            }

            for (int Torso = 0; Torso < LoadedUnit.Parts.ListTorso.Count; Torso++)
            {
                lstTorso.Items.Add(LoadedUnit.Parts.ListTorso[Torso]);
            }

            for (int LeftArm = 0; LeftArm < LoadedUnit.Parts.ListLeftArm.Count; LeftArm++)
            {
                lstLeftArm.Items.Add(LoadedUnit.Parts.ListLeftArm[LeftArm]);
            }

            for (int RightArm = 0; RightArm < LoadedUnit.Parts.ListRightArm.Count; RightArm++)
            {
                lstRightArm.Items.Add(LoadedUnit.Parts.ListRightArm[RightArm]);
            }

            for (int Legs = 0; Legs < LoadedUnit.Parts.ListLegs.Count; Legs++)
            {
                lstLegs.Items.Add(LoadedUnit.Parts.ListLegs[Legs]);
            }

            #endregion

            this.Text = LoadedUnit.RelativePath + " - Project Eternity Unit Editor";

            txtName.Text = LoadedUnit.RelativePath;
            txtPrice.Text = LoadedUnit.Price.ToString();
            txtDescription.Text = LoadedUnit.Description;
        }

        private void ResetControls()
        {
            FilePath = null;

            txtName.Text = "";
            txtPrice.Text = "0";
            txtDescription.Text = "";

            lblBaseHPBoost.Text = "0";
            lblBaseENBoost.Text = "0";
            lblBaseArmorBoost.Text = "0";
            lblBaseMobilityBoost.Text = "0";
            lblBaseMovementBoost.Text = "0";

            lblMELBoost.Text = "0";
            lblRNGBoost.Text = "0";
            lblDEFBoost.Text = "0";
            lblEVABoost.Text = "0";
            lblHITBoost.Text = "0";
            
            lstPilots.Items.Clear();

            tabParts.SelectedIndex = 0;

            ListHead.Clear();
            ListTorso.Clear();
            ListLeftArm.Clear();
            ListRightArm.Clear();
            ListLegs.Clear();

            lstHead.Items.Clear();
            lstTorso.Items.Clear();
            lstLeftArm.Items.Clear();
            lstRightArm.Items.Clear();
            lstLegs.Items.Clear();

            lstHead.Items.Clear();
            lstTorso.Items.Clear();
            lstLeftArm.Items.Clear();
            lstRightArm.Items.Clear();
            lstLegs.Items.Clear();
        }

        private void txtNumberOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtNumberNegativeOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' && ((TextBox)sender).SelectionStart != 0)
                e.Handled = true;
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;
        }

        private void UpdatePartBoosts()
        {
            Int32 BaseHP = 0;
            Int32 BaseEN = 0;
            Int32 BaseArmor = 0;
            Int32 BaseMobility = 0;
            float BaseMovement = 0;

            Int32 MEL = 0;
            Int32 RNG = 0;
            Int32 DEF = 0;
            Int32 EVA = 0;
            Int32 HIT = 0;

            #region Head

            if (ListHead.Count > 0 && lstHead.SelectedIndex >= 0)
            {
                PartHead ActiveHead = ListHead[lstHead.SelectedIndex];

                BaseHP +=       ActiveHead.HP;
                BaseEN +=       ActiveHead.EN;
                BaseArmor +=    ActiveHead.Armor;
                BaseMobility += ActiveHead.Mobility;
                BaseMovement += ActiveHead.Movement;

                MEL += ActiveHead.MEL;
                RNG += ActiveHead.RNG;
                DEF += ActiveHead.DEF;
                EVA += ActiveHead.EVA;
                HIT += ActiveHead.HIT;

                #region Antena

                if (ActiveHead.ListPartAntena.Count > 0)
                {
                    Part ActiveAntena = new Part(ListHead[lstHead.SelectedIndex].ListPartAntena[0], PartTypes.HeadAntena);

                    BaseHP +=       ActiveAntena.BaseHP;
                    BaseEN +=       ActiveAntena.BaseEN;
                    BaseArmor +=    ActiveAntena.BaseArmor;
                    BaseMobility += ActiveAntena.BaseMobility;
                    BaseMovement += ActiveAntena.BaseMovement;

                    MEL += ActiveAntena.MEL;
                    RNG += ActiveAntena.RNG;
                    DEF += ActiveAntena.DEF;
                    EVA += ActiveAntena.EVA;
                    HIT += ActiveAntena.HIT;
                }

                #endregion

                #region Ears

                if (ActiveHead.ListPartEars.Count > 0)
                {
                    Part ActiveEars = new Part(ListHead[lstHead.SelectedIndex].ListPartEars[0], PartTypes.HeadEars);

                    BaseHP +=       ActiveEars.BaseHP;
                    BaseEN +=       ActiveEars.BaseEN;
                    BaseArmor +=    ActiveEars.BaseArmor;
                    BaseMobility += ActiveEars.BaseMobility;
                    BaseMovement += ActiveEars.BaseMovement;

                    MEL += ActiveEars.MEL;
                    RNG += ActiveEars.RNG;
                    DEF += ActiveEars.DEF;
                    EVA += ActiveEars.EVA;
                    HIT += ActiveEars.HIT;
                }

                #endregion

                #region Eyes

                if (ActiveHead.ListPartEyes.Count > 0)
                {
                    Part ActiveEyes = new Part(ListHead[lstHead.SelectedIndex].ListPartEyes[0], PartTypes.HeadEyes);

                    BaseHP +=       ActiveEyes.BaseHP;
                    BaseEN +=       ActiveEyes.BaseEN;
                    BaseArmor +=    ActiveEyes.BaseArmor;
                    BaseMobility += ActiveEyes.BaseMobility;
                    BaseMovement += ActiveEyes.BaseMovement;

                    MEL += ActiveEyes.MEL;
                    RNG += ActiveEyes.RNG;
                    DEF += ActiveEyes.DEF;
                    EVA += ActiveEyes.EVA;
                    HIT += ActiveEyes.HIT;
                }

                #endregion

                #region CPU

                if (ActiveHead.ListPartCPU.Count > 0)
                {
                    Part ActiveCPU = new Part(ListHead[lstHead.SelectedIndex].ListPartCPU[0], PartTypes.HeadCPU);

                    BaseHP +=       ActiveCPU.BaseHP;
                    BaseEN +=       ActiveCPU.BaseEN;
                    BaseArmor +=    ActiveCPU.BaseArmor;
                    BaseMobility += ActiveCPU.BaseMobility;
                    BaseMovement += ActiveCPU.BaseMovement;

                    MEL += ActiveCPU.MEL;
                    RNG += ActiveCPU.RNG;
                    DEF += ActiveCPU.DEF;
                    EVA += ActiveCPU.EVA;
                    HIT += ActiveCPU.HIT;
                }

                #endregion
            }

            #endregion

            #region Torso

            if (ListTorso.Count > 0 && lstTorso.SelectedIndex >= 0)
            {
                PartTorso ActiveTorso = ListTorso[lstTorso.SelectedIndex];

                BaseHP +=       ActiveTorso.HP;
                BaseEN +=       ActiveTorso.EN;
                BaseArmor +=    ActiveTorso.Armor;
                BaseMobility += ActiveTorso.Mobility;
                BaseMovement += ActiveTorso.Movement;

                MEL += ActiveTorso.MEL;
                RNG += ActiveTorso.RNG;
                DEF += ActiveTorso.DEF;
                EVA += ActiveTorso.EVA;
                HIT += ActiveTorso.HIT;

                #region Core

                if (ActiveTorso.ListPartCore.Count > 0)
                {
                    Part ActiveCore = new Part(ListTorso[lstTorso.SelectedIndex].ListPartCore[0], PartTypes.TorsoCore);

                    BaseHP +=       ActiveCore.BaseHP;
                    BaseEN +=       ActiveCore.BaseEN;
                    BaseArmor +=    ActiveCore.BaseArmor;
                    BaseMobility += ActiveCore.BaseMobility;
                    BaseMovement += ActiveCore.BaseMovement;

                    MEL += ActiveCore.MEL;
                    RNG += ActiveCore.RNG;
                    DEF += ActiveCore.DEF;
                    EVA += ActiveCore.EVA;
                    HIT += ActiveCore.HIT;
                }

                #endregion

                #region Radiator

                if (ActiveTorso.ListPartRadiator.Count > 0)
                {
                    Part ActiveRadiator = new Part(ListTorso[lstTorso.SelectedIndex].ListPartRadiator[0], PartTypes.TorsoRadiator);

                    BaseHP +=       ActiveRadiator.BaseHP;
                    BaseEN +=       ActiveRadiator.BaseEN;
                    BaseArmor +=    ActiveRadiator.BaseArmor;
                    BaseMobility += ActiveRadiator.BaseMobility;
                    BaseMovement += ActiveRadiator.BaseMovement;

                    MEL += ActiveRadiator.MEL;
                    RNG += ActiveRadiator.RNG;
                    DEF += ActiveRadiator.DEF;
                    EVA += ActiveRadiator.EVA;
                    HIT += ActiveRadiator.HIT;
                }

                #endregion

                #region Shell

                if (ActiveTorso.ListPartShell.Count > 0)
                {
                    Part ActiveShell = new Part(ListTorso[lstTorso.SelectedIndex].ListPartShell[0], PartTypes.Shell);

                    BaseHP +=       ActiveShell.BaseHP;
                    BaseEN +=       ActiveShell.BaseEN;
                    BaseArmor +=    ActiveShell.BaseArmor;
                    BaseMobility += ActiveShell.BaseMobility;
                    BaseMovement += ActiveShell.BaseMovement;

                    MEL += ActiveShell.MEL;
                    RNG += ActiveShell.RNG;
                    DEF += ActiveShell.DEF;
                    EVA += ActiveShell.EVA;
                    HIT += ActiveShell.HIT;
                }

                #endregion
            }

            #endregion

            #region Left arm

            if (ListLeftArm.Count > 0 && lstLeftArm.SelectedIndex >= 0)
            {
                PartArm ActiveLeftArm = ListLeftArm[lstLeftArm.SelectedIndex];

                BaseHP +=       ActiveLeftArm.HP;
                BaseEN +=       ActiveLeftArm.EN;
                BaseArmor +=    ActiveLeftArm.Armor;
                BaseMobility += ActiveLeftArm.Mobility;
                BaseMovement += ActiveLeftArm.Movement;

                MEL += ActiveLeftArm.MEL;
                RNG += ActiveLeftArm.RNG;
                DEF += ActiveLeftArm.DEF;
                EVA += ActiveLeftArm.EVA;
                HIT += ActiveLeftArm.HIT;

                #region Shell

                if (ActiveLeftArm.ListPartShell.Count > 0)
                {
                    Part ActiveShell = new Part(ListLeftArm[lstLeftArm.SelectedIndex].ListPartShell[0], PartTypes.Shell);

                    BaseHP +=       ActiveShell.BaseHP;
                    BaseEN +=       ActiveShell.BaseEN;
                    BaseArmor +=    ActiveShell.BaseArmor;
                    BaseMobility += ActiveShell.BaseMobility;
                    BaseMovement += ActiveShell.BaseMovement;

                    MEL += ActiveShell.MEL;
                    RNG += ActiveShell.RNG;
                    DEF += ActiveShell.DEF;
                    EVA += ActiveShell.EVA;
                    HIT += ActiveShell.HIT;
                }

                #endregion

                #region Strength

                if (ActiveLeftArm.ListPartStrength.Count > 0)
                {
                    Part ActiveStrength = new Part(ListLeftArm[lstLeftArm.SelectedIndex].ListPartStrength[0], PartTypes.Strength);

                    BaseHP +=       ActiveStrength.BaseHP;
                    BaseEN +=       ActiveStrength.BaseEN;
                    BaseArmor +=    ActiveStrength.BaseArmor;
                    BaseMobility += ActiveStrength.BaseMobility;
                    BaseMovement += ActiveStrength.BaseMovement;

                    MEL += ActiveStrength.MEL;
                    RNG += ActiveStrength.RNG;
                    DEF += ActiveStrength.DEF;
                    EVA += ActiveStrength.EVA;
                    HIT += ActiveStrength.HIT;
                }

                #endregion
            }

            #endregion

            #region Right arm

            if (ListRightArm.Count > 0 && lstRightArm.SelectedIndex >= 0)
            {
                PartArm ActiveRightArm = ListRightArm[lstRightArm.SelectedIndex];

                BaseHP +=       ActiveRightArm.HP;
                BaseEN +=       ActiveRightArm.EN;
                BaseArmor +=    ActiveRightArm.Armor;
                BaseMobility += ActiveRightArm.Mobility;
                BaseMovement += ActiveRightArm.Movement;

                MEL += ActiveRightArm.MEL;
                RNG += ActiveRightArm.RNG;
                DEF += ActiveRightArm.DEF;
                EVA += ActiveRightArm.EVA;
                HIT += ActiveRightArm.HIT;

                #region Shell

                if (ActiveRightArm.ListPartShell.Count > 0)
                {
                    Part ActiveShell = new Part(ListRightArm[lstRightArm.SelectedIndex].ListPartShell[0], PartTypes.Shell);

                    BaseHP +=       ActiveShell.BaseHP;
                    BaseEN +=       ActiveShell.BaseEN;
                    BaseArmor +=    ActiveShell.BaseArmor;
                    BaseMobility += ActiveShell.BaseMobility;
                    BaseMovement += ActiveShell.BaseMovement;

                    MEL += ActiveShell.MEL;
                    RNG += ActiveShell.RNG;
                    DEF += ActiveShell.DEF;
                    EVA += ActiveShell.EVA;
                    HIT += ActiveShell.HIT;
                }

                #endregion

                #region Strength

                if (ActiveRightArm.ListPartStrength.Count > 0)
                {
                    Part ActiveStrength = new Part(ListRightArm[lstRightArm.SelectedIndex].ListPartStrength[0], PartTypes.Strength);

                    BaseHP +=       ActiveStrength.BaseHP;
                    BaseEN +=       ActiveStrength.BaseEN;
                    BaseArmor +=    ActiveStrength.BaseArmor;
                    BaseMobility += ActiveStrength.BaseMobility;
                    BaseMovement += ActiveStrength.BaseMovement;

                    MEL += ActiveStrength.MEL;
                    RNG += ActiveStrength.RNG;
                    DEF += ActiveStrength.DEF;
                    EVA += ActiveStrength.EVA;
                    HIT += ActiveStrength.HIT;
                }

                #endregion
            }

            #endregion

            #region Legs

            if (ListLegs.Count > 0 && lstLegs.SelectedIndex >= 0)
            {
                PartLegs ActiveLegs = ListLegs[lstLegs.SelectedIndex];

                BaseHP +=       ActiveLegs.HP;
                BaseEN +=       ActiveLegs.EN;
                BaseArmor +=    ActiveLegs.Armor;
                BaseMobility += ActiveLegs.Mobility;
                BaseMovement += ActiveLegs.Movement;

                MEL += ActiveLegs.MEL;
                RNG += ActiveLegs.RNG;
                DEF += ActiveLegs.DEF;
                EVA += ActiveLegs.EVA;
                HIT += ActiveLegs.HIT;

                #region Shell

                if (ActiveLegs.ListPartShell.Count > 0)
                {
                    Part ActiveShell = new Part(ListLegs[lstLegs.SelectedIndex].ListPartShell[0], PartTypes.Shell);

                    BaseHP +=       ActiveShell.BaseHP;
                    BaseEN +=       ActiveShell.BaseEN;
                    BaseArmor +=    ActiveShell.BaseArmor;
                    BaseMobility += ActiveShell.BaseMobility;
                    BaseMovement += ActiveShell.BaseMovement;

                    MEL += ActiveShell.MEL;
                    RNG += ActiveShell.RNG;
                    DEF += ActiveShell.DEF;
                    EVA += ActiveShell.EVA;
                    HIT += ActiveShell.HIT;
                }

                #endregion

                #region Strength

                if (ActiveLegs.ListPartStrength.Count > 0)
                {
                    Part ActiveStrength = new Part(ListLegs[lstLegs.SelectedIndex].ListPartStrength[0], PartTypes.Strength);

                    BaseHP += ActiveStrength.BaseHP;
                    BaseEN += ActiveStrength.BaseEN;
                    BaseArmor += ActiveStrength.BaseArmor;
                    BaseMobility += ActiveStrength.BaseMobility;
                    BaseMovement += ActiveStrength.BaseMovement;

                    MEL += ActiveStrength.MEL;
                    RNG += ActiveStrength.RNG;
                    DEF += ActiveStrength.DEF;
                    EVA += ActiveStrength.EVA;
                    HIT += ActiveStrength.HIT;
                }

                #endregion
            }

            #endregion

            #region Label text

            #region Unit boosts

            lblBaseHPBoost.Text = BaseHP.ToString();
            if (BaseHP > 0)
                lblBaseHPBoost.ForeColor = Color.Green;
            else if (BaseHP < 0)
                lblBaseHPBoost.ForeColor = Color.Red;
            else
                lblBaseHPBoost.ForeColor = Color.Black;

            lblBaseENBoost.Text = BaseEN.ToString();
            if (BaseEN > 0)
                lblBaseENBoost.ForeColor = Color.Green;
            else if (BaseEN < 0)
                lblBaseENBoost.ForeColor = Color.Red;
            else
                lblBaseENBoost.ForeColor = Color.Black;

            lblBaseArmorBoost.Text = BaseArmor.ToString();
            if (BaseArmor > 0)
                lblBaseArmorBoost.ForeColor = Color.Green;
            else if (BaseArmor < 0)
                lblBaseArmorBoost.ForeColor = Color.Red;
            else
                lblBaseArmorBoost.ForeColor = Color.Black;

            lblBaseMobilityBoost.Text = BaseMobility.ToString();
            if (BaseMobility > 0)
                lblBaseMobilityBoost.ForeColor = Color.Green;
            else if (BaseMobility < 0)
                lblBaseMobilityBoost.ForeColor = Color.Red;
            else
                lblBaseMobilityBoost.ForeColor = Color.Black;

            lblBaseMovementBoost.Text = BaseMovement.ToString();
            if (BaseMovement > 0)
                lblBaseMovementBoost.ForeColor = Color.Green;
            else if (BaseMovement < 0)
                lblBaseMovementBoost.ForeColor = Color.Red;
            else
                lblBaseMovementBoost.ForeColor = Color.Black;

            #endregion

            #region Unit's Pilot stats boosts

            lblMELBoost.Text = MEL.ToString();
            if (MEL > 0)
                lblMELBoost.ForeColor = Color.Green;
            else if (MEL < 0)
                lblMELBoost.ForeColor = Color.Red;
            else
                lblMELBoost.ForeColor = Color.Black;

            lblRNGBoost.Text = RNG.ToString();
            if (RNG > 0)
                lblRNGBoost.ForeColor = Color.Green;
            else if (RNG < 0)
                lblRNGBoost.ForeColor = Color.Red;
            else
                lblRNGBoost.ForeColor = Color.Black;

            lblDEFBoost.Text = DEF.ToString();
            if (DEF > 0)
                lblDEFBoost.ForeColor = Color.Green;
            else if (DEF < 0)
                lblDEFBoost.ForeColor = Color.Red;
            else
                lblDEFBoost.ForeColor = Color.Black;

            lblEVABoost.Text = EVA.ToString();
            if (EVA > 0)
                lblEVABoost.ForeColor = Color.Green;
            else if (EVA < 0)
                lblEVABoost.ForeColor = Color.Red;
            else
                lblEVABoost.ForeColor = Color.Black;

            lblHITBoost.Text = HIT.ToString();
            if (HIT > 0)
                lblHITBoost.ForeColor = Color.Green;
            else if (HIT < 0)
                lblHITBoost.ForeColor = Color.Red;
            else
                lblHITBoost.ForeColor = Color.Black;

            #endregion

            #endregion
        }

        #region Parts

        #region Head

        private void btnAddHead_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Head;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Head"));
        }

        private void btnRemoveHead_Click(object sender, EventArgs e)
        {
            if (lstHead.SelectedIndex >= 0)
            {
                int Index = lstHead.SelectedIndex;

                ListHead.RemoveAt(lstHead.SelectedIndex);
                lstHead.Items.RemoveAt(lstHead.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstHead.Items.Count > 0)
                    lstHead.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpHead_Click(object sender, EventArgs e)
        {
            if (lstHead.SelectedIndex > 0)
            {
                PartHead HeadPath = ListHead[lstHead.SelectedIndex];
                string Head = (string)lstHead.Items[lstHead.SelectedIndex];
                int Index = lstHead.SelectedIndex - 1;

                ListHead.RemoveAt(lstHead.SelectedIndex);
                lstHead.Items.RemoveAt(lstHead.SelectedIndex);
                //Selected Index is now -1.
                ListHead.Insert(Index, HeadPath);
                lstHead.Items.Insert(Index, Head);

                lstHead.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownHead_Click(object sender, EventArgs e)
        {
            if (lstHead.SelectedIndex >= 0 && lstHead.SelectedIndex < lstHead.Items.Count - 1)
            {
                PartHead HeadPath = ListHead[lstHead.SelectedIndex];
                string Head = (string)lstHead.Items[lstHead.SelectedIndex];
                int Index = lstHead.SelectedIndex + 1;

                ListHead.RemoveAt(lstHead.SelectedIndex);
                lstHead.Items.RemoveAt(lstHead.SelectedIndex);
                //Selected Index is now -1.
                ListHead.Insert(Index, HeadPath);
                lstHead.Items.Insert(Index, Head);

                lstHead.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Torso

        private void btnAddTorso_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Torso;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Torso"));
        }

        private void btnRemoveTorso_Click(object sender, EventArgs e)
        {
            if (lstTorso.SelectedIndex >= 0)
            {
                int Index = lstTorso.SelectedIndex;

                ListTorso.RemoveAt(lstTorso.SelectedIndex);
                lstTorso.Items.RemoveAt(lstTorso.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstTorso.Items.Count > 0)
                    lstTorso.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpTorso_Click(object sender, EventArgs e)
        {
            if (lstTorso.SelectedIndex > 0)
            {
                PartTorso TorsoPath = ListTorso[lstTorso.SelectedIndex];
                string Torso = (string)lstTorso.Items[lstTorso.SelectedIndex];
                int Index = lstTorso.SelectedIndex - 1;

                ListTorso.RemoveAt(lstTorso.SelectedIndex);
                lstTorso.Items.RemoveAt(lstTorso.SelectedIndex);
                //Selected Index is now -1.
                ListTorso.Insert(Index, TorsoPath);
                lstTorso.Items.Insert(Index, Torso);

                lstTorso.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownTorso_Click(object sender, EventArgs e)
        {
            if (lstTorso.SelectedIndex >= 0 && lstTorso.SelectedIndex < lstTorso.Items.Count - 1)
            {
                PartTorso TorsoPath = ListTorso[lstTorso.SelectedIndex];
                string Torso = (string)lstTorso.Items[lstTorso.SelectedIndex];
                int Index = lstTorso.SelectedIndex + 1;

                ListTorso.RemoveAt(lstTorso.SelectedIndex);
                lstTorso.Items.RemoveAt(lstTorso.SelectedIndex);
                //Selected Index is now -1.
                ListTorso.Insert(Index, TorsoPath);
                lstTorso.Items.Insert(Index, Torso);

                lstTorso.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Left arm

        private void btnAddLeftArm_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.LeftArm;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Arm"));
        }

        private void btnRemoveLeftArm_Click(object sender, EventArgs e)
        {
            if (lstLeftArm.SelectedIndex >= 0)
            {
                int Index = lstLeftArm.SelectedIndex;

                ListLeftArm.RemoveAt(lstLeftArm.SelectedIndex);
                lstLeftArm.Items.RemoveAt(lstLeftArm.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstLeftArm.Items.Count > 0)
                    lstLeftArm.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpLeftArm_Click(object sender, EventArgs e)
        {
            if (lstLeftArm.SelectedIndex > 0)
            {
                PartArm LeftArmPath = ListLeftArm[lstLeftArm.SelectedIndex];
                string LeftArm = (string)lstLeftArm.Items[lstLeftArm.SelectedIndex];
                int Index = lstLeftArm.SelectedIndex - 1;

                ListLeftArm.RemoveAt(lstLeftArm.SelectedIndex);
                lstLeftArm.Items.RemoveAt(lstLeftArm.SelectedIndex);
                //Selected Index is now -1.
                ListLeftArm.Insert(Index, LeftArmPath);
                lstLeftArm.Items.Insert(Index, LeftArm);

                lstLeftArm.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownLeftArm_Click(object sender, EventArgs e)
        {
            if (lstLeftArm.SelectedIndex >= 0 && lstLeftArm.SelectedIndex < lstLeftArm.Items.Count - 1)
            {
                PartArm LeftArmPath = ListLeftArm[lstLeftArm.SelectedIndex];
                string LeftArm = (string)lstLeftArm.Items[lstLeftArm.SelectedIndex];
                int Index = lstLeftArm.SelectedIndex + 1;

                ListLeftArm.RemoveAt(lstLeftArm.SelectedIndex);
                lstLeftArm.Items.RemoveAt(lstLeftArm.SelectedIndex);
                //Selected Index is now -1.
                ListLeftArm.Insert(Index, LeftArmPath);
                lstLeftArm.Items.Insert(Index, LeftArm);

                lstLeftArm.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Right arm

        private void btnAddRightArm_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.RightArm;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Arm"));
        }

        private void btnRemoveRightArm_Click(object sender, EventArgs e)
        {
            if (lstRightArm.SelectedIndex >= 0)
            {
                int Index = lstRightArm.SelectedIndex;

                ListRightArm.RemoveAt(lstRightArm.SelectedIndex);
                lstRightArm.Items.RemoveAt(lstRightArm.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstRightArm.Items.Count > 0)
                    lstRightArm.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpRightArm_Click(object sender, EventArgs e)
        {
            if (lstRightArm.SelectedIndex > 0)
            {
                PartArm RightArmPath = ListRightArm[lstRightArm.SelectedIndex];
                string RightArm = (string)lstRightArm.Items[lstRightArm.SelectedIndex];
                int Index = lstRightArm.SelectedIndex - 1;

                ListRightArm.RemoveAt(lstRightArm.SelectedIndex);
                lstRightArm.Items.RemoveAt(lstRightArm.SelectedIndex);
                //Selected Index is now -1.
                ListRightArm.Insert(Index, RightArmPath);
                lstRightArm.Items.Insert(Index, RightArm);

                lstRightArm.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownRightArm_Click(object sender, EventArgs e)
        {
            if (lstRightArm.SelectedIndex >= 0 && lstRightArm.SelectedIndex < lstRightArm.Items.Count - 1)
            {
                PartArm RightArmPath = ListRightArm[lstRightArm.SelectedIndex];
                string RightArm = (string)lstRightArm.Items[lstRightArm.SelectedIndex];
                int Index = lstRightArm.SelectedIndex + 1;

                ListRightArm.RemoveAt(lstRightArm.SelectedIndex);
                lstRightArm.Items.RemoveAt(lstRightArm.SelectedIndex);
                //Selected Index is now -1.
                ListRightArm.Insert(Index, RightArmPath);
                lstRightArm.Items.Insert(Index, RightArm);

                lstRightArm.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Legs

        private void btnAddLegs_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Legs;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Legs"));
        }

        private void btnRemoveLegs_Click(object sender, EventArgs e)
        {
            if (lstLegs.SelectedIndex >= 0)
            {
                int Index = lstLegs.SelectedIndex;

                ListLegs.RemoveAt(lstLegs.SelectedIndex);
                lstLegs.Items.RemoveAt(lstLegs.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstLegs.Items.Count > 0)
                    lstLegs.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpLegs_Click(object sender, EventArgs e)
        {
            if (lstLegs.SelectedIndex > 0)
            {
                PartLegs LegsPath = ListLegs[lstLegs.SelectedIndex];
                string Legs = (string)lstLegs.Items[lstLegs.SelectedIndex];
                int Index = lstLegs.SelectedIndex - 1;

                ListLegs.RemoveAt(lstLegs.SelectedIndex);
                lstLegs.Items.RemoveAt(lstLegs.SelectedIndex);
                //Selected Index is now -1.
                ListLegs.Insert(Index, LegsPath);
                lstLegs.Items.Insert(Index, Legs);

                lstLegs.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownLegs_Click(object sender, EventArgs e)
        {
            if (lstLegs.SelectedIndex >= 0 && lstLegs.SelectedIndex < lstLegs.Items.Count - 1)
            {
                PartLegs LegsPath = ListLegs[lstLegs.SelectedIndex];
                string Legs = (string)lstLegs.Items[lstLegs.SelectedIndex];
                int Index = lstLegs.SelectedIndex + 1;

                ListLegs.RemoveAt(lstLegs.SelectedIndex);
                lstLegs.Items.RemoveAt(lstLegs.SelectedIndex);
                //Selected Index is now -1.
                ListLegs.Insert(Index, LegsPath);
                lstLegs.Items.Insert(Index, Legs);

                lstLegs.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #endregion

        private void cbUseSeries_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void btnAddPilot_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Pilot;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        private void btnRemovePilot_Click(object sender, EventArgs e)
        {
            int Index = lstPilots.SelectedIndex;
            lstPilots.Items.RemoveAt(Index);
            ListPilot.RemoveAt(Index);

            //SelectedIndex = -1 at this point
            if (Index < lstPilots.Items.Count)
                lstPilots.SelectedIndex = Index;
            else if (lstPilots.Items.Count > 0)
                lstPilots.SelectedIndex = Index - 1;
        }

        private void tsmNew_Click(object sender, EventArgs e)
        {
            ResetControls();
            this.Name = "New unit - Project Eternity Unit Editor";
        }

        private void tsmOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Project Eternity Unit (*.peu)|*.peu|All files (*.*)|*.*";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                ResetControls();
                FilePath = OFD.FileName;
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            if (FilePath == null)
                tsmSaveAs_Click(sender, e);
            else
                SaveItem(FilePath, txtName.Text);
        }

        private void tsmSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Project Eternity Unit (*.peu)|*.peu|All files (*.*)|*.*";
            SFD.FileName = txtName.Text;
            if (SFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = SFD.FileName;

                SaveItem(FilePath, txtName.Text);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Pilot:
                        Character NewPilot = new Character(Items[I], null, DicRequirement, DicEffect);
                        if (NewPilot != null)
                        {
                            if (lstPilots.Items.Contains(NewPilot.Name))
                            {
                                MessageBox.Show("This pilot is already listed.\r\n" + NewPilot.Name);
                                return;
                            }
                            ListPilot.Add(NewPilot);
                            lstPilots.Items.Add(Items[I]);
                        }
                        break;

                    case ItemSelectionChoices.Head:
                        string HeadName = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                        PartHead NewHead = new PartHead(HeadName);
                        if (lstHead.Items.Contains(NewHead.RelativePath))
                        {
                            MessageBox.Show("This head is already listed.\r\n" + NewHead.RelativePath);
                            return;
                        }
                        ListHead.Add(NewHead);
                        lstHead.Items.Add(HeadName);
                        break;

                    case ItemSelectionChoices.Torso:
                        string TorsoName = Items[I].Substring(0, Items[I].Length - 5).Substring(28);
                        PartTorso NewTorso = new PartTorso(TorsoName);
                        if (NewTorso != null)
                        {
                            if (lstTorso.Items.Contains(NewTorso.RelativePath))
                            {
                                MessageBox.Show("This torso is already listed.\r\n" + NewTorso.RelativePath);
                                return;
                            }
                            ListTorso.Add(NewTorso);
                            lstTorso.Items.Add(TorsoName);
                        }
                        break;

                    case ItemSelectionChoices.LeftArm:
                        string LeftArmName = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                        PartArm NewLeftArm = new PartArm(LeftArmName);
                        if (NewLeftArm != null)
                        {
                            if (lstLeftArm.Items.Contains(NewLeftArm.RelativePath))
                            {
                                MessageBox.Show("This left arm is already listed.\r\n" + NewLeftArm.RelativePath);
                                return;
                            }
                            ListLeftArm.Add(NewLeftArm);
                            lstLeftArm.Items.Add(LeftArmName);
                        }
                        break;

                    case ItemSelectionChoices.RightArm:
                        string RightArmName = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                        PartArm NewRightArm = new PartArm(RightArmName);
                        if (NewRightArm != null)
                        {
                            if (lstRightArm.Items.Contains(NewRightArm.RelativePath))
                            {
                                MessageBox.Show("This right arm is already listed.\r\n" + NewRightArm.RelativePath);
                                return;
                            }
                            ListRightArm.Add(NewRightArm);
                            lstRightArm.Items.Add(RightArmName);
                        }
                        break;

                    case ItemSelectionChoices.Legs:
                        string LegsName = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                        PartLegs NewLegs = new PartLegs(LegsName);
                        if (NewLegs != null)
                        {
                            if (lstLegs.Items.Contains(NewLegs.RelativePath))
                            {
                                MessageBox.Show("This head is already listed.\r\n" + NewLegs.RelativePath);
                                return;
                            }
                            ListLegs.Add(NewLegs);
                            lstLegs.Items.Add(LegsName);
                        }
                        break;
                }
            }
        }

        private void UnitEditor_Shown(object sender, EventArgs e)
        {
            #region Load parts

            for (int P = 0; P < lstPilots.Items.Count; P++)
            {
                string Name = (string)lstPilots.Items[P];
                //Add the pilot.
                ListPilot.Add(new Character(Name, null, DicRequirement, DicEffect));
            }

            for (int Head = 0; Head < lstHead.Items.Count; Head++)
            {
                string Name = (string)lstHead.Items[Head];
                //Add the Head.
                ListHead.Add(new PartHead(Name));
            }

            for (int Torso = 0; Torso < lstTorso.Items.Count; Torso++)
            {
                string Name = (string)lstTorso.Items[Torso];
                //Add the Torso.
                ListTorso.Add(new PartTorso(Name));
            }

            for (int LeftArm = 0; LeftArm < lstLeftArm.Items.Count; LeftArm++)
            {
                string Name = (string)lstLeftArm.Items[LeftArm];
                //Add the Arm.
                ListLeftArm.Add(new PartArm(Name));
            }

            for (int RightArm = 0; RightArm < lstRightArm.Items.Count; RightArm++)
            {
                string Name = (string)lstRightArm.Items[RightArm];
                //Add the Arm.
                ListRightArm.Add(new PartArm(Name));
            }

            for (int Legs = 0; Legs < lstLegs.Items.Count; Legs++)
            {
                string Name = (string)lstLegs.Items[Legs];
                //Add the Legs.
                ListLegs.Add(new PartLegs(Name));
            }

            #endregion

            UpdatePartBoosts();
        }

        private void lstPartUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePartBoosts();
        }
    }
}
