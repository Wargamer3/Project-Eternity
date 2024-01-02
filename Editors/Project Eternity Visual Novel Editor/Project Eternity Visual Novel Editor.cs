using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class ProjectEternityVisualNovelEditor : BaseEditor
    {
        public delegate void InitScriptDelegate(CutsceneScript NewScript);

        private InitScriptDelegate InitScript;

        private FormWindowState LastWindowState;
        
        private int LastTimelineIndex;

        private CheckBox cbShowFlowchart;
        private DialogEditor DialogEditor;
        private FlowchartEditor FlowchartEditor;
        private Dictionary<string, CutsceneScript> Scripts;
        private Dictionary<string, VisualNovelEditorExtension> DicExtension;

        public ProjectEternityVisualNovelEditor()
        {
            InitializeComponent();

            LastWindowState = this.WindowState;
            
            LastTimelineIndex = -1;
        }

        public ProjectEternityVisualNovelEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                //Create the Part file.
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Number of characters.
                BW.Write(0);//Number of bust characters.
                BW.Write(0);//Number of portrait characters.
                BW.Write(0);//Number of backgrounds.
                BW.Write(0);//Number of scripts.

                FS.Close();
                BW.Close();
            }

            #region Extensions

            InitScript = (NewScript) => DialogEditor.InitScript(NewScript);
            DicExtension = GetAllExtensions<VisualNovelEditorExtension>(InitScript);
            foreach (KeyValuePair<string, VisualNovelEditorExtension> ActiveExtension in DicExtension)
            {
                ToolStripMenuItem tsmExtension = new ToolStripMenuItem(ActiveExtension.Value.ToString());
                tsmExtension.Tag = ActiveExtension.Value;
                tsmExtension.Size = new Size(98, 20);
                tsmExtension.Click += new EventHandler(tsmExtension_Click);
                mnuToolBar.Items.Add(tsmExtension);
            }

            #endregion

            #region cbDrawScripts

            //Init the DrawScripts button (as it can't be done with the tool box)
            VisualNovelViewer.DrawScripts = false;
            cbShowFlowchart = new CheckBox();
            cbShowFlowchart.Text = "Show Flowchart";
            cbShowFlowchart.AutoSize = false;
            //Link a CheckedChanged event to a method.
            cbShowFlowchart.CheckedChanged += new EventHandler(cbShowFlowchart_CheckedChanged);
            cbShowFlowchart.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowFlowchart.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost tsmDrawScripts = new ToolStripControlHost(cbShowFlowchart);
            tsmDrawScripts.AutoSize = false;
            tsmDrawScripts.Width = 150;
            mnuToolBar.Items.Add(tsmDrawScripts);

            #endregion

            LoadVN(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathVisualNovel }, "Visual Novels/", new string[] { ".pevn" }, typeof(ProjectEternityVisualNovelEditor)),
                new EditorInfo(new string[] { GUIRootPathVisualNovelBustPortraits, GUIRootPathVisualNovelCharacters }, "Visual Novels/Bust Portraits/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathVisualNovelPortraits, GUIRootPathVisualNovelCharacters }, "Visual Novels/Portraits/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathVisualNovelBackgrounds }, "Visual Novels/Backgrounds/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            VisualNovelViewer.ActiveVisualNovel.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            FlowchartEditor.Close();
        }

        #region Methods

        private void LoadVN(string VNPath)
        {
            string Name = VNPath.Substring(0, VNPath.Length - 5).Substring(22);
            this.Text = Name + " - Project Eternity Visual Novel Editor";

            VisualNovelViewer.ActiveVisualNovel = new VisualNovel(Name);
            VisualNovelViewer.ActiveVisualNovel.UseLocalization = false;
            VisualNovelViewer.ActiveVisualNovel.Content = new ContentManager(VisualNovelViewer.Services);
            VisualNovelViewer.ActiveVisualNovel.Content.RootDirectory = "Content";
            VisualNovelViewer.ActiveVisualNovel.ListGameScreen = new List<GameScreens.GameScreen>();

            VisualNovelViewer.Preload();

            VisualNovelViewer.Services.AddService<GraphicsDevice>(VisualNovelViewer.GraphicsDevice);
            VisualNovelViewer.ActiveVisualNovel.Load();

            for (int C = 0; C < VisualNovelViewer.ActiveVisualNovel.ListCharacter.Count; C++)
            {
                lstCharacters.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListCharacter[C]);
                cboLeftCharacter.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListCharacter[C].CharacterName);
                cboRightCharacter.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListCharacter[C].CharacterName);
                cboTopCharacter.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListCharacter[C].CharacterName);
                cboBottomCharacter.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListCharacter[C].CharacterName);
            }

            for (int C = 0; C < VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Count; C++)
            {
                lstExtraPortraits.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListBustPortrait[C]);
            }

            for (int P = 0; P < VisualNovelViewer.ActiveVisualNovel.ListPortrait.Count; P++)
            {
                if (VisualNovelViewer.ActiveVisualNovel.ListPortrait[P].IsAnimated)
                    continue;

                lstExtraPortraits.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListPortrait[P]);
            }

            for (int B = 0; B < VisualNovelViewer.ActiveVisualNovel.ListBackground.Count; B++)
            {
                lstBackgrounds.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListBackground[B]);
                cboBackground.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListBackground[B]);
            }

            PopulateDialog();
        }

        private void PopulateDialog()
        {
            lstDialogs.Items.Clear();

            foreach (Dialog ActiveTimelineDialog in VisualNovelViewer.ActiveVisualNovel.Timeline)
            {
                lstDialogs.Items.Add(ActiveTimelineDialog);

                PopulateDialog(ActiveTimelineDialog);
            }

            if (lstDialogs.Items.Count > 0)
            {
                lstDialogs.SelectedIndex = 0;
            }
        }

        private void PopulateDialog(Dialog StartingDialog)
        {
            foreach (int FollowingDialogIndex in StartingDialog.ListNextDialog)
            {
                if (!lstDialogs.Items.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[FollowingDialogIndex]))
                {
                    lstDialogs.Items.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[FollowingDialogIndex]);

                    PopulateDialog(VisualNovelViewer.ActiveVisualNovel.ListDialog[FollowingDialogIndex]);
                }
            }
        }

        #endregion

        #region Menu

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, "");
        }

        private void tsmExportLocalizationFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            SFD.FileName = VisualNovelViewer.ActiveVisualNovel.VisualNovelPath + "-default.xml";
            if (SFD.ShowDialog() == DialogResult.OK)
            {
                VisualNovelViewer.ActiveVisualNovel.ExportLocalizationFile(SFD.FileName);
            }
        }

        private void tsmEditCurrentDialog_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                DialogEditor.SetDialog((Dialog)lstDialogs.SelectedItem);
                DialogEditor.ShowDialog();
            }
        }

        private void tsmExtension_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ActiveItem = (ToolStripMenuItem)sender;
            VisualNovelEditorExtension ActiveExtension = (VisualNovelEditorExtension)ActiveItem.Tag;
            ActiveExtension.OnClick(this);
        }

        #endregion

        #region Basic Dialog manipulation

        private void DialogSelected(Dialog CurrentDialog)
        {
            foreach (KeyValuePair<string, VisualNovelEditorExtension> ActiveExtension in DicExtension)
            {
                ActiveExtension.Value.SetCurrentDialog(CurrentDialog);
            }

            LastTimelineIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(CurrentDialog);
            VisualNovelViewer.ActiveVisualNovel.CurrentDialog = CurrentDialog;
            VisualNovelViewer.ActiveVisualNovel.DialogChoice = 0;

            int CharacterIndex;
            int PortraitIndex;

            VisualNovelViewer.ActiveVisualNovel.GetBustPortraitIndices(CurrentDialog.LeftCharacter, out CharacterIndex, out PortraitIndex);

            cboLeftCharacter.SelectedIndex = CharacterIndex;
            cboLeftPortrait.SelectedIndex = PortraitIndex;

            VisualNovelViewer.ActiveVisualNovel.GetBustPortraitIndices(CurrentDialog.RightCharacter, out CharacterIndex, out PortraitIndex);

            cboRightCharacter.SelectedIndex = CharacterIndex;
            cboRightPortrait.SelectedIndex = PortraitIndex;

            VisualNovelViewer.ActiveVisualNovel.GetBoxPortraitIndices(CurrentDialog.TopCharacter, out CharacterIndex, out PortraitIndex);

            cboTopCharacter.SelectedIndex = CharacterIndex;
            cboTopCharacterPortrait.SelectedIndex = PortraitIndex;

            VisualNovelViewer.ActiveVisualNovel.GetBoxPortraitIndices(CurrentDialog.BottomCharacter, out CharacterIndex, out PortraitIndex);
            cboBottomCharacter.SelectedIndex = CharacterIndex;
            cboBottomCharacterPortrait.SelectedIndex = PortraitIndex;

            if (CurrentDialog.Back != null)
                cboBackground.SelectedIndex = VisualNovelViewer.ActiveVisualNovel.ListBackground.IndexOf(CurrentDialog.Back) + 1;
            else
                cboBackground.SelectedIndex = -1;

            //Update the ActiveCharacter value.
            if (CurrentDialog.ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Left)
            {
                if (!rbLeftCharacter.Checked)
                    rbLeftCharacter.Checked = true;
            }
            else if (CurrentDialog.ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Right)
            {
                if (!rbRightCharacter.Checked)
                    rbRightCharacter.Checked = true;
            }
            else if (CurrentDialog.ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Both)
            {
                if (!rbBoth.Checked)
                    rbBoth.Checked = true;
            }
            else
            {
                rbNone.Checked = true;
            }

            //Update the text.
            txtText.Text = CurrentDialog.Text;
            txtTextPreview.Text = CurrentDialog.TextPreview;

            if (CurrentDialog.TopPortaitVisibleState == Dialog.PortaitVisibleStates.Visible)
                rbTopDialogVisible.Checked = true;
            else if (CurrentDialog.TopPortaitVisibleState == Dialog.PortaitVisibleStates.Greyed)
                rbTopDialogGreyed.Checked = true;
            else if (CurrentDialog.TopPortaitVisibleState == Dialog.PortaitVisibleStates.Invisible)
                rbTopDialogInvisible.Checked = true;
            txtTopDialogText.Text = CurrentDialog.TextTop;

            if (CurrentDialog.BottomPortaitVisibleState == Dialog.PortaitVisibleStates.Visible)
                rbBottomDialogVisible.Checked = true;
            else if (CurrentDialog.BottomPortaitVisibleState == Dialog.PortaitVisibleStates.Greyed)
                rbBottomDialogGreyed.Checked = true;
            else if (CurrentDialog.BottomPortaitVisibleState == Dialog.PortaitVisibleStates.Invisible)
                rbBottomDialogInvisible.Checked = true;
            txtBottomDialogText.Text = CurrentDialog.Text;
            txtBottomDialogTextPreview.Text = CurrentDialog.TextPreview;

            VisualNovelViewer.ActiveVisualNovel.UpdateTextChoices();
            
            ckBustOverrideData.Checked = CurrentDialog.OverrideCharacterPriority;
            ckTopDialogOverrideData.Checked = CurrentDialog.OverrideCharacterPriority;
            ckBottomDialogOverrideData.Checked = CurrentDialog.OverrideCharacterPriority;
        }

        private void lstDialogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null && LastTimelineIndex != VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf((Dialog)lstDialogs.SelectedItem))
            {
                //Update the combo box with the new Dialog data.
                DialogSelected((Dialog)lstDialogs.SelectedItem);
            }
        }

        private void cboBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                if (cboBackground.SelectedIndex > 0)
                    ((Dialog)lstDialogs.SelectedItem).Back = VisualNovelViewer.ActiveVisualNovel.ListBackground[cboBackground.SelectedIndex - 1];
                else
                    ((Dialog)lstDialogs.SelectedItem).Back = null;

                lstDialogs.Update();
            }
        }

        private void btnAddFrame_Click(object sender, EventArgs e)
        {
            Dialog NewDialog;
            //If it's not the first Dialog created, copy the last one.(Just so if you make a long list of dialog with the same characters you don't have to copy that part manually)
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog.Count > 0)
            {
                NewDialog = new Dialog(VisualNovelViewer.ActiveVisualNovel.ListDialog.Last());
            }
            else
                NewDialog = new Dialog(null, null, null, null, null, Dialog.ActiveBustCharacterStates.None, "", "");

            //Add it to the Timeline, if there's none, add it to the first Frame place.
            if (VisualNovelViewer.ActiveVisualNovel.Timeline.Count == 0)
                NewDialog.Position = new Microsoft.Xna.Framework.Point(0, 0);
            else
                NewDialog.Position = new Microsoft.Xna.Framework.Point(0, VisualNovelViewer.ActiveVisualNovel.Timeline.Last().Position.Y + VisualNovelViewer.BoxHeight);

            NewDialog.CutsceneBefore = new Cutscene(null, Scripts);
            NewDialog.CutsceneDuring = new Cutscene(null, Scripts);
            NewDialog.CutsceneAfter = new Cutscene(null, Scripts);

            //Add it to the Lists
            VisualNovelViewer.ActiveVisualNovel.Timeline.Add(NewDialog);
            VisualNovelViewer.ActiveVisualNovel.ListDialog.Add(NewDialog);
            lstDialogs.Items.Add(NewDialog);
            lstDialogs.SelectedIndex = lstDialogs.Items.Count - 1;

            //Update its text in the lists.
            lstDialogs.Update();
        }

        private void btnInsertFrame_Click(object sender, EventArgs e)
        {//If there is something selected.
            if (lstDialogs.SelectedItem != null)
            {
                int InsertIndex = lstDialogs.SelectedIndex;
                Dialog NewDialog;
                //If it's not the first Dialog created, copy the last one.(Just so if you make a long list of dialog with the same characters you don't have to copy that part manually)
                if (VisualNovelViewer.ActiveVisualNovel.ListDialog.Count > 0)
                    NewDialog = new Dialog(VisualNovelViewer.ActiveVisualNovel.ListDialog[InsertIndex]);
                else
                    NewDialog = new Dialog(null, null, null, null, null, Dialog.ActiveBustCharacterStates.None, "", "");

                //Add it to the Timeline, if there's none, add it to the first Frame place.
                if (VisualNovelViewer.ActiveVisualNovel.Timeline.Count == 0)
                    NewDialog.Position = new Microsoft.Xna.Framework.Point(0, 0);
                else
                    NewDialog.Position = new Microsoft.Xna.Framework.Point(0, VisualNovelViewer.ActiveVisualNovel.Timeline.Last().Position.Y + VisualNovelViewer.BoxHeight);

                NewDialog.CutsceneBefore = new Cutscene(null, Scripts);
                NewDialog.CutsceneDuring = new Cutscene(null, Scripts);
                NewDialog.CutsceneAfter = new Cutscene(null, Scripts);

                //Add it to the Lists
                VisualNovelViewer.ActiveVisualNovel.Timeline.Add(NewDialog);
                VisualNovelViewer.ActiveVisualNovel.ListDialog.Insert(InsertIndex, NewDialog);
                lstDialogs.Items.Insert(InsertIndex, NewDialog);

                foreach(Dialog ActiveDialog in VisualNovelViewer.ActiveVisualNovel.ListDialog)
                {
                    for (int D = 0; D < ActiveDialog.ListNextDialog.Count; ++D)
                    {
                        if (ActiveDialog.ListNextDialog[D] >= InsertIndex)
                        {
                            ActiveDialog.ListNextDialog[D] = ActiveDialog.ListNextDialog[D] + 1;
                        }
                    }
                }

                //Update its text in the lists.
                lstDialogs.SelectedIndex = InsertIndex;
                lstDialogs.Update();
            }
            else//Just add it normally.
            {
                btnAddFrame_Click(sender, e);
            }
        }

        private void btnDeleteFrame_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                Dialog RemovedDialog = (Dialog)lstDialogs.SelectedItem;
                int Index = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(RemovedDialog);

                //Move the current tile set.
                if (VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(RemovedDialog))
                    VisualNovelViewer.ActiveVisualNovel.Timeline.Remove(RemovedDialog);

                foreach (Dialog ActiveDialog in VisualNovelViewer.ActiveVisualNovel.ListDialog)
                {
                    for (int i = ActiveDialog.ListNextDialog.Count - 1; i >= 0; --i)
                    {
                        if (ActiveDialog.ListNextDialog[i] == Index)
                        {
                            ActiveDialog.ListNextDialog.RemoveAt(i);
                        }
                        else if (ActiveDialog.ListNextDialog[i] > Index)
                        {
                            ActiveDialog.ListNextDialog[i] -= 1;
                        }
                    }
                }

                VisualNovelViewer.ActiveVisualNovel.ListDialog.Remove(RemovedDialog);
                lstDialogs.Items.Remove(RemovedDialog);

                //Replace the index with a new one.
                if (lstDialogs.Items.Count > 0)
                {
                    if (Index >= lstDialogs.Items.Count)
                        lstDialogs.SelectedIndex = lstDialogs.Items.Count - 1;
                    else
                        lstDialogs.SelectedIndex = Index;
                }
            }
        }

        private void btnClearFrames_Click(object sender, EventArgs e)
        {
            if (lstDialogs.Items.Count >= 0)
            {
                lstDialogs.SelectedIndex = 0;
                while (lstDialogs.Items.Count > 0)
                    btnDeleteFrame_Click(sender, e);
            }
        }

        private void ckOverrideData_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                CheckBox CheckboxSender = (CheckBox)sender;

                ((Dialog)lstDialogs.SelectedItem).OverrideCharacterPriority = CheckboxSender.Checked;

                ckBustOverrideData.Checked = CheckboxSender.Checked;
                ckTopDialogOverrideData.Checked = CheckboxSender.Checked;
                ckBottomDialogOverrideData.Checked = CheckboxSender.Checked;

                btnBustDataOverride.Enabled = CheckboxSender.Checked;
                btnTopDialogDataOverride.Enabled = CheckboxSender.Checked;
                btnBottomDialogDataOverride.Enabled = CheckboxSender.Checked;
            }
        }

        private void btnDataOverride_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                CharacterData NewForm = new CharacterData(((Dialog)lstDialogs.SelectedItem).ListSpeakerPriority);
                NewForm.ShowDialog();
            }
        }

        #endregion

        #region Bust Dialog

        private void cboLeftCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                cboLeftPortrait.Items.Clear();
                cboLeftPortrait.Items.Add("None");

                if (cboLeftCharacter.SelectedIndex > 0)
                {
                    Character ActiveCharacter = VisualNovelViewer.ActiveVisualNovel.ListCharacter[cboLeftCharacter.SelectedIndex - 1].LoadedCharacter;
                    for (int P = 0; P < ActiveCharacter.ArrayPortraitBustPath.Length; ++P)
                    {
                        string PortraitType = "Bust Portraits";
                        string Name = ActiveCharacter.ArrayPortraitBustPath[P];
                        string FullName = PortraitType + "/" + Name;

                        SimpleAnimation NewCharacter = new SimpleAnimation(Name, ActiveCharacter.FullName, VisualNovelViewer.content.Load<Texture2D>("Visual Novels/" + FullName));
                        cboLeftPortrait.Items.Add(NewCharacter);
                    }

                    for (int P = 0; P < VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Count; ++P)
                    {
                        SimpleAnimation NewCharacter = new SimpleAnimation(VisualNovelViewer.ActiveVisualNovel.ListBustPortrait[P]);
                        NewCharacter.Path = ActiveCharacter.FullName;

                        cboLeftPortrait.Items.Add(NewCharacter);
                    }
                }
                else
                {
                    cboLeftPortrait.Items.AddRange(VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.ToArray());
                }
            }
        }

        private void cboRightCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                cboRightPortrait.Items.Clear();
                cboRightPortrait.Items.Add("None");

                if (cboRightCharacter.SelectedIndex > 0)
                {
                    Character ActiveCharacter = VisualNovelViewer.ActiveVisualNovel.ListCharacter[cboRightCharacter.SelectedIndex - 1].LoadedCharacter;
                    for (int P = 0; P < ActiveCharacter.ArrayPortraitBustPath.Length; ++P)
                    {
                        string PortraitType = "Bust Portraits";
                        string Name = ActiveCharacter.ArrayPortraitBustPath[P];
                        string FullName = PortraitType + "/" + Name;

                        SimpleAnimation NewCharacter = new SimpleAnimation(Name, ActiveCharacter.FullName, VisualNovelViewer.content.Load<Texture2D>("Visual Novels/" + FullName));
                        cboRightPortrait.Items.Add(NewCharacter);
                    }

                    for (int P = 0; P < VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Count; ++P)
                    {
                        SimpleAnimation NewCharacter = new SimpleAnimation(VisualNovelViewer.ActiveVisualNovel.ListBustPortrait[P]);
                        NewCharacter.Path = ActiveCharacter.FullName;

                        cboRightPortrait.Items.Add(NewCharacter);
                    }
                }
                else
                {
                    cboRightPortrait.Items.AddRange(VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.ToArray());
                }
            }
        }

        private void cboLeftPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboLeftPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboLeftPortrait.SelectedItem;
                }

                ((Dialog)lstDialogs.SelectedItem).LeftCharacter = ActivePortrait;

                lstDialogs.Update();
            }
        }

        private void cboRightPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboRightPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboRightPortrait.SelectedItem;
                }

               ((Dialog)lstDialogs.SelectedItem).RightCharacter = ActivePortrait;

                lstDialogs.Update();
            }
        }

        private void rbLeftCharacter_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                ((Dialog)lstDialogs.SelectedItem).ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Left;
                lstDialogs.Update();
            }
        }

        private void rbRightCharacter_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                ((Dialog)lstDialogs.SelectedItem).ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Right;
                lstDialogs.Update();
            }
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                ((Dialog)lstDialogs.SelectedItem).ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.None;
                lstDialogs.Update();
            }
        }

        private void rbBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                ((Dialog)lstDialogs.SelectedItem).ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Both;
                lstDialogs.Update();
            }
        }

        private void txtText_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Open the text editor.
                new TextEdit(((Dialog)lstDialogs.SelectedItem).Text, txtText).ShowDialog();
                tabBustDialog.Focus();
            }
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Update the current Dialog Text with the textbox text.
                ((Dialog)lstDialogs.SelectedItem).Text = txtText.Text;
                txtBottomDialogText.Text = txtText.Text;
            }
        }

        private void txtTextPreview_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Update the current Dialog TextPreview with the textbox text.
                ((Dialog)lstDialogs.SelectedItem).TextPreview = txtTextPreview.Text;
                txtBottomDialogTextPreview.Text = txtTextPreview.Text;
            }
        }

        #endregion

        #region Top Dialog

        private void cboTopCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                cboTopCharacterPortrait.Items.Clear();
                cboTopCharacterPortrait.Items.Add("None");

                if (cboTopCharacter.SelectedIndex > 0)
                {
                    Character ActiveCharacter = VisualNovelViewer.ActiveVisualNovel.ListCharacter[cboTopCharacter.SelectedIndex - 1].LoadedCharacter;
                    for (int P = 0; P < ActiveCharacter.ArrayPortraitBoxPath.Length; ++P)
                    {
                        string PortraitType = "Portraits";
                        string Name = ActiveCharacter.ArrayPortraitBoxPath[P];
                        string FullName = PortraitType + "/" + Name;

                        SimpleAnimation NewCharacter = new SimpleAnimation(Name, ActiveCharacter.FullName, VisualNovelViewer.content.Load<Texture2D>("Visual Novels/" + FullName));
                        cboTopCharacterPortrait.Items.Add(NewCharacter);
                    }

                    for (int P = 0; P < VisualNovelViewer.ActiveVisualNovel.ListPortrait.Count; ++P)
                    {
                        SimpleAnimation NewCharacter = new SimpleAnimation(VisualNovelViewer.ActiveVisualNovel.ListPortrait[P]);
                        NewCharacter.Path = ActiveCharacter.FullName;

                        cboTopCharacterPortrait.Items.Add(NewCharacter);
                    }
                }
                else
                {
                    cboTopCharacterPortrait.Items.AddRange(VisualNovelViewer.ActiveVisualNovel.ListPortrait.ToArray());
                }
            }
        }

        private void cbTopCharacterPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboTopCharacterPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboTopCharacterPortrait.SelectedItem;
                }

                ((Dialog)lstDialogs.SelectedItem).TopCharacter = ActivePortrait;

                lstDialogs.Update();
            }
        }

        private void rbTopDialogVisibleState_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                if (rbTopDialogVisible.Checked)
                    ((Dialog)lstDialogs.SelectedItem).TopPortaitVisibleState = Dialog.PortaitVisibleStates.Visible;
                else if (rbTopDialogGreyed.Checked)
                    ((Dialog)lstDialogs.SelectedItem).TopPortaitVisibleState = Dialog.PortaitVisibleStates.Greyed;
                else if (rbTopDialogInvisible.Checked)
                    ((Dialog)lstDialogs.SelectedItem).TopPortaitVisibleState = Dialog.PortaitVisibleStates.Invisible;
            }
        }

        private void txtTopDialogText_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Open the text editor.
                new TextEdit(((Dialog)lstDialogs.SelectedItem).Text, txtTopDialogText).ShowDialog();
                tabTopDialog.Focus();
            }
        }

        private void txtTopDialogText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Update the current Dialog Text with the textbox text.
                ((Dialog)lstDialogs.SelectedItem).TextTop = txtTopDialogText.Text;
            }
        }

        #endregion

        #region Bottom Dialog

        private void cboBottomCharacterPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                cboBottomCharacterPortrait.Items.Clear();
                cboBottomCharacterPortrait.Items.Add("None");

                if (cboBottomCharacter.SelectedIndex > 0)
                {
                    Character ActiveCharacter = VisualNovelViewer.ActiveVisualNovel.ListCharacter[cboBottomCharacter.SelectedIndex - 1].LoadedCharacter;
                    string PortraitType = "Portraits";

                    for (int P = 0; P < ActiveCharacter.ArrayPortraitBoxPath.Length; ++P)
                    {
                        string Name = ActiveCharacter.ArrayPortraitBoxPath[P];
                        string FullName = PortraitType + "/" + Name;

                        SimpleAnimation NewCharacter = new SimpleAnimation(Name, ActiveCharacter.FullName, VisualNovelViewer.content.Load<Texture2D>("Visual Novels/" + FullName));
                        cboBottomCharacterPortrait.Items.Add(NewCharacter);
                    }

                    for (int P = 0; P < VisualNovelViewer.ActiveVisualNovel.ListPortrait.Count; ++P)
                    {
                        SimpleAnimation NewCharacter = new SimpleAnimation(VisualNovelViewer.ActiveVisualNovel.ListPortrait[P]);
                        NewCharacter.Path = ActiveCharacter.FullName;

                        cboBottomCharacterPortrait.Items.Add(NewCharacter);
                    }
                }
                else
                {
                    cboBottomCharacterPortrait.Items.AddRange(VisualNovelViewer.ActiveVisualNovel.ListPortrait.ToArray());
                }
            }
        }

        private void cboBottomCharacterPortrait_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboBottomCharacterPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboBottomCharacterPortrait.SelectedItem;
                }

                ((Dialog)lstDialogs.SelectedItem).BottomCharacter = ActivePortrait;

                lstDialogs.Update();
            }
        }

        private void rbBottomDialogVisibleState_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {
                if (rbBottomDialogVisible.Checked)
                    ((Dialog)lstDialogs.SelectedItem).BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Visible;
                else if (rbBottomDialogGreyed.Checked)
                    ((Dialog)lstDialogs.SelectedItem).BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Greyed;
                else if (rbBottomDialogInvisible.Checked)
                    ((Dialog)lstDialogs.SelectedItem).BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Invisible;
            }
        }

        private void txtBottomDialogText_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Open the text editor.
                new TextEdit(((Dialog)lstDialogs.SelectedItem).Text, txtBottomDialogText).ShowDialog();
                tabBottomDialog.Focus();
            }
        }

        private void txtBottomDialogText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Update the current Dialog Text with the textbox text.
                ((Dialog)lstDialogs.SelectedItem).Text = txtBottomDialogText.Text;
                txtText.Text = txtBottomDialogText.Text;
            }
        }

        private void txtBottomDialogTextPreview_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedItem != null)
            {//Update the current Dialog TextPreview with the textbox text.
                ((Dialog)lstDialogs.SelectedItem).TextPreview = txtBottomDialogTextPreview.Text;
                txtTextPreview.Text = txtBottomDialogTextPreview.Text;
            }
        }

        #endregion

        #region Assets

        #region Characters

        private void lstLeftCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstExtraPortraits.SelectedIndex >= 0)
            {
                string CharacterName = lstExtraPortraits.SelectedItems[0].ToString();
                lstBackgrounds.SelectedIndex = -1;
                //Change textbox text to the chracter's name.
                txtAssetName.Text = CharacterName;
            }
        }

        private void btnAddCharacter_Click(object sender, EventArgs e)
        {
            List<string> Items = ShowContextMenuWithItem(GUIRootPathCharacters, "Chose a new character.");

            if (Items == null || Items.Count == 0)
                return;

            string FullName = Items[0].Substring(0, Items[0].Length - 4).Substring(19);

            //Create a new character out of the returned paths.

            VisualNovelCharacter NewCharacter = new VisualNovelCharacter(FullName);

            //Update the characters lists.
            cboLeftCharacter.Items.Add(FullName);
            cboRightCharacter.Items.Add(FullName);
            cboTopCharacter.Items.Add(FullName);
            cboBottomCharacter.Items.Add(FullName);
            lstCharacters.Items.Add(NewCharacter);
            VisualNovelViewer.ActiveVisualNovel.ListCharacter.Add(NewCharacter);
        }

        private void btnDeleteCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                int Index = lstCharacters.SelectedIndex;

                cboLeftCharacter.Items.RemoveAt(Index + 1);
                cboRightCharacter.Items.RemoveAt(Index + 1);
                cboTopCharacter.Items.RemoveAt(Index + 1);
                cboBottomCharacter.Items.RemoveAt(Index + 1);
                lstCharacters.Items.RemoveAt(Index);
                VisualNovelViewer.ActiveVisualNovel.ListCharacter.RemoveAt(Index);
            }
        }

        private void btnCharacterData_Click(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                CharacterData NewForm = new CharacterData(((VisualNovelCharacter)lstCharacters.Items[lstCharacters.SelectedIndex]).ListSpeakerPriority);
                NewForm.ShowDialog();
            }
        }

        private void btnAddAnimation_Click(object sender, EventArgs e)
        {
            List<string> Items = ShowContextMenuWithItem(GUIRootPathAnimations, "Chose a sprite for the new character.");

            if (Items == null || Items.Count == 0)
                return;

            string Name = Items[0].Substring(0, Items[0].Length - 4).Substring(19);

            //Create a new character out of the returned paths.
            SimpleAnimation NewCharacter = new SimpleAnimation(Name, Name, new AnimationLooped(Name));
            foreach (KeyValuePair<string, Timeline> Timeline in AnimationClass.LoadAllTimelines())
                NewCharacter.ActiveAnimation.DicTimeline.Add(Timeline.Key, Timeline.Value);
            NewCharacter.ActiveAnimation.Content = VisualNovelViewer.content;
            NewCharacter.ActiveAnimation.Load();

            VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Add(NewCharacter);
            VisualNovelViewer.ActiveVisualNovel.ListPortrait.Add(NewCharacter);

            //Update the characters lists.
            lstExtraPortraits.Items.Add(NewCharacter);

            cboLeftCharacter.Items.Add(Name);
            cboRightCharacter.Items.Add(Name);
            cboTopCharacter.Items.Add(Name);
            cboBottomCharacter.Items.Add(Name);
        }

        private void btnAddExtraPortrait_Click(object sender, EventArgs e)
        {
            List<string> Items = ShowContextMenuWithItem(GUIRootPathVisualNovelCharacters, "Chose a sprite for the new character.");

            if (Items == null || Items.Count == 0)
                return;

            for (int i = 0; i < Items.Count; ++i)
            {
                string FullName = Items[i].Substring(0, Items[i].Length - 4).Substring(22);
                string[] NameData = FullName.Split('\\', '/');
                string PortraitType = NameData[0];
                string Name = FullName.Substring(PortraitType.Length + 1);

                //Create a new character out of the returned paths.

                SimpleAnimation NewCharacter = new SimpleAnimation(Name, Name, VisualNovelViewer.content.Load<Texture2D>("Visual Novels/" + FullName));

                //Update the characters lists.
                lstExtraPortraits.Items.Add(NewCharacter);

                if (PortraitType == "Bust Portraits")
                {
                    cboLeftPortrait.Items.Add(NewCharacter);
                    cboRightPortrait.Items.Add(NewCharacter);
                    VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Add(NewCharacter);
                }
                else if (PortraitType == "Portraits")
                {
                    cboTopCharacterPortrait.Items.Add(NewCharacter);
                    cboBottomCharacterPortrait.Items.Add(NewCharacter);
                    VisualNovelViewer.ActiveVisualNovel.ListPortrait.Add(NewCharacter);
                }
            }
        }

        private void btnDeleteExtraPortrait_Click(object sender, EventArgs e)
        {
            if (lstExtraPortraits.SelectedIndex >= 0)
            {
                SimpleAnimation SelectedPortrait = (SimpleAnimation)lstExtraPortraits.SelectedItem;
                int Index = lstExtraPortraits.SelectedIndex;
                //Move the current tile set.
                VisualNovelViewer.ActiveVisualNovel.ListBustPortrait.Remove(SelectedPortrait);
                VisualNovelViewer.ActiveVisualNovel.ListPortrait.Remove(SelectedPortrait);
                lstExtraPortraits.Items.RemoveAt(Index);
                //Replace the index with a new one.
                if (lstExtraPortraits.Items.Count > 0)
                {
                    if (Index >= lstExtraPortraits.Items.Count)
                        lstExtraPortraits.SelectedIndex = lstExtraPortraits.Items.Count - 1;
                    else
                        lstExtraPortraits.SelectedIndex = Index;
                }
                else
                    txtAssetName.Text = "";
            }
        }

        #endregion

        #region Backgrounds

        private void lstBackgrounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBackgrounds.SelectedIndex >= 0)
            {
                lstCharacters.SelectedIndex = -1;
                //Change textbox text to the background's name.
                txtAssetName.Text = VisualNovelViewer.ActiveVisualNovel.ListBackground[lstBackgrounds.SelectedIndex].Name;
            }
        }

        private void txtBackgroundName_TextChanged(object sender, EventArgs e)
        {
            if (lstBackgrounds.SelectedIndex >= 0)
            {//Update the background name.
                VisualNovelViewer.ActiveVisualNovel.ListBackground[lstBackgrounds.SelectedIndex].Name = txtAssetName.Text;
                lstBackgrounds.Items[lstBackgrounds.SelectedIndex] = txtAssetName.Text;
                //Update the dialog combo box name.
                cboBackground.Items[lstBackgrounds.SelectedIndex] = txtAssetName.Text;

                lstDialogs.Update();
            }
        }

        private void btnAddBackground_Click(object sender, EventArgs e)
        {
            List<string> Items = ShowContextMenuWithItem(GUIRootPathVisualNovelBackgrounds);

            if (Items == null)
                return;

            string FullPath = Items[0];
            string Name = Path.GetFileNameWithoutExtension(Items[0]);

            VisualNovelViewer.AddBackground(Name, Name);
            //Update the characters lists.
            lstBackgrounds.Items.Add(Name);
            cboBackground.Items.Add(Name);
        }

        private void btnDeleteBackground_Click(object sender, EventArgs e)
        {
            if (lstBackgrounds.SelectedIndex >= 0)
            {
                int Index = lstBackgrounds.SelectedIndex;
                //Move the current tile set.
                VisualNovelViewer.ActiveVisualNovel.ListBackground.RemoveAt(Index);
                lstBackgrounds.Items.RemoveAt(Index);
                cboBackground.Items.RemoveAt(Index);
                //Replace the index with a new one.
                if (lstBackgrounds.Items.Count > 0)
                {
                    if (Index >= lstBackgrounds.Items.Count)
                        lstBackgrounds.SelectedIndex = lstBackgrounds.Items.Count - 1;
                    else
                        lstBackgrounds.SelectedIndex = Index;
                }
                else
                    txtAssetName.Text = "";
            }
        }

        #endregion

        #endregion

        #region Resize events

        private void ProjectEternityVisualNovelEditor_Resize(object sender, EventArgs e)
        {
            splitContainer1.Size = new Size(this.Width - 16, this.Height - 68);
            VisualNovelViewer.Size = new Size(splitContainer1.Panel1.Width - 3, splitContainer1.Panel1.Height - 1);
        }

        private void ProjectEternityVisualNovelEditor_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            VisualNovelViewer.Size = new Size(splitContainer1.Panel1.Width, splitContainer1.Panel1.Height - 1);
        }

        private void ProjectEternityVisualNovelEditor_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState != LastWindowState)
            {
                LastWindowState = this.WindowState;
            }
        }

        #endregion

        private void ProjectEternityVisualNovelEditor_Shown(object sender, EventArgs e)
        {
            tmrAnimation.Enabled = true;
            Scripts = CutsceneScriptHolder.LoadAllScripts();
            DialogEditor = new DialogEditor();
            DialogEditor.InitHeadless();
            FlowchartEditor = new FlowchartEditor();
            FlowchartEditor.Init(VisualNovelViewer.ActiveVisualNovel);
            FlowchartEditor.VisibleChanged += (ScriptingSender, ScriptingEvent) => cbShowFlowchart.Checked = FlowchartEditor.Visible;
            FlowchartEditor.OnDialogSelect = (SelectedDialog) => lstDialogs.SelectedItem = SelectedDialog;
        }

        private void cbShowFlowchart_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowFlowchart.Checked)
            {
                FlowchartEditor.StartPosition = FormStartPosition.Manual;
                if (Location.X + Width / 2 > Screen.PrimaryScreen.Bounds.Width / 2)
                {
                    FlowchartEditor.Location = new Point(Location.X - FlowchartEditor.Width, Location.Y);
                }
                else
                {
                    FlowchartEditor.Location = new Point(Location.X + Width, Location.Y);
                }
                FlowchartEditor.Show();
            }
            else
            {
                FlowchartEditor.Hide();
            }
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            VisualNovelViewer.ActiveVisualNovel.Update(new Microsoft.Xna.Framework.GameTime());
        }
    }
}
