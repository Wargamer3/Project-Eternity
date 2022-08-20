using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    /// <summary>
    /// Represent a basic page in a visual novel.
    /// </summary>
    public class VisualNovel : GameScreen
    {
        public delegate void VisualNovelFrameChanged();

        public delegate void VisualNovelPaused();

        public delegate void VisualNovelResumed();

        public delegate void VisualNovelEnded();

        public List<VisualNovelCharacter> ListCharacter;
        public List<SimpleAnimation> ListBustPortrait;
        public List<SimpleAnimation> ListPortrait;
        public List<VisualNovelBackground> ListBackground;
        public List<Dialog> ListDialog;//Used for the dialog editor and localization files.
        public List<Dialog> Timeline;//Used for the VN sumary, it contains the first script on a specific point in the timeline. (Aka, the Frames)
        public int TimelineIndex;
        private int TimelineIndexMax;
        public int DialogChoice;//Choice to take in the dialog.
        public int DialogChoiceMinIndex;
        private int MaxDialogChoice;

        public Dialog CurrentDialog;
        public Dialog LastDialog;

        #region Getters for the CurrentDialog.

        private VisualNovelBackground CurrentBackground
        {
            get
            {
                if (CurrentDialog != null)
                    return CurrentDialog.Back;
                else
                    return null;
            }
        }

        private SimpleAnimation LeftCharacter
        {
            get
            {
                if (CurrentDialog != null)
                    return CurrentDialog.LeftCharacter;
                else
                    return null;
            }
        }

        private SimpleAnimation RightCharacter
        {
            get
            {
                if (CurrentDialog != null)
                    return CurrentDialog.RightCharacter;
                else
                    return null;
            }
        }

        private Dialog.ActiveBustCharacterStates ActiveChar
        {
            get
            {
                if (CurrentDialog != null)
                    return CurrentDialog.ActiveBustCharacterState;
                else
                    return Dialog.ActiveBustCharacterStates.None;
            }
        }

        #endregion

        private SpriteFont fntMultiDialogFont;
        private SpriteFont fntFinlanderFont;

        public bool IsPaused = false;
        public bool UseLocalization = true;
        public Dictionary<string, int> DicMapVariables;

        public VisualNovelFrameChanged OnVisualNovelFrameChanged;
        public VisualNovelPaused OnVisualNovelPaused;
        public VisualNovelResumed OnVisualNovelResumed;
        public VisualNovelEnded OnVisualNovelEnded;

        public string VisualNovelPath;
        private readonly Dictionary<string, CutsceneScript> DicCutsceneScript;

        private Vector2 LeftPosition;
        private Vector2 RightPosition;

        public const int VNBoxHeight = 128;

        private BoxScrollbar ChoicesScrollbar;

        private VisualNovel()
            : base()
        {
            ListCharacter = new List<VisualNovelCharacter>();
            ListBustPortrait = new List<SimpleAnimation>();
            ListPortrait = new List<SimpleAnimation>();
            ListBackground = new List<VisualNovelBackground>();
            ListDialog = new List<Dialog>();
            Timeline = new List<Dialog>();
            TimelineIndex = 0;
            TimelineIndexMax = 0;
            DialogChoice = 0;
            DicMapVariables = new Dictionary<string, int>();
        }

        public VisualNovel(string VisualNovelPath)
            : this()
        {
            this.VisualNovelPath = VisualNovelPath;
        }

        public VisualNovel(string VisualNovelPath, Dictionary<string, CutsceneScript> DicCutsceneScript)
            : this(VisualNovelPath)
        {
            this.DicCutsceneScript = DicCutsceneScript;
        }

        public override void Load()
        {
            Dictionary<string, CutsceneScript> DicCutsceneScript = this.DicCutsceneScript;
            if (DicCutsceneScript == null)
            {
                DicCutsceneScript = CutsceneScriptHolder.LoadAllScripts();
            }

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            fntMultiDialogFont = Content.Load<SpriteFont>("Fonts/VisualNovelMultiDialogFont");

            ChoicesScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, Constants.Height - VNBoxHeight), VNBoxHeight, 4, OnScrollbarChange);

            LeftPosition = new Vector2(0, Constants.Height - VNBoxHeight);
            RightPosition = new Vector2(Constants.Width, Constants.Height - VNBoxHeight);

            if (VisualNovelPath != null)
            {
                FileStream FS = new FileStream("Content/Visual Novels/" + VisualNovelPath + ".pevn", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                #region Assets

                int CharacterCount = BR.ReadInt32();
                for (int C = 0; C < CharacterCount; C++)
                {
                    ListCharacter.Add(new VisualNovelCharacter(BR));
                }

                int BustCharacterCount = BR.ReadInt32();
                for (int i = 0; i < BustCharacterCount; i++)
                {
                    ListBustPortrait.Add(new SimpleAnimation(BR, Content, "Visual Novels/Bust Portraits/"));
                }

                int PortraitCharacterCount = BR.ReadInt32();
                for (int i = 0; i < PortraitCharacterCount; i++)
                {
                    ListPortrait.Add(new SimpleAnimation(BR, Content, "Visual Novels/Portraits/"));
                }

                int BackgroundCount = BR.ReadInt32();//Number of backgrounds.
                for (int i = 0; i < BackgroundCount; i++)
                {
                    string Name = BR.ReadString();
                    string Path = BR.ReadString();
                    ListBackground.Add(new VisualNovelBackground(Name, Path, Content.Load<Texture2D>("Visual Novels/Backgrounds/" + Path)));
                }

                #endregion

                #region Dialog

                int ListDialogCount = BR.ReadInt32();//Number of script.
                for (int D = 0; D < ListDialogCount; D++)
                {
                    //Base info.
                    int PositionX = BR.ReadInt32();
                    int PositionY = BR.ReadInt32();

                    //Get the index of the assets used.
                    int LeftCharacterIndex = BR.ReadInt32();
                    int LeftCharacterPortraitIndex = BR.ReadInt32();
                    int RightCharacterIndex = BR.ReadInt32();
                    int RightCharacterPortraitIndex = BR.ReadInt32();
                    int TopCharacterIndex = BR.ReadInt32();
                    int TopCharacterPortraitIndex = BR.ReadInt32();
                    int BottomCharacterIndex = BR.ReadInt32();
                    int BottomCharacterPortraitIndex = BR.ReadInt32();
                    int BackgroundIndex = BR.ReadInt32();

                    //Init the assets.

                    #region Left Character

                    SimpleAnimation LeftCharacter;
                    if (LeftCharacterIndex > 0)
                    {
                        if (LeftCharacterPortraitIndex <= ListCharacter[LeftCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath.Length)
                        {
                            string ActivePortraitName = ListCharacter[LeftCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath[LeftCharacterPortraitIndex - 1];
                            string PortraitType = "Bust Portraits";
                            string FullName = PortraitType + "/" + ActivePortraitName;

                            LeftCharacter = new SimpleAnimation(ActivePortraitName, ActivePortraitName, Content.Load<Texture2D>("Visual Novels/" + FullName));
                        }
                        else
                        {
                            LeftCharacter = new SimpleAnimation(ListBustPortrait[LeftCharacterPortraitIndex - ListCharacter[LeftCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath.Length - 1]);
                        }
                    }
                    else if (LeftCharacterIndex == 0 && LeftCharacterPortraitIndex > 0)
                    {
                        LeftCharacter = new SimpleAnimation(ListBustPortrait[LeftCharacterPortraitIndex - 1]);
                    }
                    else
                    {
                        LeftCharacter = null;
                    }

                    if (LeftCharacter != null)
                    {
                        if (LeftCharacter.IsAnimated)
                        {
                            LeftCharacter = new SimpleAnimation(LeftCharacter.Name, LeftCharacter.Path, new AnimationLooped(LeftCharacter.Path));
                            LeftCharacter.Load(Content, "");
                        }

                        if (LeftCharacterIndex > 0)
                        {
                            LeftCharacter.Path = ListCharacter[LeftCharacterIndex - 1].LoadedCharacter.FullName;
                        }
                    }

                    #endregion

                    #region Right Character

                    SimpleAnimation RightCharacter;
                    if (RightCharacterIndex > 0)
                    {
                        if (RightCharacterPortraitIndex <= ListCharacter[RightCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath.Length)
                        {
                            string ActivePortraitName = ListCharacter[RightCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath[RightCharacterPortraitIndex - 1];
                            string PortraitType = "Bust Portraits";
                            string FullName = PortraitType + "/" + ActivePortraitName;

                            RightCharacter = new SimpleAnimation(ActivePortraitName, ActivePortraitName, Content.Load<Texture2D>("Visual Novels/" + FullName));
                        }
                        else
                        {
                            RightCharacter = new SimpleAnimation(ListBustPortrait[RightCharacterPortraitIndex - ListCharacter[RightCharacterIndex - 1].LoadedCharacter.ArrayPortraitBustPath.Length - 1]);
                        }
                    }
                    else if (RightCharacterIndex == 0 && RightCharacterPortraitIndex > 0)
                    {
                        RightCharacter = new SimpleAnimation(ListBustPortrait[RightCharacterPortraitIndex - 1]);
                    }
                    else
                    {
                        RightCharacter = null;
                    }

                    if (RightCharacter != null)
                    {
                        if (RightCharacter.IsAnimated)
                        {
                            RightCharacter = new SimpleAnimation(RightCharacter.Name, RightCharacter.Path, new AnimationLooped(RightCharacter.Path));
                            RightCharacter.Load(Content, "");
                        }

                        if (RightCharacterIndex > 0)
                        {
                            RightCharacter.Path = ListCharacter[RightCharacterIndex - 1].LoadedCharacter.FullName;
                        }
                    }

                    #endregion

                    #region Top Character

                    SimpleAnimation TopCharacter;

                    if (TopCharacterIndex > 0)
                    {
                        if (TopCharacterPortraitIndex <= ListCharacter[TopCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath.Length)
                        {
                            string ActivePortraitName = ListCharacter[TopCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath[TopCharacterPortraitIndex - 1];
                            string PortraitType = "Portraits";
                            string FullName = PortraitType + "/" + ActivePortraitName;

                            TopCharacter = new SimpleAnimation(ActivePortraitName, ActivePortraitName, Content.Load<Texture2D>("Visual Novels/" + FullName));
                        }
                        else
                        {
                            TopCharacter = new SimpleAnimation(ListPortrait[TopCharacterPortraitIndex - ListCharacter[TopCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath.Length - 1]);
                        }
                    }
                    else if (TopCharacterIndex == 0 && TopCharacterPortraitIndex > 0)
                    {
                        TopCharacter = new SimpleAnimation(ListPortrait[TopCharacterPortraitIndex - 1]);
                    }
                    else
                    {
                        TopCharacter = null;
                    }

                    if (TopCharacter != null)
                    {
                        if (TopCharacter.IsAnimated)
                        {
                            TopCharacter = new SimpleAnimation(TopCharacter.Name, TopCharacter.Path, new AnimationLooped(TopCharacter.Path));
                            TopCharacter.Load(Content, "");
                        }

                        if (TopCharacterIndex > 0)
                        {
                            TopCharacter.Path = ListCharacter[TopCharacterIndex - 1].LoadedCharacter.FullName;
                        }
                    }

                    #endregion

                    #region Bottom Character

                    SimpleAnimation BottomCharacter;

                    if (BottomCharacterIndex > 0)
                    {
                        if (BottomCharacterPortraitIndex <= ListCharacter[BottomCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath.Length)
                        {
                            string ActivePortraitName = ListCharacter[BottomCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath[BottomCharacterPortraitIndex - 1];
                            string PortraitType = "Portraits";
                            string FullName = PortraitType + "/" + ActivePortraitName;

                            BottomCharacter = new SimpleAnimation(ActivePortraitName, ActivePortraitName, Content.Load<Texture2D>("Visual Novels/" + FullName));
                        }
                        else
                        {
                            BottomCharacter = new SimpleAnimation(ListPortrait[BottomCharacterPortraitIndex - ListCharacter[BottomCharacterIndex - 1].LoadedCharacter.ArrayPortraitBoxPath.Length - 1]);
                        }
                    }
                    else if (BottomCharacterIndex == 0 && BottomCharacterPortraitIndex > 0)
                    {
                        BottomCharacter = new SimpleAnimation(ListPortrait[BottomCharacterPortraitIndex - 1]);
                    }
                    else
                    {
                        BottomCharacter = null;
                    }

                    if (BottomCharacter != null)
                    {
                        if (BottomCharacter.IsAnimated)
                        {
                            BottomCharacter = new SimpleAnimation(BottomCharacter.Name, BottomCharacter.Path, new AnimationLooped(BottomCharacter.Path));
                            BottomCharacter.Load(Content, "");
                        }

                        if (BottomCharacterIndex > 0)
                        {
                            BottomCharacter.Path = ListCharacter[BottomCharacterIndex - 1].LoadedCharacter.FullName;
                        }
                    }

                    #endregion

                    VisualNovelBackground Back;
                    if (BackgroundIndex > 0)
                        Back = ListBackground[BackgroundIndex - 1];
                    else
                        Back = null;

                    int ActiveChar = BR.ReadByte();
                    int ActiveTopChar = BR.ReadByte();
                    int ActiveBottomChar = BR.ReadByte();

                    //Load the texts.
                    string Text = BR.ReadString();
                    string TextPreview = BR.ReadString();
                    string TextTop = BR.ReadString();
                    bool OverrideCharacterPriority = BR.ReadBoolean();

                    //Init the Dialog.
                    Dialog NewDialog = new Dialog(LeftCharacter, RightCharacter, TopCharacter, BottomCharacter, Back, (Dialog.ActiveBustCharacterStates)ActiveChar, Text, TextPreview);
                    NewDialog.TopPortaitVisibleState = (Dialog.PortaitVisibleStates)ActiveTopChar;
                    NewDialog.BottomPortaitVisibleState = (Dialog.PortaitVisibleStates)ActiveBottomChar;
                    NewDialog.TextTop = TextTop;
                    NewDialog.Position = new Point(PositionX, PositionY);

                    NewDialog.OverrideCharacterPriority = OverrideCharacterPriority;
                    if (OverrideCharacterPriority)
                    {
                        int ListSpeakerPriorityCount = BR.ReadInt32();
                        for (int S = 0; S < ListSpeakerPriorityCount; ++S)
                        {
                            NewDialog.ListSpeakerPriority.Add(new SpeakerPriority((SpeakerPriority.PriorityTypes)BR.ReadByte(), BR.ReadString()));
                        }
                    }

                    //Add its linked dialogs.
                    int LinkedDialogsSize = BR.ReadInt32();
                    for (int L = 0; L < LinkedDialogsSize; L++)
                    {
                        NewDialog.ListNextDialog.Add(BR.ReadInt32());
                    }
                    ListDialog.Add(NewDialog);

                    //Check if it's a Timeline Dialog.
                    if (PositionX == 0)
                        Timeline.Add(NewDialog);

                    NewDialog.CutsceneBefore = new Cutscene(null, BR, DicCutsceneScript);
                    NewDialog.CutsceneAfter = new Cutscene(null, BR, DicCutsceneScript);
                }

                #endregion

                BR.Close();
                FS.Close();
            }
            if (Timeline.Count > 0)
            {
                Timeline.Sort(delegate(Dialog d1, Dialog d2) { return d1.Position.Y.CompareTo(d2.Position.Y); });
                CurrentDialog = Timeline[0];
                UpdateTextChoices();

                if (CurrentDialog.CutsceneBefore != null)
                    PushScreen(CurrentDialog.CutsceneBefore);
            }

            if (UseLocalization)
                ReadLocalizationFile();
        }

        public void Save()
        {
            FileStream FS = new FileStream("Content/Visual Novels/" + VisualNovelPath + ".pevn", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            #region Assets

            BW.Write(ListCharacter.Count);
            for (int C = 0; C < ListCharacter.Count; C++)
            {
                ListCharacter[C].Save(BW);
            }

            BW.Write(ListBustPortrait.Count);
            for (int i = 0; i < ListBustPortrait.Count; i++)
            {
                BW.Write(ListBustPortrait[i].IsAnimated);
                BW.Write(ListBustPortrait[i].Name);
                BW.Write(ListBustPortrait[i].Path);
            }

            BW.Write(ListPortrait.Count);
            for (int i = 0; i < ListPortrait.Count; i++)
            {
                BW.Write(ListPortrait[i].IsAnimated);
                BW.Write(ListPortrait[i].Name);
                BW.Write(ListPortrait[i].Path);
            }

            BW.Write(ListBackground.Count);//Number of backgrounds.
            for (int i = 0; i < ListBackground.Count; i++)
            {
                BW.Write(ListBackground[i].Name);
                BW.Write(ListBackground[i].Path);
            }

            #endregion

            BW.Write(ListDialog.Count);//Number of Dialogs.
            for (int D = 0; D < ListDialog.Count; D++)
            {
                //Basic Script info.
                BW.Write(ListDialog[D].Position.X);
                BW.Write(ListDialog[D].Position.Y);

                Dialog SavedDialog = ListDialog[D];//Used so no casting will be needed later.

                int CharacterIndex;
                int PortraitIndex;

                GetBustPortraitIndices(SavedDialog.LeftCharacter, out CharacterIndex, out PortraitIndex);

                BW.Write(CharacterIndex);
                BW.Write(PortraitIndex);

                GetBustPortraitIndices(SavedDialog.RightCharacter, out CharacterIndex, out PortraitIndex);

                BW.Write(CharacterIndex);
                BW.Write(PortraitIndex);

                GetBoxPortraitIndices(SavedDialog.TopCharacter, out CharacterIndex, out PortraitIndex);

                BW.Write(CharacterIndex);
                BW.Write(PortraitIndex);

                GetBoxPortraitIndices(SavedDialog.BottomCharacter, out CharacterIndex, out PortraitIndex);

                BW.Write(CharacterIndex);
                BW.Write(PortraitIndex);

                if (SavedDialog.Back != null)
                    BW.Write(ListBackground.IndexOf(SavedDialog.Back) + 1);
                else
                    BW.Write(0);

                BW.Write((byte)SavedDialog.ActiveBustCharacterState);
                BW.Write((byte)SavedDialog.TopPortaitVisibleState);
                BW.Write((byte)SavedDialog.BottomPortaitVisibleState);

                //Save its text.
                BW.Write(SavedDialog.Text);
                BW.Write(SavedDialog.TextPreview);
                BW.Write(SavedDialog.TextTop);

                BW.Write(SavedDialog.OverrideCharacterPriority);
                if (SavedDialog.OverrideCharacterPriority)
                {
                    BW.Write(SavedDialog.ListSpeakerPriority.Count);
                    for (int S = 0; S < SavedDialog.ListSpeakerPriority.Count; ++S)
                    {
                        BW.Write((byte)SavedDialog.ListSpeakerPriority[S].PriorityType);
                        BW.Write(SavedDialog.ListSpeakerPriority[S].Name);
                    }
                }

                //Save it's linked dialogs.
                BW.Write(SavedDialog.ListNextDialog.Count);
                for (int L = 0; L < SavedDialog.ListNextDialog.Count; L++)
                    BW.Write(SavedDialog.ListNextDialog[L]);

                SavedDialog.CutsceneBefore.Save(BW);
                SavedDialog.CutsceneAfter.Save(BW);
            }

            BW.Close();
            FS.Close();
        }

        public void GetBustPortraitIndices(SimpleAnimation ActiveCharacterPortrait, out int CharacterIndex, out int PortraitIndex)
        {
            CharacterIndex = 0;
            PortraitIndex = 0;

            if (ActiveCharacterPortrait != null)
            {
                for (int C = 0; C < ListCharacter.Count && CharacterIndex == 0; C++)
                {
                    for (int P = 0; P < ListCharacter[C].LoadedCharacter.ArrayPortraitBustPath.Length; ++P)
                    {
                        if (ListCharacter[C].LoadedCharacter.ArrayPortraitBustPath[P] == ActiveCharacterPortrait.Name)
                        {
                            CharacterIndex = C + 1;
                            PortraitIndex = P + 1;
                            return;
                        }
                    }

                    if (PortraitIndex == 0 && ActiveCharacterPortrait.Path == ListCharacter[C].LoadedCharacter.FullName)
                    {
                        int ExtraPortraitIndex = ListBustPortrait.FindIndex(x => x.Name == ActiveCharacterPortrait.Name);
                        if (ExtraPortraitIndex >= 0)
                        {
                            PortraitIndex = ListCharacter[C].LoadedCharacter.ArrayPortraitBustPath.Length + ExtraPortraitIndex + 1;
                            CharacterIndex = C + 1;
                            return;
                        }
                    }
                }

                PortraitIndex = ListBustPortrait.FindIndex(x => x.Name == ActiveCharacterPortrait.Name);
                if (PortraitIndex < 0)
                {
                    PortraitIndex = ListBustPortrait.IndexOf(ActiveCharacterPortrait);
                }
                ++PortraitIndex;
                CharacterIndex = 0;
            }
        }

        public void GetBoxPortraitIndices(SimpleAnimation ActiveCharacterPortrait, out int CharacterIndex, out int PortraitIndex)
        {
            CharacterIndex = 0;
            PortraitIndex = 0;

            if (ActiveCharacterPortrait != null)
            {
                for (int C = 0; C < ListCharacter.Count && CharacterIndex == 0; C++)
                {
                    for (int P = 0; P < ListCharacter[C].LoadedCharacter.ArrayPortraitBoxPath.Length; ++P)
                    {
                        if (ListCharacter[C].LoadedCharacter.ArrayPortraitBoxPath[P] == ActiveCharacterPortrait.Name)
                        {
                            CharacterIndex = C + 1;
                            PortraitIndex = P + 1;
                            return;
                        }
                    }

                    if (PortraitIndex == 0 && ActiveCharacterPortrait.Path == ListCharacter[C].LoadedCharacter.FullName)
                    {
                        int ExtraPortraitIndex = ListPortrait.FindIndex(x => x.Name == ActiveCharacterPortrait.Name);
                        if (ExtraPortraitIndex >= 0)
                        {
                            PortraitIndex = ListCharacter[C].LoadedCharacter.ArrayPortraitBoxPath.Length + ExtraPortraitIndex + 1;
                            CharacterIndex = C + 1;
                            return;
                        }
                    }
                }

                PortraitIndex = ListPortrait.IndexOf(ActiveCharacterPortrait) + 1;
                CharacterIndex = 0;
            }
        }

        public void ReadLocalizationFile()
        {
            string CompletePath = "Content/Visual Novels/" + VisualNovelPath + "-" + GameLanguage + ".xml";
            if (File.Exists(CompletePath))
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(CompletePath);
                for (int D = 0; D < ListDialog.Count; ++D)
                {
                    XmlNodeList ActiveDialogList = XmlDoc.GetElementsByTagName("Dialog" + D);
                    if (ActiveDialogList.Count == 1 && ActiveDialogList[0].ChildNodes.Count == 3)
                    {
                        XmlNode ActiveDialog = ActiveDialogList[0];

                        string Text = ActiveDialog.ChildNodes[0].Value;
                        string TextPreview = ActiveDialog.ChildNodes[1].Value;
                        string TextTop = ActiveDialog.ChildNodes[2].Value;

                        ListDialog[D].Text = Text;
                        ListDialog[D].TextPreview = TextPreview;
                        ListDialog[D].TextTop = TextTop;
                    }
                }
            }
        }

        public void ExportLocalizationFile(string ExportPath)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlNode RootNode = XmlDoc.CreateElement("Dialogs");
            XmlDoc.AppendChild(RootNode);

            for (int D = 0; D < ListDialog.Count; ++D)
            {
                XmlNode DialogNode = XmlDoc.CreateElement("Dialog" + D);

                XmlNode DialogTextNode = XmlDoc.CreateElement("DialogText");
                DialogTextNode.InnerText = ListDialog[D].Text;
                DialogNode.AppendChild(DialogTextNode);

                XmlNode DialogTextPreviewNode = XmlDoc.CreateElement("DialogTextPreview");
                DialogTextPreviewNode.InnerText = ListDialog[D].TextPreview;
                DialogNode.AppendChild(DialogTextPreviewNode);

                XmlNode DialogTextTopNode = XmlDoc.CreateElement("DialogTextTop");
                DialogTextTopNode.InnerText = ListDialog[D].TextTop;
                DialogNode.AppendChild(DialogTextTopNode);

                RootNode.AppendChild(DialogNode);
            }

            XmlDoc.Save(ExportPath);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsPaused || CurrentDialog == null)
                return;

            ChoicesScrollbar.Update(gameTime);

            //Left Character.
            if (LeftCharacter != null)
            {
                if (LeftCharacter.IsAnimated)
                    LeftCharacter.Update(gameTime);
            }
            //Right character.
            if (RightCharacter != null)
            {
                if (RightCharacter.IsAnimated)
                    RightCharacter.Update(gameTime);
            }

            //Skip the visual novel.
            if (InputHelper.InputSkipPressed())
            {//If at the last scene.
                if (TimelineIndex == Timeline.Count)
                {
                    if (OnVisualNovelEnded != null)
                        OnVisualNovelEnded();

                    //Skip to the next screen.
                    RemoveScreen(this);
                    Content.Unload();
                }
                else
                {//Move the the next waypoint.
                    IncrementTimeline();
                }
            }

            if (InputHelper.InputUpPressed())
            {
                DialogChoice -= DialogChoice > 0 ? 1 : 0;
                if (DialogChoice <= DialogChoiceMinIndex)
                {
                    DialogChoiceMinIndex = DialogChoice;
                    ChoicesScrollbar.SetValue(DialogChoiceMinIndex);
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                DialogChoice += DialogChoice < CurrentDialog.ListNextDialog.Count - 1 ? 1 : 0;
                if (DialogChoice - MaxDialogChoice >= DialogChoiceMinIndex)
                {
                    DialogChoiceMinIndex = DialogChoice - MaxDialogChoice + 1;
                    ChoicesScrollbar.SetValue(DialogChoiceMinIndex);
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                if (CurrentDialog.CutsceneAfter != null)
                    PushScreen(CurrentDialog.CutsceneAfter);

                if (TimelineIndex < Timeline.Count)
                {
                    //If there is no dialog linked to the CurrentDialog.
                    if (CurrentDialog.ListNextDialog.Count == 0 || string.IsNullOrEmpty(ListDialog[CurrentDialog.ListNextDialog[DialogChoice]].Text))
                    {
                        IncrementTimeline();
                        if (TimelineIndex >= Timeline.Count)
                        {
                            if (OnVisualNovelEnded != null)
                                OnVisualNovelEnded();

                            RemoveScreen(this);
                        }
                    }
                    else
                    {
                        CurrentDialog = ListDialog[CurrentDialog.ListNextDialog[DialogChoice]];
                        OnNewFrame();
                    }
                }

                if (CurrentDialog.CutsceneBefore != null)
                    PushScreen(CurrentDialog.CutsceneBefore);
            }
            else if (InputHelper.InputCancelPressed())
            {//Show/Hide the sumary.
                PushScreen(new ExtraMenu(this, fntFinlanderFont, TimelineIndexMax));
            }
        }

        private void OnScrollbarChange(float ScrollbarValue)
        {
            DialogChoiceMinIndex = (int)ScrollbarValue;
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            //Left Character.
            if (LeftCharacter != null)
            {
                LeftCharacter.BeginDraw(g);
            }
            //Right character.
            if (RightCharacter != null)
            {
                RightCharacter.BeginDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (IsPaused)
                return;

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            #region Bust Dialog rendering

            //Background first.
            if (CurrentBackground != null)
            {
                g.Draw(CurrentBackground.Sprite, new Rectangle(0, 0, CurrentBackground.Sprite.Width, CurrentBackground.Sprite.Height), Color.White);
            }

            //Left Character.
            if (LeftCharacter != null)
            {
                DrawCharacter(g, LeftCharacter, LeftPosition, SpriteEffects.FlipHorizontally, ActiveChar != Dialog.ActiveBustCharacterStates.Left && ActiveChar != Dialog.ActiveBustCharacterStates.Both);
            }
            //Right character.
            if (RightCharacter != null)
            {
                DrawCharacter(g, RightCharacter, RightPosition, SpriteEffects.None, ActiveChar != Dialog.ActiveBustCharacterStates.Right && ActiveChar != Dialog.ActiveBustCharacterStates.Both);
            }

            #endregion

            if (CurrentDialog != null)
            {
                if (CurrentDialog.TopPortaitVisibleState != Dialog.PortaitVisibleStates.Invisible)
                {
                    DrawBox(g, new Vector2(0, 0), Constants.Width, VNBoxHeight, Color.White);
                    DrawBox(g, new Vector2(10, 10), 108, 108, Color.White);
                    g.Draw(sprPixel, new Rectangle(16, 16, 96, 96), Color.Gray);

                    if (CurrentDialog.TopCharacter != null)
                        g.Draw(CurrentDialog.TopCharacter.StaticSprite, new Vector2(16, 16), Color.White);

                    if (!string.IsNullOrEmpty(CurrentDialog.TextTop))
                        DrawText(g, new Vector2(125, 8), CurrentDialog.TextTop);
                    else if (LastDialog != null)
                        DrawText(g, new Vector2(125, 8), LastDialog.Text);

                    if (CurrentDialog.TopPortaitVisibleState == Dialog.PortaitVisibleStates.Greyed)
                        g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, VNBoxHeight), Color.FromNonPremultiplied(128, 128, 128, 128));
                }

                //Text.
                DrawBox(g, new Vector2(0, Constants.Height - VNBoxHeight), Constants.Width, VNBoxHeight, Color.White);

                if (CurrentDialog.BottomPortaitVisibleState != Dialog.PortaitVisibleStates.Invisible)
                {
                    DrawBox(g, new Vector2(10, Constants.Height - VNBoxHeight + 10), 108, 108, Color.White);
                    g.Draw(sprPixel, new Rectangle(16, Constants.Height - VNBoxHeight + 16, 96, 96), Color.Gray);

                    if (CurrentDialog.BottomCharacter != null)
                        g.Draw(CurrentDialog.BottomCharacter.StaticSprite, new Vector2(16, Constants.Height - VNBoxHeight + 16), Color.White);

                    DrawText(g, new Vector2(125, Constants.Height - VNBoxHeight + 8), CurrentDialog.Text);

                    if (CurrentDialog.BottomPortaitVisibleState == Dialog.PortaitVisibleStates.Greyed)
                        g.Draw(sprPixel, new Rectangle(0, Constants.Height - VNBoxHeight, Constants.Width, VNBoxHeight), Color.FromNonPremultiplied(128, 128, 128, 128));
                }
                else
                    DrawText(g, new Vector2(5, Constants.Height - VNBoxHeight), CurrentDialog.Text);
            }
        }

        private void DrawText(CustomSpriteBatch g, Vector2 Position, string Text)
        {
            //If there's no linked dialogs.
            if (CurrentDialog.ListNextDialog.Count == 0 || ListDialog[CurrentDialog.ListNextDialog[0]].TextPreview == "")
            {
                g.DrawString(fntFinlanderFont, Text, new Vector2(Position.X, Position.Y), Color.White);
            }
            else
            {
                //Draw the text.
                g.DrawString(fntFinlanderFont, Text, new Vector2(Position.X, Position.Y), Color.White);
                //Get the Y position from where to draw the linked Dialogs TextPreview.
                int TextEndY = (int)fntFinlanderFont.MeasureString(CurrentDialog.Text).Y;

                //Draw the linked Dialogs TextPreview.
                for (int i = DialogChoiceMinIndex, D = 0; D < MaxDialogChoice; ++D, ++i)
                {
                    g.Draw(sprPixel, new Rectangle((int)Position.X + 10, (int)Position.Y + 13 + TextEndY + D * 25, 5, 5), Color.Black);
                    g.DrawString(fntFinlanderFont, ListDialog[CurrentDialog.ListNextDialog[i]].TextPreview, new Vector2(Position.X + 20, Position.Y + TextEndY + D * 25), Color.White);
                    if (i == DialogChoice)
                    {
                        g.Draw(sprPixel, new Rectangle((int)Position.X + 20, (int)Position.Y + 5 + TextEndY + D * 25, 400, fntFinlanderFont.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 100));
                    }
                }

                if (CurrentDialog.ListNextDialog.Count >= MaxDialogChoice)
                {
                    ChoicesScrollbar.Draw(g);
                }
            }
        }

        private static void DrawCharacter(CustomSpriteBatch g, SimpleAnimation DrawnCharacter, Vector2 Position, SpriteEffects ActiveSpriteEffects, bool IsShadowed)
        {
            if (DrawnCharacter.IsAnimated)
            {
                Vector2 AnimationOriginPosition = DrawnCharacter.ActiveAnimation.AnimationOrigin.Position;

                for (int L = DrawnCharacter.ActiveAnimation.ListAnimationLayer.Count - 1; L >= 0; --L)
                {
                    int OriginX = (int)AnimationOriginPosition.X;
                    if (ActiveSpriteEffects == SpriteEffects.FlipHorizontally)
                        OriginX = DrawnCharacter.ActiveAnimation.ListAnimationLayer[L].renderTarget.Width - (int)AnimationOriginPosition.X;

                    g.Draw(DrawnCharacter.ActiveAnimation.ListAnimationLayer[L].renderTarget,
                        new Vector2(50, 50), null, Color.White, 0,
                        new Vector2(0, 0),
                        new Vector2(1, 1), ActiveSpriteEffects, 0);

                    if (IsShadowed)
                    {
                        g.Draw(DrawnCharacter.ActiveAnimation.ListAnimationLayer[L].renderTarget,
                            new Vector2(Position.X, Position.Y), null, Color.FromNonPremultiplied(0, 0, 0, 127), 0,
                            new Vector2(OriginX, AnimationOriginPosition.Y),
                            new Vector2(1, 1), ActiveSpriteEffects, 0);
                    }
                }
            }
            else
            {
                int X = 0;
                if (ActiveSpriteEffects != SpriteEffects.FlipHorizontally)
                {
                    X = Constants.Width - DrawnCharacter.StaticSprite.Width;
                }
                int Y = Constants.Height - VNBoxHeight - DrawnCharacter.StaticSprite.Height;

                g.Draw(DrawnCharacter.StaticSprite, new Rectangle(X, Y, DrawnCharacter.StaticSprite.Width, DrawnCharacter.StaticSprite.Height),
                                              new Rectangle(0, 0, DrawnCharacter.StaticSprite.Width, DrawnCharacter.StaticSprite.Height),
                                              Color.White, 0, Vector2.Zero, ActiveSpriteEffects, 0);

                if (IsShadowed)
                {
                    g.Draw(DrawnCharacter.StaticSprite, new Rectangle(X, Y, DrawnCharacter.StaticSprite.Width, DrawnCharacter.StaticSprite.Height),
                                                  new Rectangle(0, 0, DrawnCharacter.StaticSprite.Width, DrawnCharacter.StaticSprite.Height),
                                                  Color.FromNonPremultiplied(0, 0, 0, 127), 0, Vector2.Zero, ActiveSpriteEffects, 0);
                }
            }
        }

        private void IncrementTimeline()
        {
            //Move the next Timeline entry.
            TimelineIndex++;
            //If there is a next entry, set the CurrentDialog to it.
            if (TimelineIndex < Timeline.Count)
            {
                LastDialog = CurrentDialog;
                CurrentDialog = Timeline[TimelineIndex];
            }

            OnNewFrame();
        }

        private void OnNewFrame()
        {
            UpdateTextChoices();

            //Update the MaxTime.
            if (TimelineIndex > TimelineIndexMax)
            {
                TimelineIndexMax = TimelineIndex;

                //Make sure this is called only on the first time.
                if (OnVisualNovelFrameChanged != null)
                    OnVisualNovelFrameChanged();
            }
        }

        public void UpdateTextChoices()
        {
            DialogChoice = 0;
            DialogChoiceMinIndex = 0;

            float TextEndY = Constants.Height - VNBoxHeight + 8 + fntFinlanderFont.MeasureString(CurrentDialog.Text).Y;
            MaxDialogChoice = (Constants.Height - (int)TextEndY) / 25;

            if (MaxDialogChoice >= CurrentDialog.ListNextDialog.Count)
                MaxDialogChoice = CurrentDialog.ListNextDialog.Count;

            ChoicesScrollbar.SetValue(0);
            ChoicesScrollbar.ChangeMaxValue(Math.Max(1, CurrentDialog.ListNextDialog.Count - MaxDialogChoice));
        }
    }
}
