using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class ProjectEternityVisualNovelEditor : BaseEditor
    {
        public delegate void InitScriptDelegate(CutsceneScript NewScript);

        private InitScriptDelegate InitScript;

        private FormWindowState LastWindowState;
        
        private int LastTimelineIndex;

        private CheckBox cbDrawScripts;
        private DialogEditor DialogEditor;
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
            cbDrawScripts = new CheckBox();
            cbDrawScripts.Text = "Show Scripts";
            cbDrawScripts.AutoSize = false;
            //Link a CheckedChanged event to a method.
            cbDrawScripts.CheckedChanged += new EventHandler(cbDrawScripts_CheckedChanged);
            cbDrawScripts.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbDrawScripts.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost tsmDrawScripts = new ToolStripControlHost(cbDrawScripts);
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

            for (int D = 0; D < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; D++)
            {
                lstDialogs.Items.Add(" -  - ");
            }

            for (int D = 0; D < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; D++)
            {
                UpdatelstTimelineText(D);
            }
        }

        private bool GetRealIndex(Dialog StartingDialog, int DialogIndex, ref int RealIndex)
        {
            if (StartingDialog == null)
            {
                foreach (Dialog ActiveTimelineDialog in VisualNovelViewer.ActiveVisualNovel.Timeline)
                {
                    if (GetRealIndex(ActiveTimelineDialog, DialogIndex, ref RealIndex))
                    {
                        return true;
                    }

                    ++RealIndex;
                }
            }
            else
            {
                if (RealIndex == DialogIndex)
                {
                    RealIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(StartingDialog);
                    return true;
                }

                foreach (int FollowingDialogIndex in StartingDialog.ListNextDialog)
                {
                    ++RealIndex;

                    if (RealIndex == DialogIndex)
                    {
                        RealIndex = FollowingDialogIndex;
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdatelstTimelineText(int Index)
        {
            int RealIndex = 0;
            GetRealIndex(null, Index, ref RealIndex);

            string LeftCharacterText = "";
            string RightCharacterText = "";
            string BackgroundText = "";
            //Add a * before the LeftCharacter name to show it's the one selected.
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Left)
                LeftCharacterText += "*";
            //Add its name.
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].LeftCharacter != null)
                LeftCharacterText += VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].LeftCharacter.Name;

            //Add a * before the RightCharacter name to show it's the one selected.
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Right)
                RightCharacterText += "*";
            //Add its name.
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].RightCharacter != null)
                RightCharacterText += VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].RightCharacter.Name;

            //Add the Background name.
            if (VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].Back != null)
                BackgroundText += VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].Back.Name;
            //Set the final text in the lstDialogs.
            lstDialogs.Items[Index] = (RealIndex + 1) + " - " + LeftCharacterText + " - " + RightCharacterText + " - " + BackgroundText + VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex].Text;

        }

        //Sort the Dialogs in the timeline.
        private void UpdateTimeline()
        {
            VisualNovelViewer.ActiveVisualNovel.Timeline.Sort(delegate(Dialog d1, Dialog d2) { return d1.Position.Y.CompareTo(d2.Position.Y); });
        }

        private bool CheckScriptCollisionWithMouse(int MouseX, int MouseY, int ScriptX, int ScriptY)
        {
            if (MouseX >= ScriptX && MouseX <= ScriptX + VisualNovelViewer.imgScript.Width && MouseY >= ScriptY && MouseY <= ScriptY + VisualNovelViewer.imgScript.Height)
                return true;
            return false;
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
            if (cbDrawScripts.Checked)
            {
                if (VisualNovelViewer.ListDialogSelected.Count > 0)
                {
                    DialogEditor.SetDialog(VisualNovelViewer.ListDialogSelected[0]);
                    DialogEditor.ShowDialog();
                }
            }
            else if (lstDialogs.SelectedIndex >= 0)
            {
                DialogEditor.SetDialog(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex]);
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

        private void lstTimeline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0 && LastTimelineIndex != lstDialogs.SelectedIndex)
            {
                int RealIndex = 0;
                GetRealIndex(null, lstDialogs.SelectedIndex, ref RealIndex);
                //Update the combo box with the new Dialog data.
                DialogSelected(VisualNovelViewer.ActiveVisualNovel.ListDialog[RealIndex]);
            }
        }

        private void cboBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                if (cboBackground.SelectedIndex > 0)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Back = VisualNovelViewer.ActiveVisualNovel.ListBackground[cboBackground.SelectedIndex - 1];
                else
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Back = null;
                UpdatelstTimelineText(lstDialogs.SelectedIndex);
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
                NewDialog.Position = new Microsoft.Xna.Framework.Point(0, VisualNovelViewer.ActiveVisualNovel.Timeline.Last().Position.Y + VisualNovelViewer.imgScript.Height);
            
            NewDialog.CutsceneBefore = new Cutscene(null, Scripts);
            NewDialog.CutsceneAfter = new Cutscene(null, Scripts);

            //Add it to the Lists
            VisualNovelViewer.ActiveVisualNovel.Timeline.Add(NewDialog);
            VisualNovelViewer.ActiveVisualNovel.ListDialog.Add(NewDialog);
            lstDialogs.Items.Add(" -  - ");
            lstDialogs.SelectedIndex = lstDialogs.Items.Count - 1;

            //Update its text in the lists.
            UpdatelstTimelineText(VisualNovelViewer.ActiveVisualNovel.ListDialog.Count - 1);
        }

        private void btnInsertFrame_Click(object sender, EventArgs e)
        {//If there is something selected.
            if (lstDialogs.SelectedIndex >= 0)
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
                    NewDialog.Position = new Microsoft.Xna.Framework.Point(0, VisualNovelViewer.ActiveVisualNovel.Timeline.Last().Position.Y + VisualNovelViewer.imgScript.Height);

                NewDialog.CutsceneBefore = new Cutscene(null, Scripts);
                NewDialog.CutsceneAfter = new Cutscene(null, Scripts);

                //Add it to the Lists
                VisualNovelViewer.ActiveVisualNovel.Timeline.Add(NewDialog);
                VisualNovelViewer.ActiveVisualNovel.ListDialog.Insert(InsertIndex, NewDialog);
                lstDialogs.Items.Insert(InsertIndex, " -  - ");

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
                UpdatelstTimelineText(InsertIndex);
            }
            else//Just add it normally.
            {
                btnAddFrame_Click(sender, e);
            }
        }

        private void btnDeleteFrame_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                int Index = lstDialogs.SelectedIndex;
                int IndexScript = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex]);
                Dialog RemovedDialog = VisualNovelViewer.ActiveVisualNovel.ListDialog.ElementAt(IndexScript);
                //Move the current tile set.
                if (VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(RemovedDialog))
                    VisualNovelViewer.ActiveVisualNovel.Timeline.Remove(RemovedDialog);
                VisualNovelViewer.ActiveVisualNovel.ListDialog.RemoveAt(Index);
                lstDialogs.Items.RemoveAt(Index);

                VisualNovelViewer.ActiveVisualNovel.ListDialog.Remove(RemovedDialog);
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
            if (lstDialogs.SelectedIndex >= 0)
            {
                CheckBox CheckboxSender = (CheckBox)sender;

                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].OverrideCharacterPriority = CheckboxSender.Checked;

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
            if (lstDialogs.SelectedIndex >= 0)
            {
                CharacterData NewForm = new CharacterData(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].ListSpeakerPriority);
                NewForm.ShowDialog();
            }
        }

        #endregion

        #region Bust Dialog

        private void cboLeftCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
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
            if (lstDialogs.SelectedIndex >= 0)
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
            if (lstDialogs.SelectedIndex >= 0)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboLeftPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboLeftPortrait.SelectedItem;
                }

                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].LeftCharacter = ActivePortrait;

                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void cboRightPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboRightPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboRightPortrait.SelectedItem;
                }

                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].RightCharacter = ActivePortrait;

                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbLeftCharacter_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Left;
                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbRightCharacter_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Right;
                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.None;
                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].ActiveBustCharacterState = Dialog.ActiveBustCharacterStates.Both;
                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void txtText_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Open the text editor.
                new TextEdit(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Text, txtText).ShowDialog();
                tabBustDialog.Focus();
            }
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Update the current Dialog Text with the textbox text.
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Text = txtText.Text;
                txtBottomDialogText.Text = txtText.Text;
            }
        }

        private void txtTextPreview_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Update the current Dialog TextPreview with the textbox text.
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TextPreview = txtTextPreview.Text;
                txtBottomDialogTextPreview.Text = txtTextPreview.Text;
            }
        }

        #endregion

        #region Top Dialog

        private void cboTopCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
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
            if (lstDialogs.SelectedIndex >= 0)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboTopCharacterPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboTopCharacterPortrait.SelectedItem;
                }

                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TopCharacter = ActivePortrait;

                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbTopDialogVisibleState_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                if (rbTopDialogVisible.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TopPortaitVisibleState = Dialog.PortaitVisibleStates.Visible;
                else if (rbTopDialogGreyed.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TopPortaitVisibleState = Dialog.PortaitVisibleStates.Greyed;
                else if (rbTopDialogInvisible.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TopPortaitVisibleState = Dialog.PortaitVisibleStates.Invisible;
            }
        }

        private void txtTopDialogText_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Open the text editor.
                new TextEdit(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Text, txtTopDialogText).ShowDialog();
                tabTopDialog.Focus();
            }
        }

        private void txtTopDialogText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Update the current Dialog Text with the textbox text.
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TextTop = txtTopDialogText.Text;
            }
        }

        #endregion

        #region Bottom Dialog

        private void cboBottomCharacterPortrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
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
            if (lstDialogs.SelectedIndex >= 0)
            {
                SimpleAnimation ActivePortrait = null;

                if (cboBottomCharacterPortrait.SelectedIndex > 0)
                {
                    ActivePortrait = (SimpleAnimation)cboBottomCharacterPortrait.SelectedItem;
                }

                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].BottomCharacter = ActivePortrait;

                UpdatelstTimelineText(lstDialogs.SelectedIndex);
            }
        }

        private void rbBottomDialogVisibleState_CheckedChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {
                if (rbBottomDialogVisible.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Visible;
                else if (rbBottomDialogGreyed.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Greyed;
                else if (rbBottomDialogInvisible.Checked)
                    VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].BottomPortaitVisibleState = Dialog.PortaitVisibleStates.Invisible;
            }
        }

        private void txtBottomDialogText_Click(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Open the text editor.
                new TextEdit(VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Text, txtBottomDialogText).ShowDialog();
                tabBottomDialog.Focus();
            }
        }

        private void txtBottomDialogText_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Update the current Dialog Text with the textbox text.
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].Text = txtBottomDialogText.Text;
                txtText.Text = txtBottomDialogText.Text;
            }
        }

        private void txtBottomDialogTextPreview_TextChanged(object sender, EventArgs e)
        {
            if (lstDialogs.SelectedIndex >= 0)
            {//Update the current Dialog TextPreview with the textbox text.
                VisualNovelViewer.ActiveVisualNovel.ListDialog[lstDialogs.SelectedIndex].TextPreview = txtBottomDialogTextPreview.Text;
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

                //Update the dialogs background name if they were using the modified background.
                for (int D = 0; D < VisualNovelViewer.ActiveVisualNovel.Timeline.Count; D++)
                {
                    if (VisualNovelViewer.ActiveVisualNovel.Timeline[D].Back == VisualNovelViewer.ActiveVisualNovel.ListBackground[lstBackgrounds.SelectedIndex])
                        UpdatelstTimelineText(D);
                }
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

        //End of a click, mostly will execute an action.
        private void pbVisualNovelPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (VisualNovelViewer.DrawScripts)
            {
                VisualNovelViewer.MouseX = e.X;
                VisualNovelViewer.MouseY = e.Y;

                Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

                #region Left click

                if (e.Button == MouseButtons.Left)
                {
                    Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                    //Clicked on an object.
                    if (VisualNovelViewer.MovingScripts.Count > 0)
                    {
                        VisualNovelViewer.MovingScripts.Clear();//No need to move scripts at this point, make sure it's cleared.
                    }
                    else
                    {
                        //Lool for a script under the mouse to find if it's a script that you were moving.
                        for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                        {//If linking scripts together and it's not linking itself.
                            #region Linking events

                            if (VisualNovelViewer.ScriptLinkTypeChoice != ScriptLinkType.None)
                            {//Make sure it's not pointing on itself.
                                if (VisualNovelViewer.ScriptLink != S)
                                {
                                    switch (VisualNovelViewer.ScriptLinkTypeChoice)
                                    {
                                        case ScriptLinkType.FromDialog:
                                            //If linking to an other other dialog and the Script is a dialog.
                                            //See if the mouse is over the left linking box of a script.
                                            if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                                && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                                            {//If it's not already linked.
                                                if (!(VisualNovelViewer.ActiveVisualNovel.ListDialog[VisualNovelViewer.ScriptLink]).ListNextDialog.Contains(S))
                                                {//Add the link.
                                                    (VisualNovelViewer.ActiveVisualNovel.ListDialog[VisualNovelViewer.ScriptLink]).ListNextDialog.Add(S);
                                                    //If not holding shift, reset the starting link.
                                                    if (Control.ModifierKeys != Keys.Shift)
                                                    {
                                                        VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                                                        VisualNovelViewer.ScriptLink = -1;
                                                        VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                                                    }
                                                    return;
                                                }
                                            }
                                            break;

                                        //A dialog linked to an other dialog.
                                        case ScriptLinkType.ToDialog:
                                            if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90
                                                && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                            {
                                                //If it's not already linked.
                                                if (!(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]).ListNextDialog.Contains(VisualNovelViewer.ScriptLink))
                                                {//Add the link.
                                                    (VisualNovelViewer.ActiveVisualNovel.ListDialog[S]).ListNextDialog.Add(VisualNovelViewer.ScriptLink);
                                                    //If not holding shift, reset the starting link.
                                                    if (Control.ModifierKeys != Keys.Shift)
                                                    {
                                                        VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                                                        VisualNovelViewer.ScriptLink = -1;
                                                        VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                                                    }
                                                    return;
                                                }
                                            }
                                            break;
                                    }
                                }
                                //If it's pointing at itself.
                                else if (VisualNovelViewer.ScriptLink == S)
                                {
                                    //See if the mouse is over a linking box.
                                    if ((MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                            && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                                        || ((MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90)
                                            && ((MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                            || MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 40 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 47)))
                                    {//Don't reset the ScriptLinking.(So you won't clear the ScriptLinking if you click on the linking box instead of draging it)
                                        return;
                                    }
                                }
                            }

                            #endregion

                            //Found one
                            if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y) && VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                            {
                                return;
                            }
                        }
                        //If not holding shift, reset the starting link.
                        if (Control.ModifierKeys != Keys.Shift)
                        {
                            VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                            VisualNovelViewer.ScriptLink = -1;
                            VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                            VisualNovelViewer.ListDialogSelected.Clear();//If you reached this place, you weren't moving something so unselect everything.
                        }
                    }
                }

                #endregion

                #region Right click

                //Right click and not moving scripts and not linking scripts.
                else if (e.Button == MouseButtons.Right && VisualNovelViewer.MovingScripts.Count == 0 && VisualNovelViewer.ScriptLinkTypeChoice == ScriptLinkType.None)
                {
                    //Lool for a script under the mouse to find an object that you were moving.
                    for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                    {
                        Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                        //Script detected, open the context menu.
                        if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                        {//Nothing selected, show New, Edit and delete.
                            if (VisualNovelViewer.ListDialogSelected.Count == 0)
                            {
                                VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                tsmEdit.Visible = true;
                                tsmDelete.Visible = true;
                            }//One thing selected, Either it's the one we want or not, it's the new selected object, show New, Edit and delete.
                            else if (VisualNovelViewer.ListDialogSelected.Count == 1)
                            {
                                VisualNovelViewer.ListDialogSelected.Clear();
                                VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                tsmEdit.Visible = true;
                                tsmDelete.Visible = true;
                            }
                            else//Multiple Dialogs selected.
                            {//If it's already selected, just show New and Delete. (Can't edit multiple Dialogs)
                                if (VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                                {
                                    tsmEdit.Visible = false;
                                    tsmDelete.Visible = true;
                                }
                                else
                                {//It's the new selected object, show New, Edit and delete.
                                    VisualNovelViewer.ListDialogSelected.Clear();
                                    VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                    tsmEdit.Visible = true;
                                    tsmDelete.Visible = true;
                                }
                            }
                            //Open the context menu.
                            cmsScriptEditor.Show(VisualNovelViewer, e.Location);
                            return;
                        }
                        else
                        {
                            #region Linking boxes

                            //Right side linking box.
                            if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90)
                            {//Script box link.
                                if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 40 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 47)
                                {
                                    VisualNovelViewer.ActiveVisualNovel.ListDialog[S].ListNextDialog.Clear();
                                    return;
                                }
                                else
                                {
                                    //Dialog box link.
                                    if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                    {
                                        VisualNovelViewer.ActiveVisualNovel.ListDialog[S].ListNextDialog.Clear();
                                        return;
                                    }
                                }
                            }
                            //Left side linking box.
                            else if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                            {
                                //Loop in the ListDialog to find a Dialog linked to it.
                                for (int D = 0; D < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; D++)
                                {
                                    //If the Dialog is linked to the selected Dialog.
                                    if ((VisualNovelViewer.ActiveVisualNovel.ListDialog[D]).ListNextDialog.Contains(S))
                                    {//Remove the link.
                                        (VisualNovelViewer.ActiveVisualNovel.ListDialog[D]).ListNextDialog.Remove(S);
                                    }
                                }
                                return;
                            }

                            #endregion
                        }
                    }
                    //No linking box detected.
                    tsmEdit.Visible = false;
                    tsmDelete.Visible = false;
                    cmsScriptEditor.Show(VisualNovelViewer, e.Location);
                }

                #endregion
            }
        }

        private void pbVisualNovelPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (VisualNovelViewer.DrawScripts)
            {
                int MouseOldX = VisualNovelViewer.MouseX;
                int MouseOldY = VisualNovelViewer.MouseY;
                VisualNovelViewer.MouseX = e.X;
                VisualNovelViewer.MouseY = e.Y;

                Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

                if (e.Button == MouseButtons.Left)
                {//Not currently moving something
                    if (VisualNovelViewer.MovingScripts.Count == 0)
                    {//If there is a line between a script and the mouse, draw with the mouse position so it draw at the right place.
                        if (VisualNovelViewer.ScriptLinkTypeChoice != ScriptLinkType.None)
                        {
                        }

                        #region Select Scripts

                        else if (Control.ModifierKeys == Keys.Shift)
                        {
                            Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                            //Lool for a script under the mouse.
                            for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                            {//Found one
                                if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                                {//If it's not already in the list of selected scripts.
                                    if (!VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                                    {
                                        VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                    }
                                    break;
                                }
                            }
                        }
                        else if (Control.ModifierKeys == Keys.Alt)
                        {
                            ScriptEditorOrigin.X = Math.Max(0, ScriptEditorOrigin.X - e.X - MouseOldX);
                            ScriptEditorOrigin.Y -= e.Y - MouseOldY;
                            VisualNovelViewer.ScriptEditorOrigin = ScriptEditorOrigin;
                        }

                        #endregion
                    }
                    else
                    {
                        #region Move Scripts

                        //Move the selected scripts.
                        for (int S = 0; S < VisualNovelViewer.ListDialogSelected.Count; S++)
                        {
                            #region X Movement

                            //If it's close enough to be inside the Timeline.
                            if (e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X < VisualNovelViewer.imgScript.Width / 2)
                            {
                                VisualNovelViewer.ListDialogSelected[S].Position.X = 0;
                                //If it's not already in the Timeline, add it.
                                if (!VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(VisualNovelViewer.ListDialogSelected[S]))
                                    VisualNovelViewer.ActiveVisualNovel.Timeline.Add((Dialog)VisualNovelViewer.ListDialogSelected[S]);
                            }
                            else
                            {//If it's near the border of the Timeline but not close enough to be inside.
                                if (e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X < VisualNovelViewer.imgScript.Width)
                                    VisualNovelViewer.ListDialogSelected[S].Position.X = VisualNovelViewer.imgScript.Width;//Put it next to the timeline.
                                else
                                {//Move it normally.
                                    VisualNovelViewer.ListDialogSelected[S].Position.X = e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X;
                                }
                                if (VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(VisualNovelViewer.ListDialogSelected[S]))
                                    VisualNovelViewer.ActiveVisualNovel.Timeline.Remove((Dialog)VisualNovelViewer.ListDialogSelected[S]);
                            }

                            #endregion

                            #region Y Movement

                            //If in the Timeline.
                            if (VisualNovelViewer.ListDialogSelected[S].Position.X == 0)
                            {//Snap it to a grid of the size of the imgScript.
                                VisualNovelViewer.ListDialogSelected[S].Position.Y = ((e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y) / VisualNovelViewer.imgScript.Height) * VisualNovelViewer.imgScript.Height;
                                UpdateTimeline();
                            }//If it's under 0, snap it to 0.
                            else if (e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y < 0)
                                VisualNovelViewer.ListDialogSelected[S].Position.Y = 0;
                            else
                            {//Move it normally.
                                VisualNovelViewer.ListDialogSelected[S].Position.Y = e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y;
                            }

                            #endregion
                        }

                        #endregion
                    }
                }
                else if (e.Button == MouseButtons.Right)
                { }
            }
        }

        //Called when you first hold a mouse bouton down.
        private void pbVisualNovelPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (VisualNovelViewer.DrawScripts)
            {
                VisualNovelViewer.MouseX = e.X;
                VisualNovelViewer.MouseY = e.Y;

                Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

                if (e.Button == MouseButtons.Left)
                {
                    Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                    //Lool for a script under the mouse.
                    for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                    {//Found one
                        if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                        {//If it's not already in the list of selected scripts.
                            if (!VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                            {
                                if (Control.ModifierKeys != Keys.Shift)
                                    VisualNovelViewer.ListDialogSelected.Clear();

                                VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                lstDialogs.SelectedIndex = S;
                            }
                            if (Control.ModifierKeys != Keys.Shift)
                            {
                                for (int M = 0; M < VisualNovelViewer.ListDialogSelected.Count; M++)
                                {
                                    VisualNovelViewer.MovingScripts.Add(new Microsoft.Xna.Framework.Point(MousePos.X - VisualNovelViewer.ListDialogSelected[M].Position.X, MousePos.Y - VisualNovelViewer.ListDialogSelected[M].Position.Y));
                                }
                            }
                            return;//Stop here so it won't execute the rest.
                        }
                        else
                        {
                            if (VisualNovelViewer.ScriptLinkTypeChoice == ScriptLinkType.None)
                            {//Right side linking box.
                                if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90)
                                {
                                    //Dialog box.
                                    if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                    {
                                        VisualNovelViewer.ScriptLinkStartingPoint = new Microsoft.Xna.Framework.Point(VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 86, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 31);
                                        VisualNovelViewer.ScriptLink = S;
                                        VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.FromDialog;
                                    }
                                }
                                //Left side linking box.
                                else if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                    && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                                {
                                    VisualNovelViewer.ScriptLinkStartingPoint = new Microsoft.Xna.Framework.Point(VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 7, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 5);
                                    VisualNovelViewer.ScriptLink = S;
                                    VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.ToDialog;
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Tool Strip Menu

        private void tmsNewDialog_Click(object sender, EventArgs e)
        {
            btnAddFrame_Click(sender, e);
        }

        private void tsmEdit_Click(object sender, EventArgs e)
        {
            if (VisualNovelViewer.ListDialogSelected.Count == 1)
            {
                lstDialogs.SelectedIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(VisualNovelViewer.ListDialogSelected[0]);
            }
        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {
            for (int S = 0; S < VisualNovelViewer.ListDialogSelected.Count; S++)
            {//Select the dialog.
                lstDialogs.SelectedIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(VisualNovelViewer.ListDialogSelected[S]);
                btnDeleteFrame_Click(sender, e);
            }
        }

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
        }

        private void cbDrawScripts_CheckedChanged(object sender, EventArgs e)
        {
            VisualNovelViewer.DrawScripts = cbDrawScripts.Checked;
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            VisualNovelViewer.ActiveVisualNovel.Update(new Microsoft.Xna.Framework.GameTime());
        }
    }
}
