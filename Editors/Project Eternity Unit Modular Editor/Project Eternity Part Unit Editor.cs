using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Modular;

namespace ProjectEternity.Editors.UnitModularEditor
{
    public partial class UnitPartEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Weapon, HeadAntena, HeadEars, HeadEyes, HeadCPU, TorsoCore, TorsoRadiator, TorsoShell, ArmShell, ArmStrength, LegsShell, LegsStrength };

        #region Members declaration

        private ItemSelectionChoices ItemSelectionChoice;

        private List<Part> ListHeadAntena;
        private List<Part> ListHeadEars;
        private List<Part> ListHeadEyes;
        private List<Part> ListHeadCPU;

        private List<Part> ListTorsoCore;
        private List<Part> ListTorsoRadiator;
        private List<Part> ListTorsoShell;

        private List<Part> ListArmShell;
        private List<Part> ListArmStrength;

        private List<Part> ListLegsShell;
        private List<Part> ListLegsStrength;

        private List<PartUnit.WeaponSlot> ListWeaponSlot;

        #endregion

        public UnitPartEditor()
        {
            InitializeComponent();

            ListHeadAntena = new List<Part>();
            ListHeadEars = new List<Part>();
            ListHeadEyes = new List<Part>();
            ListHeadCPU = new List<Part>();

            ListTorsoCore = new List<Part>();
            ListTorsoRadiator = new List<Part>();
            ListTorsoShell = new List<Part>();

            ListArmShell = new List<Part>();
            ListArmStrength = new List<Part>();

            ListLegsShell = new List<Part>();
            ListLegsStrength = new List<Part>();

            ListWeaponSlot = new List<PartUnit.WeaponSlot>();

            //Hide the header
            tabPartSelector.Region = new Region(new RectangleF(
             0,
               21,
                 tabPartSelector.Width,
                    tabPartSelector.Height));
        }

        public UnitPartEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            tabPartSelector.SelectedIndex = (int)Params[0];
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadPartUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { "Units/Modular/Head" }, "Deathmatch/Units/Modular/Head/", new string[] { ".peup" }, typeof(UnitPartEditor), true, new object[] { PartUnitTypes.Head }),
                new EditorInfo(new string[] { "Units/Modular/Torso" }, "Deathmatch/Units/Modular/Torso/", new string[] { ".peup" }, typeof(UnitPartEditor), true, new object[] { PartUnitTypes.Torso }),
                new EditorInfo(new string[] { "Units/Modular/Arm" }, "Deathmatch/Units/Modular/Arm/", new string[] { ".peup" }, typeof(UnitPartEditor), true, new object[] { PartUnitTypes.Arm }),
                new EditorInfo(new string[] { "Units/Modular/Legs" }, "Deathmatch/Units/Modular/Legs/", new string[] { ".peup" }, typeof(UnitPartEditor), true, new object[] { PartUnitTypes.Legs })
            };
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            Int32 BaseHP = Convert.ToInt32(txtBaseHP.Text);
            Int32 BaseEN = Convert.ToInt32(txtBaseEN.Text);
            Int32 BaseArmor = Convert.ToInt32(txtBaseArmor.Text);
            Int32 BaseMobility = Convert.ToInt32(txtBaseMobility.Text);
            float BaseMovement = Convert.ToSingle(txtBaseMovement.Text);

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtDescription.Text);
            BW.Write((int)txtPrice.Value);

            BW.Write((int)txtBaseHP.Value);
            BW.Write((int)txtBaseEN.Value);
            BW.Write((int)txtBaseArmor.Value);
            BW.Write((int)txtBaseMobility.Value);
            BW.Write((float)txtBaseMovement.Value);

            BW.Write(ListWeaponSlot.Count);
            for (int S = 0; S < ListWeaponSlot.Count; S++)
            {
                BW.Write(ListWeaponSlot[S].Count);
                for (int W = 0; W < ListWeaponSlot[S].Count; W++)
                {
                    BW.Write(ListWeaponSlot[S][W]);
                }
            }

            #region Specific part components

            switch ((PartUnitTypes)tabPartSelector.SelectedIndex)
            {
                case PartUnitTypes.Head:

                    BW.Write(lstHeadAntena.Items.Count);
                    for (int A = 0; A < lstHeadAntena.Items.Count; A++)
                    {
                        BW.Write((string)lstHeadAntena.Items[A]);
                    }

                    BW.Write(lstHeadEars.Items.Count);
                    for (int E = 0; E < lstHeadEars.Items.Count; E++)
                    {
                        BW.Write((string)lstHeadEars.Items[E]);
                    }

                    BW.Write(lstHeadEyes.Items.Count);
                    for (int E = 0; E < lstHeadEyes.Items.Count; E++)
                    {
                        BW.Write((string)lstHeadEyes.Items[E]);
                    }

                    BW.Write(lstHeadCPU.Items.Count);
                    for (int C = 0; C < lstHeadCPU.Items.Count; C++)
                    {
                        BW.Write((string)lstHeadCPU.Items[C]);
                    }

                    break;

                case PartUnitTypes.Torso:

                    BW.Write(lstTorsoCore.Items.Count);
                    for (int C = 0; C < lstTorsoCore.Items.Count; C++)
                    {
                        BW.Write((string)lstTorsoCore.Items[C]);
                    }

                    BW.Write(lstTorsoRadiator.Items.Count);
                    for (int R = 0; R < lstTorsoRadiator.Items.Count; R++)
                    {
                        BW.Write((string)lstTorsoRadiator.Items[R]);
                    }

                    BW.Write(lstTorsoShell.Items.Count);
                    for (int S = 0; S < lstTorsoShell.Items.Count; S++)
                    {
                        BW.Write((string)lstTorsoShell.Items[S]);
                    }

                    break;

                case PartUnitTypes.Arm:

                    BW.Write(lstArmShell.Items.Count);
                    for (int S = 0; S < lstArmShell.Items.Count; S++)
                    {
                        BW.Write((string)lstArmShell.Items[S]);
                    }

                    BW.Write(lstArmStrength.Items.Count);
                    for (int S = 0; S < lstArmStrength.Items.Count; S++)
                    {
                        BW.Write((string)lstArmStrength.Items[S]);
                    }

                    break;

                case PartUnitTypes.Legs:

                    BW.Write(lstLegsShell.Items.Count);
                    for (int S = 0; S < lstLegsShell.Items.Count; S++)
                    {
                        BW.Write((string)lstLegsShell.Items[S]);
                    }

                    BW.Write(lstLegsStrength.Items.Count);
                    for (int S = 0; S < lstLegsStrength.Items.Count; S++)
                    {
                        BW.Write((string)lstLegsStrength.Items[S]);
                    }

                    break;
            }

            #endregion

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Part Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Part Unit.</param>
        private void LoadPartUnit(string PartPath)
        {
            string Name = PartPath.Substring(0, PartPath.Length - 5).Substring(22);
            PartUnit LoadedPart = null;
            switch ((PartUnitTypes)tabPartSelector.SelectedIndex)
            {
                #region Head

                case PartUnitTypes.Head:
                    PartHead Head = new PartHead(Name.Substring(5));
                    LoadedPart = Head;
                    tabPartSelector.SelectedIndex = 0;

                    for (int Antena = 0; Antena < Head.ListPartAntena.Count; Antena++)
                    {
                        lstHeadAntena.Items.Add(Head.ListPartAntena[Antena]);
                    }

                    for (int Ears = 0; Ears < Head.ListPartEars.Count; Ears++)
                    {
                        lstHeadEars.Items.Add(Head.ListPartEars[Ears]);
                    }

                    for (int Eyes = 0; Eyes < Head.ListPartEyes.Count; Eyes++)
                    {
                        lstHeadEyes.Items.Add(Head.ListPartEyes[Eyes]);
                    }

                    for (int CPU = 0; CPU < Head.ListPartCPU.Count; CPU++)
                    {
                        lstHeadCPU.Items.Add(Head.ListPartCPU[CPU]);
                    }

                    break;

                #endregion

                #region Torso

                case PartUnitTypes.Torso:
                    PartTorso Torso = new PartTorso(Name.Substring(6));
                    LoadedPart = Torso;
                    tabPartSelector.SelectedIndex = 1;

                    for (int Core = 0; Core < Torso.ListPartCore.Count; Core++)
                    {
                        lstTorsoCore.Items.Add(Torso.ListPartCore[Core]);
                    }

                    for (int Radiator = 0; Radiator < Torso.ListPartRadiator.Count; Radiator++)
                    {
                        lstTorsoRadiator.Items.Add(Torso.ListPartRadiator[Radiator]);
                    }

                    for (int Shell = 0; Shell < Torso.ListPartShell.Count; Shell++)
                    {
                        lstTorsoShell.Items.Add(Torso.ListPartShell[Shell]);
                    }

                    break;

                #endregion

                #region Arm

                case PartUnitTypes.Arm:
                    PartArm Arm = new PartArm(Name.Substring(4));
                    LoadedPart = Arm;
                    tabPartSelector.SelectedIndex = 2;

                    for (int Shell = 0; Shell < Arm.ListPartShell.Count; Shell++)
                    {
                        lstArmShell.Items.Add(Arm.ListPartShell[Shell]);
                    }

                    for (int Strength = 0; Strength < Arm.ListPartStrength.Count; Strength++)
                    {
                        lstArmStrength.Items.Add(Arm.ListPartStrength[Strength]);
                    }

                    break;

                #endregion

                #region Legs

                case PartUnitTypes.Legs:
                    PartLegs Legs = new PartLegs(Name.Substring(5));
                    LoadedPart = Legs;
                    tabPartSelector.SelectedIndex = 3;

                    for (int Shell = 0; Shell < Legs.ListPartShell.Count; Shell++)
                    {
                        lstLegsShell.Items.Add(Legs.ListPartShell[Shell]);
                    }

                    for (int Strength = 0; Strength < Legs.ListPartStrength.Count; Strength++)
                    {
                        lstLegsStrength.Items.Add(Legs.ListPartStrength[Strength]);
                    }

                    break;

                #endregion
            }

            this.Text = LoadedPart.RelativePath + " - Project Eternity Part Editor";

            txtPrice.Text = LoadedPart.Price.ToString();
            txtDescription.Text = LoadedPart.Description;

            txtBaseHP.Text = LoadedPart.HP.ToString();
            txtBaseEN.Text = LoadedPart.EN.ToString();
            txtBaseArmor.Text = LoadedPart.Armor.ToString();
            txtBaseMobility.Text = LoadedPart.Mobility.ToString();
            txtBaseMovement.Text = LoadedPart.Movement.ToString();

            for (int S = 0; S < LoadedPart.ListWeaponSlot.Count; S++)
            {
                PartUnit.WeaponSlot NewWeaponSlot = new PartUnit.WeaponSlot(LoadedPart.ListWeaponSlot[S].Count);

                for (int W = 0; W < LoadedPart.ListWeaponSlot[S].Count; W++)
                {
                    NewWeaponSlot.ListWeaponSlot.Add(LoadedPart.ListWeaponSlot[S][W]);
                }

                ListWeaponSlot.Add(NewWeaponSlot);
                lstWeaponSlots.Items.Add("Weapon Slot " + (lstWeaponSlots.Items.Count + 1));
            }
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

            if (ListHeadAntena.Count > 0)
            {
                BaseHP += ListHeadAntena[0].BaseHP;
                BaseEN += ListHeadAntena[0].BaseEN;
                BaseArmor += ListHeadAntena[0].BaseArmor;
                BaseMobility += ListHeadAntena[0].BaseMobility;
                BaseMovement += ListHeadAntena[0].BaseMovement;

                MEL += ListHeadAntena[0].MEL;
                RNG += ListHeadAntena[0].RNG;
                DEF += ListHeadAntena[0].DEF;
                EVA += ListHeadAntena[0].EVA;
                HIT += ListHeadAntena[0].HIT;
            }
            if (ListHeadEars.Count > 0)
            {
                BaseHP += ListHeadEars[0].BaseHP;
                BaseEN += ListHeadEars[0].BaseEN;
                BaseArmor += ListHeadEars[0].BaseArmor;
                BaseMobility += ListHeadEars[0].BaseMobility;
                BaseMovement += ListHeadEars[0].BaseMovement;

                MEL += ListHeadEars[0].MEL;
                RNG += ListHeadEars[0].RNG;
                DEF += ListHeadEars[0].DEF;
                EVA += ListHeadEars[0].EVA;
                HIT += ListHeadEars[0].HIT;
            }
            if (ListHeadEyes.Count > 0)
            {
                BaseHP += ListHeadEyes[0].BaseHP;
                BaseEN += ListHeadEyes[0].BaseEN;
                BaseArmor += ListHeadEyes[0].BaseArmor;
                BaseMobility += ListHeadEyes[0].BaseMobility;
                BaseMovement += ListHeadEyes[0].BaseMovement;

                MEL += ListHeadEyes[0].MEL;
                RNG += ListHeadEyes[0].RNG;
                DEF += ListHeadEyes[0].DEF;
                EVA += ListHeadEyes[0].EVA;
                HIT += ListHeadEyes[0].HIT;
            }
            if (ListHeadCPU.Count > 0)
            {
                BaseHP += ListHeadCPU[0].BaseHP;
                BaseEN += ListHeadCPU[0].BaseEN;
                BaseArmor += ListHeadCPU[0].BaseArmor;
                BaseMobility += ListHeadCPU[0].BaseMobility;
                BaseMovement += ListHeadCPU[0].BaseMovement;

                MEL += ListHeadCPU[0].MEL;
                RNG += ListHeadCPU[0].RNG;
                DEF += ListHeadCPU[0].DEF;
                EVA += ListHeadCPU[0].EVA;
                HIT += ListHeadCPU[0].HIT;
            }

            #endregion

            #region Torso

            if (ListTorsoCore.Count > 0)
            {
                BaseHP += ListTorsoCore[0].BaseHP;
                BaseEN += ListTorsoCore[0].BaseEN;
                BaseArmor += ListTorsoCore[0].BaseArmor;
                BaseMobility += ListTorsoCore[0].BaseMobility;
                BaseMovement += ListTorsoCore[0].BaseMovement;

                MEL += ListTorsoCore[0].MEL;
                RNG += ListTorsoCore[0].RNG;
                DEF += ListTorsoCore[0].DEF;
                EVA += ListTorsoCore[0].EVA;
                HIT += ListTorsoCore[0].HIT;
            }

            if (ListTorsoRadiator.Count > 0)
            {
                BaseHP += ListTorsoRadiator[0].BaseHP;
                BaseEN += ListTorsoRadiator[0].BaseEN;
                BaseArmor += ListTorsoRadiator[0].BaseArmor;
                BaseMobility += ListTorsoRadiator[0].BaseMobility;
                BaseMovement += ListTorsoRadiator[0].BaseMovement;

                MEL += ListTorsoRadiator[0].MEL;
                RNG += ListTorsoRadiator[0].RNG;
                DEF += ListTorsoRadiator[0].DEF;
                EVA += ListTorsoRadiator[0].EVA;
                HIT += ListTorsoRadiator[0].HIT;
            }

            if (ListTorsoShell.Count > 0)
            {
                BaseHP += ListTorsoShell[0].BaseHP;
                BaseEN += ListTorsoShell[0].BaseEN;
                BaseArmor += ListTorsoShell[0].BaseArmor;
                BaseMobility += ListTorsoShell[0].BaseMobility;
                BaseMovement += ListTorsoShell[0].BaseMovement;

                MEL += ListTorsoShell[0].MEL;
                RNG += ListTorsoShell[0].RNG;
                DEF += ListTorsoShell[0].DEF;
                EVA += ListTorsoShell[0].EVA;
                HIT += ListTorsoShell[0].HIT;
            }

            #endregion

            #region Arm

            if (ListArmShell.Count > 0)
            {
                BaseHP += ListArmShell[0].BaseHP;
                BaseEN += ListArmShell[0].BaseEN;
                BaseArmor += ListArmShell[0].BaseArmor;
                BaseMobility += ListArmShell[0].BaseMobility;
                BaseMovement += ListArmShell[0].BaseMovement;

                MEL += ListArmShell[0].MEL;
                RNG += ListArmShell[0].RNG;
                DEF += ListArmShell[0].DEF;
                EVA += ListArmShell[0].EVA;
                HIT += ListArmShell[0].HIT;
            }

            if (ListArmStrength.Count > 0)
            {
                BaseHP += ListArmStrength[0].BaseHP;
                BaseEN += ListArmStrength[0].BaseEN;
                BaseArmor += ListArmStrength[0].BaseArmor;
                BaseMobility += ListArmStrength[0].BaseMobility;
                BaseMovement += ListArmStrength[0].BaseMovement;

                MEL += ListArmStrength[0].MEL;
                RNG += ListArmStrength[0].RNG;
                DEF += ListArmStrength[0].DEF;
                EVA += ListArmStrength[0].EVA;
                HIT += ListArmStrength[0].HIT;
            }

            #endregion

            #region Legs

            if (ListLegsShell.Count > 0)
            {
                BaseHP += ListLegsShell[0].BaseHP;
                BaseEN += ListLegsShell[0].BaseEN;
                BaseArmor += ListLegsShell[0].BaseArmor;
                BaseMobility += ListLegsShell[0].BaseMobility;
                BaseMovement += ListLegsShell[0].BaseMovement;

                MEL += ListLegsShell[0].MEL;
                RNG += ListLegsShell[0].RNG;
                DEF += ListLegsShell[0].DEF;
                EVA += ListLegsShell[0].EVA;
                HIT += ListLegsShell[0].HIT;
            }

            if (ListLegsStrength.Count > 0)
            {
                BaseHP += ListLegsStrength[0].BaseHP;
                BaseEN += ListLegsStrength[0].BaseEN;
                BaseArmor += ListLegsStrength[0].BaseArmor;
                BaseMobility += ListLegsStrength[0].BaseMobility;
                BaseMovement += ListLegsStrength[0].BaseMovement;

                MEL += ListLegsStrength[0].MEL;
                RNG += ListLegsStrength[0].RNG;
                DEF += ListLegsStrength[0].DEF;
                EVA += ListLegsStrength[0].EVA;
                HIT += ListLegsStrength[0].HIT;
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

            #region Pilot boosts

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

        #region Head

        #region Antena

        private void btnAddHeadAntena_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.HeadAntena;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Head/Antena"));
        }

        private void btnRemoveHeadAntena_Click(object sender, EventArgs e)
        {
            if (lstHeadAntena.SelectedIndex >= 0)
            {
                int Index = lstHeadAntena.SelectedIndex;

                ListHeadAntena.RemoveAt(lstHeadAntena.SelectedIndex);
                lstHeadAntena.Items.RemoveAt(lstHeadAntena.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstHeadAntena.Items.Count > 0)
                    lstHeadAntena.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpHeadAntena_Click(object sender, EventArgs e)
        {
            if (lstHeadAntena.SelectedIndex > 0)
            {
                Part Antena = ListHeadAntena[lstHeadAntena.SelectedIndex];
                string AntenaName = (string)lstHeadAntena.Items[lstHeadAntena.SelectedIndex];
                int Index = lstHeadAntena.SelectedIndex - 1;

                ListHeadAntena.RemoveAt(lstHeadAntena.SelectedIndex);
                lstHeadAntena.Items.RemoveAt(lstHeadAntena.SelectedIndex);
                //Selected Index is now -1.
                ListHeadAntena.Insert(Index, Antena);
                lstHeadAntena.Items.Insert(Index, AntenaName);

                lstHeadAntena.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownHeadAntena_Click(object sender, EventArgs e)
        {
            if (lstHeadAntena.SelectedIndex >= 0 && lstHeadAntena.SelectedIndex < lstHeadAntena.Items.Count - 1)
            {
                Part Antena = ListHeadAntena[lstHeadAntena.SelectedIndex];
                string AntenaName = (string)lstHeadAntena.Items[lstHeadAntena.SelectedIndex];
                int Index = lstHeadAntena.SelectedIndex + 1;

                ListHeadAntena.RemoveAt(lstHeadAntena.SelectedIndex);
                lstHeadAntena.Items.RemoveAt(lstHeadAntena.SelectedIndex);
                //Selected Index is now -1.
                ListHeadAntena.Insert(Index, Antena);
                lstHeadAntena.Items.Insert(Index, AntenaName);

                lstHeadAntena.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Ears

        private void btnAddHeadEars_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.HeadEars;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Head/Ears"));
        }

        private void btnRemoveHeadEars_Click(object sender, EventArgs e)
        {
            if (lstHeadEars.SelectedIndex >= 0)
            {
                int Index = lstHeadEars.SelectedIndex;

                ListHeadEars.RemoveAt(lstHeadEars.SelectedIndex);
                lstHeadEars.Items.RemoveAt(lstHeadEars.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstHeadEars.Items.Count > 0)
                    lstHeadEars.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpHeadEars_Click(object sender, EventArgs e)
        {
            if (lstHeadEars.SelectedIndex > 0)
            {
                Part Ears = ListHeadEars[lstHeadEars.SelectedIndex];
                string EarsName = (string)lstHeadEars.Items[lstHeadEars.SelectedIndex];
                int Index = lstHeadEars.SelectedIndex - 1;

                ListHeadEars.RemoveAt(lstHeadEars.SelectedIndex);
                lstHeadEars.Items.RemoveAt(lstHeadEars.SelectedIndex);
                //Selected Index is now -1.
                ListHeadEars.Insert(Index, Ears);
                lstHeadEars.Items.Insert(Index, EarsName);

                lstHeadEars.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownHeadEars_Click(object sender, EventArgs e)
        {
            if (lstHeadEars.SelectedIndex >= 0 && lstHeadEars.SelectedIndex < lstHeadEars.Items.Count - 1)
            {
                Part Ears = ListHeadEars[lstHeadEars.SelectedIndex];
                string EarsName = (string)lstHeadEars.Items[lstHeadEars.SelectedIndex];
                int Index = lstHeadEars.SelectedIndex + 1;

                ListHeadEars.RemoveAt(lstHeadEars.SelectedIndex);
                lstHeadEars.Items.RemoveAt(lstHeadEars.SelectedIndex);
                //Selected Index is now -1.
                ListHeadEars.Insert(Index, Ears);
                lstHeadEars.Items.Insert(Index, EarsName);

                lstHeadEars.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Eyes

        private void btnAddHeadEyes_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.HeadEyes;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Head/Eyes"));
        }

        private void btnRemoveHeadEyes_Click(object sender, EventArgs e)
        {
            if (lstHeadEyes.SelectedIndex >= 0)
            {
                int Index = lstHeadEyes.SelectedIndex;

                ListHeadEyes.RemoveAt(lstHeadEyes.SelectedIndex);
                lstHeadEyes.Items.RemoveAt(lstHeadEyes.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstHeadEyes.Items.Count > 0)
                    lstHeadEyes.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpHeadEyes_Click(object sender, EventArgs e)
        {
            if (lstHeadEyes.SelectedIndex > 0)
            {
                Part Eyes = ListHeadEyes[lstHeadEyes.SelectedIndex];
                string EyesName = (string)lstHeadEyes.Items[lstHeadEyes.SelectedIndex];
                int Index = lstHeadEyes.SelectedIndex - 1;

                ListHeadEyes.RemoveAt(lstHeadEyes.SelectedIndex);
                lstHeadEyes.Items.RemoveAt(lstHeadEyes.SelectedIndex);
                //Selected Index is now -1.
                ListHeadEyes.Insert(Index, Eyes);
                lstHeadEyes.Items.Insert(Index, EyesName);

                lstHeadEyes.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownHeadEyes_Click(object sender, EventArgs e)
        {
            if (lstHeadEyes.SelectedIndex >= 0 && lstHeadEyes.SelectedIndex < lstHeadEyes.Items.Count - 1)
            {
                Part Eyes = ListHeadEyes[lstHeadEyes.SelectedIndex];
                string EyesName = (string)lstHeadEyes.Items[lstHeadEyes.SelectedIndex];
                int Index = lstHeadEyes.SelectedIndex + 1;

                ListHeadEyes.RemoveAt(lstHeadEyes.SelectedIndex);
                lstHeadEyes.Items.RemoveAt(lstHeadEyes.SelectedIndex);
                //Selected Index is now -1.
                ListHeadEyes.Insert(Index, Eyes);
                lstHeadEyes.Items.Insert(Index, EyesName);

                lstHeadEyes.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region CPU

        private void btnAddHeadCPU_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.HeadCPU;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Head/CPU"));
        }

        private void btnRemoveHeadCPU_Click(object sender, EventArgs e)
        {
            if (lstHeadCPU.SelectedIndex >= 0)
            {
                int Index = lstHeadCPU.SelectedIndex;

                ListHeadCPU.RemoveAt(lstHeadCPU.SelectedIndex);
                lstHeadCPU.Items.RemoveAt(lstHeadCPU.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstHeadCPU.Items.Count > 0)
                    lstHeadCPU.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpHeadCPU_Click(object sender, EventArgs e)
        {
            if (lstHeadCPU.SelectedIndex > 0)
            {
                Part CPU = ListHeadCPU[lstHeadCPU.SelectedIndex];
                string CPUName = (string)lstHeadCPU.Items[lstHeadCPU.SelectedIndex];
                int Index = lstHeadCPU.SelectedIndex - 1;

                ListHeadCPU.RemoveAt(lstHeadCPU.SelectedIndex);
                lstHeadCPU.Items.RemoveAt(lstHeadCPU.SelectedIndex);
                //Selected Index is now -1.
                ListHeadCPU.Insert(Index, CPU);
                lstHeadCPU.Items.Insert(Index, CPUName);

                lstHeadCPU.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownHeadCPU_Click(object sender, EventArgs e)
        {
            if (lstHeadCPU.SelectedIndex >= 0 && lstHeadCPU.SelectedIndex < lstHeadCPU.Items.Count - 1)
            {
                Part CPU = ListHeadCPU[lstHeadCPU.SelectedIndex];
                string CPUName = (string)lstHeadCPU.Items[lstHeadCPU.SelectedIndex];
                int Index = lstHeadCPU.SelectedIndex + 1;

                ListHeadCPU.RemoveAt(lstHeadCPU.SelectedIndex);
                lstHeadCPU.Items.RemoveAt(lstHeadCPU.SelectedIndex);
                //Selected Index is now -1.
                ListHeadCPU.Insert(Index, CPU);
                lstHeadCPU.Items.Insert(Index, CPUName);

                lstHeadCPU.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #endregion

        #region Torso

        #region Core

        private void btnAddTorsoCore_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TorsoCore;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Torso/Core"));
        }

        private void btnRemoveTorsoCore_Click(object sender, EventArgs e)
        {
            if (lstTorsoCore.SelectedIndex >= 0)
            {
                int Index = lstTorsoCore.SelectedIndex;

                ListTorsoCore.RemoveAt(lstTorsoCore.SelectedIndex);
                lstTorsoCore.Items.RemoveAt(lstTorsoCore.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstTorsoCore.Items.Count > 0)
                    lstTorsoCore.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpTorsoCore_Click(object sender, EventArgs e)
        {
            if (lstTorsoCore.SelectedIndex > 0)
            {
                Part Core = ListTorsoCore[lstTorsoCore.SelectedIndex];
                string CoreName = (string)lstTorsoCore.Items[lstTorsoCore.SelectedIndex];
                int Index = lstTorsoCore.SelectedIndex - 1;

                ListTorsoCore.RemoveAt(lstTorsoCore.SelectedIndex);
                lstTorsoCore.Items.RemoveAt(lstTorsoCore.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoCore.Insert(Index, Core);
                lstTorsoCore.Items.Insert(Index, CoreName);

                lstTorsoCore.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownTorsoCore_Click(object sender, EventArgs e)
        {
            if (lstTorsoCore.SelectedIndex >= 0 && lstTorsoCore.SelectedIndex < lstTorsoCore.Items.Count - 1)
            {
                Part Core = ListTorsoCore[lstTorsoCore.SelectedIndex];
                string CoreName = (string)lstTorsoCore.Items[lstTorsoCore.SelectedIndex];
                int Index = lstTorsoCore.SelectedIndex + 1;

                ListTorsoCore.RemoveAt(lstTorsoCore.SelectedIndex);
                lstTorsoCore.Items.RemoveAt(lstTorsoCore.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoCore.Insert(Index, Core);
                lstTorsoCore.Items.Insert(Index, CoreName);

                lstTorsoCore.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Radiator

        private void btnAddTorsoRadiator_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TorsoRadiator;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Torso/Radiator"));
        }

        private void btnRemoveTorsoRadiator_Click(object sender, EventArgs e)
        {
            if (lstTorsoRadiator.SelectedIndex >= 0)
            {
                int Index = lstTorsoRadiator.SelectedIndex;

                ListTorsoRadiator.RemoveAt(lstTorsoRadiator.SelectedIndex);
                lstTorsoRadiator.Items.RemoveAt(lstTorsoRadiator.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstTorsoRadiator.Items.Count > 0)
                    lstTorsoRadiator.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpTorsoRadiator_Click(object sender, EventArgs e)
        {
            if (lstTorsoRadiator.SelectedIndex > 0)
            {
                Part Radiator = ListTorsoRadiator[lstTorsoRadiator.SelectedIndex];
                string RadiatorName = (string)lstTorsoRadiator.Items[lstTorsoRadiator.SelectedIndex];
                int Index = lstTorsoRadiator.SelectedIndex - 1;

                ListTorsoRadiator.RemoveAt(lstTorsoRadiator.SelectedIndex);
                lstTorsoRadiator.Items.RemoveAt(lstTorsoRadiator.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoRadiator.Insert(Index, Radiator);
                lstTorsoRadiator.Items.Insert(Index, RadiatorName);

                lstTorsoRadiator.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownTorsoRadiator_Click(object sender, EventArgs e)
        {
            if (lstTorsoRadiator.SelectedIndex >= 0 && lstTorsoRadiator.SelectedIndex < lstTorsoRadiator.Items.Count - 1)
            {
                Part Radiator = ListTorsoRadiator[lstTorsoRadiator.SelectedIndex];
                string RadiatorName = (string)lstTorsoRadiator.Items[lstTorsoRadiator.SelectedIndex];
                int Index = lstTorsoRadiator.SelectedIndex + 1;

                ListTorsoRadiator.RemoveAt(lstTorsoRadiator.SelectedIndex);
                lstTorsoRadiator.Items.RemoveAt(lstTorsoRadiator.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoRadiator.Insert(Index, Radiator);
                lstTorsoRadiator.Items.Insert(Index, RadiatorName);

                lstTorsoRadiator.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Shell

        private void btnAddTorsoShell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TorsoShell;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Torso/Shell"));
        }

        private void btnRemoveTorsoShell_Click(object sender, EventArgs e)
        {
            if (lstTorsoShell.SelectedIndex >= 0)
            {
                int Index = lstTorsoShell.SelectedIndex;

                ListTorsoShell.RemoveAt(lstTorsoShell.SelectedIndex);
                lstTorsoShell.Items.RemoveAt(lstTorsoShell.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstTorsoShell.Items.Count > 0)
                    lstTorsoShell.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpTorsoShell_Click(object sender, EventArgs e)
        {
            if (lstTorsoShell.SelectedIndex > 0)
            {
                Part Shell = ListTorsoShell[lstTorsoShell.SelectedIndex];
                string ShellName = (string)lstTorsoShell.Items[lstTorsoShell.SelectedIndex];
                int Index = lstTorsoShell.SelectedIndex - 1;

                ListTorsoShell.RemoveAt(lstTorsoShell.SelectedIndex);
                lstTorsoShell.Items.RemoveAt(lstTorsoShell.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoShell.Insert(Index, Shell);
                lstTorsoShell.Items.Insert(Index, ShellName);

                lstTorsoShell.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownTorsoShell_Click(object sender, EventArgs e)
        {
            if (lstTorsoShell.SelectedIndex >= 0 && lstTorsoShell.SelectedIndex < lstTorsoShell.Items.Count - 1)
            {
                Part Shell = ListTorsoShell[lstTorsoShell.SelectedIndex];
                string ShellName = (string)lstTorsoShell.Items[lstTorsoShell.SelectedIndex];
                int Index = lstTorsoShell.SelectedIndex + 1;

                ListTorsoShell.RemoveAt(lstTorsoShell.SelectedIndex);
                lstTorsoShell.Items.RemoveAt(lstTorsoShell.SelectedIndex);
                //Selected Index is now -1.
                ListTorsoShell.Insert(Index, Shell);
                lstTorsoShell.Items.Insert(Index, ShellName);

                lstTorsoShell.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #endregion

        #region Arm

        #region Shell

        private void btnAddArmShell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ArmShell;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Arm/Shell"));
        }

        private void btnRemoveArmShell_Click(object sender, EventArgs e)
        {
            if (lstArmShell.SelectedIndex >= 0)
            {
                int Index = lstArmShell.SelectedIndex;

                ListArmShell.RemoveAt(lstArmShell.SelectedIndex);
                lstArmShell.Items.RemoveAt(lstArmShell.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstArmShell.Items.Count > 0)
                    lstArmShell.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpArmShell_Click(object sender, EventArgs e)
        {
            if (lstArmShell.SelectedIndex > 0)
            {
                Part Shell = ListArmShell[lstArmShell.SelectedIndex];
                string ArmShellName = (string)lstArmShell.Items[lstArmShell.SelectedIndex];
                int Index = lstArmShell.SelectedIndex - 1;

                ListArmShell.RemoveAt(lstArmShell.SelectedIndex);
                lstArmShell.Items.RemoveAt(lstArmShell.SelectedIndex);
                //Selected Index is now -1.
                ListArmShell.Insert(Index, Shell);
                lstArmShell.Items.Insert(Index, ArmShellName);

                lstArmShell.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownArmShell_Click(object sender, EventArgs e)
        {
            if (lstArmShell.SelectedIndex >= 0 && lstArmShell.SelectedIndex < lstArmShell.Items.Count - 1)
            {
                Part Shell = ListArmShell[lstArmShell.SelectedIndex];
                string ArmShellName = (string)lstArmShell.Items[lstArmShell.SelectedIndex];
                int Index = lstArmShell.SelectedIndex + 1;

                ListArmShell.RemoveAt(lstArmShell.SelectedIndex);
                lstArmShell.Items.RemoveAt(lstArmShell.SelectedIndex);
                //Selected Index is now -1.
                ListArmShell.Insert(Index, Shell);
                lstArmShell.Items.Insert(Index, ArmShellName);

                lstArmShell.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Strength

        private void btnAddArmStrength_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ArmStrength;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Arm/Strength"));
        }

        private void btnRemoveArmStrength_Click(object sender, EventArgs e)
        {
            if (lstArmStrength.SelectedIndex >= 0)
            {
                int Index = lstArmStrength.SelectedIndex;

                ListArmStrength.RemoveAt(lstArmStrength.SelectedIndex);
                lstArmStrength.Items.RemoveAt(lstArmStrength.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstArmStrength.Items.Count > 0)
                    lstArmStrength.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpArmStrength_Click(object sender, EventArgs e)
        {
            if (lstArmStrength.SelectedIndex > 0)
            {
                Part Strength = ListArmStrength[lstArmStrength.SelectedIndex];
                string StrengthName = (string)lstArmStrength.Items[lstArmStrength.SelectedIndex];
                int Index = lstArmStrength.SelectedIndex - 1;

                ListArmStrength.RemoveAt(lstArmStrength.SelectedIndex);
                lstArmStrength.Items.RemoveAt(lstArmStrength.SelectedIndex);
                //Selected Index is now -1.
                ListArmStrength.Insert(Index, Strength);
                lstArmStrength.Items.Insert(Index, StrengthName);

                lstArmStrength.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownArmStrength_Click(object sender, EventArgs e)
        {
            if (lstArmStrength.SelectedIndex >= 0 && lstArmStrength.SelectedIndex < lstArmStrength.Items.Count - 1)
            {
                Part Strength = ListArmStrength[lstArmStrength.SelectedIndex];
                string StrengthName = (string)lstArmStrength.Items[lstArmStrength.SelectedIndex];
                int Index = lstArmStrength.SelectedIndex + 1;

                ListArmStrength.RemoveAt(lstArmStrength.SelectedIndex);
                lstArmStrength.Items.RemoveAt(lstArmStrength.SelectedIndex);
                //Selected Index is now -1.
                ListArmStrength.Insert(Index, Strength);
                lstArmStrength.Items.Insert(Index, StrengthName);

                lstArmStrength.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #endregion

        #region Legs

        #region Shell

        private void btnAddLegsShell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.LegsShell;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Legs/Shell"));
        }

        private void btnRemoveLegsShell_Click(object sender, EventArgs e)
        {
            if (lstLegsShell.SelectedIndex >= 0)
            {
                int Index = lstLegsShell.SelectedIndex;

                ListLegsShell.RemoveAt(lstLegsShell.SelectedIndex);
                lstLegsShell.Items.RemoveAt(lstLegsShell.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstLegsShell.Items.Count > 0)
                    lstLegsShell.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpLegsShell_Click(object sender, EventArgs e)
        {
            if (lstLegsShell.SelectedIndex > 0)
            {
                Part Shell = ListLegsShell[lstLegsShell.SelectedIndex];
                string ShellName = (string)lstLegsShell.Items[lstLegsShell.SelectedIndex];
                int Index = lstLegsShell.SelectedIndex - 1;

                ListLegsShell.RemoveAt(lstLegsShell.SelectedIndex);
                lstLegsShell.Items.RemoveAt(lstLegsShell.SelectedIndex);
                //Selected Index is now -1.
                ListLegsShell.Insert(Index, Shell);
                lstLegsShell.Items.Insert(Index, ShellName);

                lstLegsShell.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownLegsShell_Click(object sender, EventArgs e)
        {
            if (lstLegsShell.SelectedIndex >= 0 && lstLegsShell.SelectedIndex < lstLegsShell.Items.Count - 1)
            {
                Part Shell = ListLegsShell[lstLegsShell.SelectedIndex];
                string ShellName = (string)lstLegsShell.Items[lstLegsShell.SelectedIndex];
                int Index = lstLegsShell.SelectedIndex + 1;

                ListLegsShell.RemoveAt(lstLegsShell.SelectedIndex);
                lstLegsShell.Items.RemoveAt(lstLegsShell.SelectedIndex);
                //Selected Index is now -1.
                ListLegsShell.Insert(Index, Shell);
                lstLegsShell.Items.Insert(Index, ShellName);

                lstLegsShell.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #region Strength

        private void btnAddLegsStrength_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.LegsStrength;
            ListMenuItemsSelected(ShowContextMenuWithItem("Units/Modular/Legs/Strength"));
        }

        private void btnRemoveLegsStrength_Click(object sender, EventArgs e)
        {
            if (lstLegsStrength.SelectedIndex >= 0)
            {
                int Index = lstLegsStrength.SelectedIndex;

                ListLegsStrength.RemoveAt(lstLegsStrength.SelectedIndex);
                lstLegsStrength.Items.RemoveAt(lstLegsStrength.SelectedIndex);

                if (Index == 0)
                    UpdatePartBoosts();
                else if (lstLegsStrength.Items.Count > 0)
                    lstLegsStrength.SelectedIndex = Index - 1;
            }
        }

        private void btnMoveUpLegsStrength_Click(object sender, EventArgs e)
        {
            if (lstLegsStrength.SelectedIndex > 0)
            {
                Part Strength = ListLegsStrength[lstLegsStrength.SelectedIndex];
                string StrengthName = (string)lstLegsStrength.Items[lstLegsStrength.SelectedIndex];
                int Index = lstLegsStrength.SelectedIndex - 1;

                ListLegsStrength.RemoveAt(lstLegsStrength.SelectedIndex);
                lstLegsStrength.Items.RemoveAt(lstLegsStrength.SelectedIndex);
                //Selected Index is now -1.
                ListLegsStrength.Insert(Index, Strength);
                lstLegsStrength.Items.Insert(Index, StrengthName);

                lstLegsStrength.SelectedIndex = Index;

                //Update the stats if there's a new item at position 0.
                if (Index == 0)
                    UpdatePartBoosts();
            }
        }

        private void btnMoveDownLegsStrength_Click(object sender, EventArgs e)
        {
            if (lstLegsStrength.SelectedIndex >= 0 && lstLegsStrength.SelectedIndex < lstLegsStrength.Items.Count - 1)
            {
                Part Strength = ListLegsStrength[lstLegsStrength.SelectedIndex];
                string StrengthName = (string)lstLegsStrength.Items[lstLegsStrength.SelectedIndex];
                int Index = lstLegsStrength.SelectedIndex + 1;

                ListLegsStrength.RemoveAt(lstLegsStrength.SelectedIndex);
                lstLegsStrength.Items.RemoveAt(lstLegsStrength.SelectedIndex);
                //Selected Index is now -1.
                ListLegsStrength.Insert(Index, Strength);
                lstLegsStrength.Items.Insert(Index, StrengthName);

                lstLegsStrength.SelectedIndex = Index;

                //Update the stats if the item was at position 0.
                if (Index == 1)
                    UpdatePartBoosts();
            }
        }

        #endregion

        #endregion

        #region Weapon

        private void btnAddWeaponSlot_Click(object sender, EventArgs e)
        {
            ListWeaponSlot.Add(new PartUnit.WeaponSlot());
            lstWeaponSlots.Items.Add("Weapon Slot " + (lstWeaponSlots.Items.Count + 1));
        }

        private void btnRemoveWeaponSlot_Click(object sender, EventArgs e)
        {
            if (lstWeaponSlots.SelectedIndex >= 0)
            {
                int Index = lstWeaponSlots.SelectedIndex;

                ListWeaponSlot.RemoveAt(lstWeaponSlots.SelectedIndex);
                lstWeaponSlots.Items.RemoveAt(lstWeaponSlots.SelectedIndex);

                if (lstWeaponSlots.Items.Count > 0)
                    lstWeaponSlots.SelectedIndex = Index - 1;
            }
        }

        private void lstWeaponSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWeaponSlots.SelectedIndex >= 0)
            {
                lstWeapon.Items.Clear();
                for (int W = 0; W < ListWeaponSlot[lstWeaponSlots.SelectedIndex].Count; W++)
                {
                    lstWeapon.Items.Add(ListWeaponSlot[lstWeaponSlots.SelectedIndex].ListWeaponSlot[W]);
                }
            }
        }

        private void btnAddWeapon_Click(object sender, EventArgs e)
        {
            if (lstWeaponSlots.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Weapon;
                ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttacks));
            }
        }

        private void btnRemoveWeapon_Click(object sender, EventArgs e)
        {
            if (lstWeapon.SelectedIndex >= 0 && lstWeaponSlots.SelectedIndex >= 0)
            {
                int Index = lstWeapon.SelectedIndex;

                ListWeaponSlot[lstWeaponSlots.SelectedIndex].ListWeaponSlot.RemoveAt(lstWeaponSlots.SelectedIndex);
                lstWeapon.Items.RemoveAt(lstWeapon.SelectedIndex);

                if (lstWeapon.Items.Count > 0)
                    lstWeapon.SelectedIndex = Index - 1;
            }
        }

        #endregion

        private void UnitPartEditor_Shown(object sender, EventArgs e)
        {
            #region Load parts

            #region Head

            for (int Antena = 0; Antena < lstHeadAntena.Items.Count; Antena++)
            {
                ListHeadAntena.Add(new Part(lstHeadAntena.Items[Antena].ToString(), PartTypes.HeadAntena));
            }

            for (int Ears = 0; Ears < lstHeadEars.Items.Count; Ears++)
            {
                ListHeadEars.Add(new Part(lstHeadEars.Items[Ears].ToString(), PartTypes.HeadEars));
            }

            for (int Eyes = 0; Eyes < lstHeadEyes.Items.Count; Eyes++)
            {
                ListHeadEyes.Add(new Part(lstHeadEyes.Items[Eyes].ToString(), PartTypes.HeadEyes));
            }

            for (int CPU = 0; CPU < lstHeadCPU.Items.Count; CPU++)
            {
                ListHeadCPU.Add(new Part(lstHeadCPU.Items[CPU].ToString(), PartTypes.HeadCPU));
            }

            #endregion

            #region Torso

            for (int Core = 0; Core < lstTorsoCore.Items.Count; Core++)
            {
                ListTorsoCore.Add(new Part(lstTorsoCore.Items[Core].ToString(), PartTypes.TorsoCore));
            }

            for (int Radiator = 0; Radiator < lstTorsoRadiator.Items.Count; Radiator++)
            {
                ListTorsoRadiator.Add(new Part(lstTorsoRadiator.Items[Radiator].ToString(), PartTypes.TorsoRadiator));
            }

            for (int Shell = 0; Shell < lstTorsoShell.Items.Count; Shell++)
            {
                ListTorsoShell.Add(new Part(lstTorsoShell.Items[Shell].ToString(), PartTypes.Shell));
            }

            #endregion

            #region Arm

            for (int Shell = 0; Shell < lstArmShell.Items.Count; Shell++)
            {
                ListArmShell.Add(new Part(lstArmShell.Items[Shell].ToString(), PartTypes.Shell));
            }

            for (int Strength = 0; Strength < lstArmStrength.Items.Count; Strength++)
            {
                ListArmStrength.Add(new Part(lstArmStrength.Items[Strength].ToString(), PartTypes.Strength));
            }

            #endregion

            #region Legs

            for (int Shell = 0; Shell < lstLegsShell.Items.Count; Shell++)
            {
                ListLegsShell.Add(new Part(lstLegsShell.Items[Shell].ToString(), PartTypes.Shell));
            }

            for (int Strength = 0; Strength < lstLegsStrength.Items.Count; Strength++)
            {
                ListLegsStrength.Add(new Part(lstLegsStrength.Items[Strength].ToString(), PartTypes.Strength));
            }

            #endregion

            #endregion

            UpdatePartBoosts();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    #region Weapon

                    case ItemSelectionChoices.Weapon:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Attacks") + 8);
                        if (Name != null)
                        {
                            if (lstWeapon.Items.Contains(Name))
                            {
                                MessageBox.Show("This weapon is already listed.\r\n" + Name);
                                return;
                            }
                            lstWeapon.Items.Add(Name);
                            ListWeaponSlot[lstWeaponSlots.SelectedIndex].ListWeaponSlot.Add(Name);
                        }
                        break;

                    #endregion

                    #region Head

                    case ItemSelectionChoices.HeadAntena:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(34);
                        Part NewHeadAntena = new Part(Name, PartTypes.HeadAntena);
                        if (NewHeadAntena != null)
                        {
                            if (lstHeadAntena.Items.Contains(Name))
                            {
                                MessageBox.Show("This head antena is already listed.\r\n" + Name);
                                return;
                            }
                            ListHeadAntena.Add(NewHeadAntena);
                            lstHeadAntena.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.HeadEars:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(32);
                        Part NewHeadEars = new Part(Name, PartTypes.HeadEars);
                        if (NewHeadEars != null)
                        {
                            if (lstHeadEars.Items.Contains(Name))
                            {
                                MessageBox.Show("This head ears is already listed.\r\n" + Name);
                                return;
                            }
                            ListHeadEars.Add(NewHeadEars);
                            lstHeadEars.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.HeadEyes:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(32);
                        Part NewHeadEyes = new Part(Name, PartTypes.HeadEyes);
                        if (NewHeadEyes != null)
                        {
                            if (lstHeadEyes.Items.Contains(Name))
                            {
                                MessageBox.Show("This head Eyes is already listed.\r\n" + Name);
                                return;
                            }
                            ListHeadEyes.Add(NewHeadEyes);
                            lstHeadEyes.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.HeadCPU:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(31);
                        Part NewHeadCPU = new Part(Name, PartTypes.HeadCPU);
                        if (NewHeadCPU != null)
                        {
                            if (lstHeadCPU.Items.Contains(Name))
                            {
                                MessageBox.Show("This head CPU is already listed.\r\n" + Name);
                                return;
                            }
                            ListHeadCPU.Add(NewHeadCPU);
                            lstHeadCPU.Items.Add(Name);
                        }
                        break;

                    #endregion

                    #region Torso

                    case ItemSelectionChoices.TorsoCore:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(33);
                        Part NewTorsoCore = new Part(Name, PartTypes.TorsoCore);
                        if (NewTorsoCore != null)
                        {
                            if (lstTorsoCore.Items.Contains(Name))
                            {
                                MessageBox.Show("This torso core is already listed.\r\n" + Name);
                                return;
                            }
                            ListTorsoCore.Add(NewTorsoCore);
                            lstTorsoCore.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.TorsoRadiator:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(37);
                        Part NewTorsoRadiator = new Part(Name, PartTypes.TorsoRadiator);
                        if (NewTorsoRadiator != null)
                        {
                            if (lstTorsoRadiator.Items.Contains(Name))
                            {
                                MessageBox.Show("This torso radiator is already listed.\r\n" + Name);
                                return;
                            }
                            ListTorsoRadiator.Add(NewTorsoRadiator);
                            lstTorsoRadiator.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.TorsoShell:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(22);
                        Part NewTorsoShell = new Part(Name, PartTypes.Shell);
                        if (NewTorsoShell != null)
                        {
                            if (lstTorsoShell.Items.Contains(Name))
                            {
                                MessageBox.Show("This torso shell is already listed.\r\n" + Name);
                                return;
                            }
                            ListTorsoShell.Add(NewTorsoShell);
                            lstTorsoShell.Items.Add(Name);
                        }
                        break;

                    #endregion

                    #region Arm

                    case ItemSelectionChoices.ArmShell:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(22);
                        Part NewArmShell = new Part(Name, PartTypes.Shell);
                        if (NewArmShell != null)
                        {
                            if (lstArmShell.Items.Contains(Name))
                            {
                                MessageBox.Show("This shell is already listed.\r\n" + Name);
                                return;
                            }
                            ListArmShell.Add(NewArmShell);
                            lstArmShell.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.ArmStrength:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(22);
                        Part NewArmStrength = new Part(Name, PartTypes.Strength);
                        if (NewArmStrength != null)
                        {
                            if (lstArmStrength.Items.Contains(Name))
                            {
                                MessageBox.Show("This strength is already listed.\r\n" + Name);
                                return;
                            }
                            ListArmStrength.Add(NewArmStrength);
                            lstArmStrength.Items.Add(Name);
                        }
                        break;

                    #endregion

                    #region Legs

                    case ItemSelectionChoices.LegsShell:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(22);
                        Part NewLegsShell = new Part(Name, PartTypes.Shell);
                        if (NewLegsShell != null)
                        {
                            if (lstLegsShell.Items.Contains(Name))
                            {
                                MessageBox.Show("This shell is already listed.\r\n" + Name);
                                return;
                            }
                            ListLegsShell.Add(NewLegsShell);
                            lstLegsShell.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.LegsStrength:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(22);
                        Part NewLegsStrength = new Part(Name, PartTypes.Strength);
                        if (NewLegsStrength != null)
                        {
                            if (lstLegsStrength.Items.Contains(Name))
                            {
                                MessageBox.Show("This strength is already listed.\r\n" + Name);
                                return;
                            }
                            ListLegsStrength.Add(NewLegsStrength);
                            lstLegsStrength.Items.Add(Name);
                        }
                        break;

                    #endregion
                }
            }
        }
    }
}
