using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.Editors.ComboEditor
{
    public partial class ComboEditor : BaseEditor
    {
        private enum ItemSelectionChoices { AnimationName, NextComboName };

        //Buffer used to draw in the panTimelineViewer.
        private BufferedGraphicsContext panTimelineViewerContext;

        private BufferedGraphics panTimelineViewerGraphicDevice;
        private Graphics panTimelineViewerGraphics;

        private int AnimationStart;
        private int AnimationEnd;
        private ItemSelectionChoices ItemSelectionChoice;
        private List<int> ListStart;
        private List<int> ListEnd;

        public ComboEditor()
        {
            InitializeComponent();

            ListStart = new List<int>();
            ListEnd = new List<int>();
            cbRotationType.SelectedIndex = 0;

            //Create a new buffer based on the pannel.
            panTimelineViewerGraphics = panAttackTimeline.CreateGraphics();
            panTimelineViewerContext = BufferedGraphicsManager.Current;
            panTimelineViewerContext.MaximumBuffer = new Size(panAttackTimeline.Width, panAttackTimeline.Height);
            panTimelineViewerGraphicDevice = panTimelineViewerContext.Allocate(panTimelineViewerGraphics, new Rectangle(0, 0, panAttackTimeline.Width, panAttackTimeline.Height));
        }

        public ComboEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                SaveItem(FilePath, "New Item");
            }

            LoadCombo(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathTripleThunderCombos }, "Triple Thunder/Combos/", new string[] { ".ttc" }, typeof(ComboEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtAnimationName.Text);
            BW.Write(cbAnimationType.SelectedIndex);
            BW.Write((byte)cbRotationType.SelectedIndex);
            BW.Write(cbInstantActivation.Checked);

            BW.Write(lstComboList.Items.Count);
            for (int C = 0; C < lstComboList.Items.Count; C++)
            {
                Combo ActiveNextCombo = (Combo)lstComboList.Items[C];

                BW.Write(ActiveNextCombo.ComboName);
                BW.Write(ListStart[C]);
                BW.Write(ListEnd[C]);

                BW.Write(ActiveNextCombo.ListInputChoice.Count);
                for (int I = 0; I < ActiveNextCombo.ListInputChoice.Count; I++)
                {
                    BW.Write((byte)ActiveNextCombo.ListInputChoice[I].AttackInput);
                    BW.Write((byte)ActiveNextCombo.ListInputChoice[I].MovementInput);
                    BW.Write(ActiveNextCombo.ListInputChoice[I].NextInputDelay);
                }
            }

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Combo at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Combo.</param>
        private void LoadCombo(string ComboPath)
        {
            string Name = ComboPath.Substring(0, ComboPath.Length - 4).Substring(30);
            this.Text = Name + " - Project Eternity Triple Thunder Combo Editor";

            Combo LoadedCombo = new Combo(Name);
            if (!string.IsNullOrEmpty(LoadedCombo.AnimationName))
            {
                LoadAnimationEnd(LoadedCombo.AnimationName);
            }
            txtAnimationLength.Text = AnimationEnd.ToString();

            txtAnimationName.Text = LoadedCombo.AnimationName;
            cbAnimationType.SelectedIndex = (int)LoadedCombo.AnimationType;
            cbRotationType.SelectedIndex = (byte)LoadedCombo.ComboRotationType;
            cbInstantActivation.Checked = LoadedCombo.InstantActivation;

            for (int C = 0; C < LoadedCombo.ListNextCombo.Count; C++)
            {
                lstComboList.Items.Add(LoadedCombo.ListNextCombo[C]);
                ListStart.Add(LoadedCombo.ListStart[C]);
                ListEnd.Add(LoadedCombo.ListEnd[C]);
            }

            if (lstComboList.Items.Count > 0)
            {
                lstComboList.SelectedIndex = 0;
            }

            if (string.IsNullOrEmpty(txtAnimationName.Text))
            {
                gbComboList.Enabled = false;
            }
        }

        private void LoadAnimationEnd(string AnimationPath)
        {
            FileStream FS = new FileStream("Content/Animations/" + AnimationPath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            AnimationStart = BR.ReadInt32();
            AnimationEnd = BR.ReadInt32();

            FS.Close();
            BR.Close();
        }

        public void DrawTimeline()
        {
            panTimelineViewerGraphicDevice.Graphics.Clear(Color.White);

            panTimelineViewerGraphicDevice.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, panAttackTimeline.Width - 1, panAttackTimeline.Height - 1));

            for (int C = 0; C < lstComboList.Items.Count; C++)
            {
                Combo ActiveNextCombo = (Combo)lstComboList.Items[C];

                int RealStart = (int)(ListStart[C] / (float)(AnimationEnd) * panAttackTimeline.Width);
                int RealEnd = (int)((ListEnd[C] - ListStart[C]) / (float)(AnimationEnd) * panAttackTimeline.Width - 1);

                panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.Blue, new Rectangle(RealStart, 1, RealEnd, panAttackTimeline.Height - 2));
            }
            panTimelineViewerGraphicDevice.Render();
        }

        private void btnSelectAnimation_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AnimationName;
            ListMenuItemsSelected(ShowContextMenuWithItem("Animations", "Select an animation to use", true));
            DrawTimeline();
        }

        private void btnAddCombo_Click(object sender, EventArgs e)
        {
            Combo NewCombo = new Combo();
            NewCombo.ComboName = "Unasigned Combo";
            NewCombo.ListInputChoice.Add(new InputChoice(AttackInputs.None, MovementInputs.None));
            ListStart.Add(0);
            ListEnd.Add(0);
            lstComboList.Items.Add(NewCombo);
            lstComboList.SelectedItem = NewCombo;
        }

        private void btnRemoveCombo_Click(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0)
            {
                int Index = lstComboList.SelectedIndex;
                lstComboList.Items.RemoveAt(Index);
                ListStart.RemoveAt(Index);
                ListEnd.RemoveAt(Index);

                if (Index >= lstComboList.Items.Count)
                {
                    lstComboList.SelectedIndex = lstComboList.Items.Count - 1;
                }
                else
                {
                    lstComboList.SelectedIndex = Index;
                }
            }
        }

        private void btnSelectCombo_Click(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.NextComboName;
                ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderCombos, "Select a combo to import", true));
            }
        }

        private void lstComboList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0)
            {
                Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];

                if (ListStart.Count != 0)
                {
                    txtStartTime.Minimum = AnimationStart;
                    txtStartTime.Value = ListStart[lstComboList.SelectedIndex];

                    txtEndTime.Maximum = AnimationEnd;

                    if (ListEnd[lstComboList.SelectedIndex] > AnimationEnd)
                        txtEndTime.Maximum = ListEnd[lstComboList.SelectedIndex];

                    txtEndTime.Value = ListEnd[lstComboList.SelectedIndex];
                }
                else
                {
                    txtStartTime.Minimum = 0;
                    txtStartTime.Value = 0;

                    txtEndTime.Maximum = 0;
                    txtEndTime.Value = 0;
                }

                gbInput.Enabled = true;

                lstInput.Items.Clear();
                for (int I = 0; I < ActiveCombo.ListInputChoice.Count; I++)
                    lstInput.Items.Add("Input " + (1 + I));

                if (lstInput.Items.Count > 0)
                {
                    lstInput.SelectedIndex = 0;
                }
            }
            else
            {
                lstInput.Items.Clear();
                gbInput.Enabled = false;
            }
        }

        private void txtStartTime_ValueChanged(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0 && lstComboList.SelectedIndex < ListStart.Count)
            {
                ListStart[lstComboList.SelectedIndex] = (int)txtStartTime.Value;
                DrawTimeline();
            }
        }

        private void txtEndTime_ValueChanged(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0 && lstComboList.SelectedIndex < ListEnd.Count)
            {
                ListEnd[lstComboList.SelectedIndex] = (int)txtEndTime.Value;
                DrawTimeline();
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void ComboEditor_Shown(object sender, EventArgs e)
        {
            DrawTimeline();
        }

        private void btnAddInput_Click(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0)
            {
                Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];
                ActiveCombo.ListInputChoice.Add(new InputChoice(AttackInputs.None, MovementInputs.None));
                lstInput.Items.Add("Input " + (1 + lstInput.Items.Count));
                lstInput.SelectedIndex = lstInput.Items.Count - 1;
            }
        }

        private void btnRemoveInput_Click(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0 && lstInput.SelectedIndex >= 0)
            {
                Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];
                int Index = lstInput.SelectedIndex;
                ActiveCombo.ListInputChoice.RemoveAt(lstInput.SelectedIndex);
                lstInput.Items.RemoveAt(lstInput.SelectedIndex);
                if (Index >= lstInput.Items.Count)
                {
                    lstInput.SelectedIndex = lstInput.Items.Count - 1;
                }
                else
                {
                    lstInput.SelectedIndex = Index;
                }
            }
        }

        private void lstInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstComboList.SelectedIndex >= 0 && lstInput.SelectedIndex >= 0)
            {
                Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];
                InputChoice ActiveInputChoice = ActiveCombo.ListInputChoice[lstInput.SelectedIndex];

                cbAttackChoice.SelectedIndex = (int)ActiveInputChoice.AttackInput;
                cbMovementChoice.SelectedIndex = (int)ActiveInputChoice.MovementInput;
                txtNextInputDelay.Value = ActiveInputChoice.NextInputDelay;
            }
        }

        private void cbAttackChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstInput.SelectedIndex < 0)
                return;

            Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];

            ActiveCombo.ListInputChoice[lstInput.SelectedIndex].AttackInput = (AttackInputs)cbAttackChoice.SelectedIndex;
        }

        private void cbMovementChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstInput.SelectedIndex < 0)
                return;

            Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];

            ActiveCombo.ListInputChoice[lstInput.SelectedIndex].MovementInput = (MovementInputs)cbMovementChoice.SelectedIndex;
        }

        private void txtNextInputDelay_ValueChanged(object sender, EventArgs e)
        {
            Combo ActiveCombo = (Combo)lstComboList.Items[lstComboList.SelectedIndex];

            ActiveCombo.ListInputChoice[lstInput.SelectedIndex].NextInputDelay = (int)txtNextInputDelay.Value;
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
                    case ItemSelectionChoices.AnimationName:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Animations") + 11);
                        if (Name != null)
                        {
                            LoadAnimationEnd(Name);

                            txtAnimationName.Text = Name;
                            txtAnimationLength.Text = AnimationEnd.ToString();
                            gbComboList.Enabled = true;
                        }
                        break;

                    case ItemSelectionChoices.NextComboName:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(30);
                        if (Name != null)
                        {
                            Combo NewCombo = new Combo(Name);
                            NewCombo.ListInputChoice = new List<InputChoice>();
                            lstComboList.Items[lstComboList.SelectedIndex] = NewCombo;
                        }
                        break;
                }
            }
        }
    }
}
