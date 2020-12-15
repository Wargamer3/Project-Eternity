using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    public class VisualNovelBackground
    {
        public string Name;
        public string Path;
        public Texture2D Sprite;

        public VisualNovelBackground(string Name, string Path, Texture2D Sprite)
        {
            this.Name = Name;
            this.Path = Path;
            this.Sprite = Sprite;
        }

        public override bool Equals(object obj)
        {
            VisualNovelBackground OtherBackground = obj as VisualNovelBackground;
            if (OtherBackground == null)
                return false;
            else
            {
                if (Name == OtherBackground.Name && Path == OtherBackground.Path)
                    return true;

                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public struct SpeakerPriority
    {
        public enum PriorityTypes { Character, Location, ID };

        public readonly PriorityTypes PriorityType;
        public readonly string Name;

        public SpeakerPriority(PriorityTypes PriorityType, string Name)
        {
            this.PriorityType = PriorityType;
            this.Name = Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Dialog
    {
        public enum ActiveBustCharacterStates { Left = 0, Right = 1, None = 2, Both = 3 };

        public enum PortaitVisibleStates { Visible, Greyed, Invisible }

        public Point Position;
        public SimpleAnimation LeftCharacter;
        public SimpleAnimation RightCharacter;
        public SimpleAnimation TopCharacter;
        public SimpleAnimation BottomCharacter;
        public PortaitVisibleStates TopPortaitVisibleState;
        public PortaitVisibleStates BottomPortaitVisibleState;
        public VisualNovelBackground Back;
        public ActiveBustCharacterStates ActiveBustCharacterState;
        public string Text;
        public string TextTop;
        public string TextPreview;
        public List<int> ListNextDialog;//List of index of the possible next dialogs, return to parent dialog if nothing is after.
        public Cutscene CutsceneBefore;
        public Cutscene CutsceneAfter;
        public bool OverrideCharacterPriority;
        public readonly List<SpeakerPriority> ListSpeakerPriority;

        public Dialog(Dialog Other)
        {
            ListNextDialog = new List<int>();
            ListSpeakerPriority = new List<SpeakerPriority>();
            OverrideCharacterPriority = false;

            Position = Point.Zero;
            LeftCharacter = Other.LeftCharacter;
            RightCharacter = Other.RightCharacter;
            TopCharacter = Other.TopCharacter;
            BottomCharacter = Other.BottomCharacter;
            Back = Other.Back;
            ActiveBustCharacterState = Other.ActiveBustCharacterState;
            TopPortaitVisibleState = Other.TopPortaitVisibleState;
            BottomPortaitVisibleState = Other.BottomPortaitVisibleState;
            Text = Other.Text;
            TextPreview = Other.TextPreview;
            TextTop = Other.TextTop;
        }

        public Dialog(SimpleAnimation LeftCharacter, SimpleAnimation RightCharacter, SimpleAnimation TopCharacter, SimpleAnimation BottomCharacter,
            VisualNovelBackground Back, ActiveBustCharacterStates ActiveBustCharacterState, string Text, string TextPreview)
        {
            ListNextDialog = new List<int>();
            ListSpeakerPriority = new List<SpeakerPriority>();
            OverrideCharacterPriority = false;

            this.Position = Point.Zero;
            this.LeftCharacter = LeftCharacter;
            this.RightCharacter = RightCharacter;
            this.TopCharacter = TopCharacter;
            this.BottomCharacter = BottomCharacter;
            this.Back = Back;
            this.ActiveBustCharacterState = ActiveBustCharacterState;
            this.Text = Text;
            this.TextPreview = TextPreview;
            TextTop = "";
            TopPortaitVisibleState = PortaitVisibleStates.Invisible;
            BottomPortaitVisibleState = PortaitVisibleStates.Invisible;
        }

        public override bool Equals(object obj)
        {
            Dialog OtherDialog = obj as Dialog;
            if (OtherDialog == null)
                return false;
            else
            {
                if (LeftCharacter == OtherDialog.LeftCharacter && RightCharacter == OtherDialog.RightCharacter && Back == OtherDialog.Back
                    && Text == OtherDialog.Text && TextPreview == OtherDialog.TextPreview && ListNextDialog == OtherDialog.ListNextDialog)
                    return true;

                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Represent a basic timeline.
    /// </summary>
    public struct VisualNovelTimeline
    {
        public int CurrentTime;
        public int MaxTime;
        //Characters to draw.
        private int[,] CharacterChoices;

        //What each scene will draw for text.
        public string[] CharacterTexts;

        //Which background to draw for each scene.
        private int[] BackgroundChoices;

        //Which point to skip to next.
        public int[] Waypoints;

        /// <summary>
        /// Create a new VisualNovelTimeline with a list of scenes to draw with each characters, text and background to use.
        /// If no character is required for a side, 0 should be used.
        /// </summary>
        /// <param name="CharacterChoices">Array of 3 choices. 1st choice is the character on the left.
        /// 2nd choice is the character on the right.
        /// 3rd choice is which character is talking(0 is left, 1 is right).
        /// The characters choices use the index of the character list + 1.</param>
        /// <param name="CharacterTexts">The text to draw.</param>
        /// <param name="BackgroundChoices">The background to draw.</param>
        /// <param name="Waypoints">List of waypoint from which to skip to.</param>
        public VisualNovelTimeline(int[,] CharacterChoices, string[] CharacterTexts, int[] BackgroundChoices, int[] Waypoints)
        {
            this.CharacterChoices = CharacterChoices;
            this.CharacterTexts = CharacterTexts;
            this.BackgroundChoices = BackgroundChoices;
            this.Waypoints = Waypoints;
            CurrentTime = 0;
            MaxTime = 0;
        }

        /// <summary>
        /// Return the number of scenes the VisualNovelTimeline contains.
        /// </summary>
        public int Size
        {
            get { return CharacterTexts.Length; }
        }

        /// <summary>
        /// Return the text index of the text list in the current scene.
        /// </summary>
        public string Text
        {
            get { return CharacterTexts[CurrentTime]; }
        }

        /// <summary>
        /// Return the background index of the background list in the current scene.
        /// </summary>
        public int Background
        {
            get { return BackgroundChoices[CurrentTime]; }
        }

        /// <summary>
        /// Returns which character is active(0 for the first, 1 for the second)
        /// </summary>
        public int CharacterActive
        {
            get { return CharacterChoices[CurrentTime, 2]; }
        }

        /// <summary>
        /// Return the index of the first character of the character list in the current scene.
        /// </summary>
        public int Character1Index
        {
            get { return CharacterChoices[CurrentTime, 0] - 1; }
        }

        /// <summary>
        /// Return the index of the second character of the character list in the current scene.
        /// </summary>
        public int Character2Index
        {
            get { return CharacterChoices[CurrentTime, 1] - 1; }
        }
    };

    public class VisualNovelCharacter
    {
        public string CharacterName;
        private Character _LoadedCharacter;
        public Character LoadedCharacter { get { return _LoadedCharacter; } }
        public readonly List<SpeakerPriority> ListSpeakerPriority;//Used to center camera if the character is talking

        public VisualNovelCharacter(string CharacterName)
        {
            this.CharacterName = CharacterName;

            ListSpeakerPriority = new List<SpeakerPriority>();

            Dictionary<string, BaseSkillRequirement> DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            Dictionary<string, BaseEffect> DicEffect = BaseEffect.LoadAllEffects();
            _LoadedCharacter = new Character(CharacterName, null, DicRequirement, DicEffect);
        }

        public VisualNovelCharacter(BinaryReader BR)
        {
            this.CharacterName = BR.ReadString();

            int ListSpeakerPriorityCounter = BR.ReadInt32();
            ListSpeakerPriority = new List<SpeakerPriority>(ListSpeakerPriorityCounter);
            for (int S = 0; S < ListSpeakerPriorityCounter; ++S)
            {
                ListSpeakerPriority.Add(new SpeakerPriority((SpeakerPriority.PriorityTypes)BR.ReadByte(), BR.ReadString()));
            }

            Dictionary<string, BaseSkillRequirement> DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            Dictionary<string, BaseEffect> DicEffect = BaseEffect.LoadAllEffects();
            _LoadedCharacter = new Character(CharacterName, null, DicRequirement, DicEffect);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(CharacterName);
            BW.Write(ListSpeakerPriority.Count);
            for (int S = 0; S < ListSpeakerPriority.Count; S++)
            {
                BW.Write((byte)ListSpeakerPriority[S].PriorityType);
                BW.Write(ListSpeakerPriority[S].Name);
            }
        }

        public override string ToString()
        {
            return CharacterName;
        }
    }

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

        private bool ShowSumary = false;
        public bool IsPaused = false;
        public bool UseLocalization = true;
        public Dictionary<string, int> DicMapVariables;

        public VisualNovelFrameChanged OnVisualNovelFrameChanged;
        public VisualNovelPaused OnVisualNovelPaused;
        public VisualNovelResumed OnVisualNovelResumed;
        public VisualNovelEnded OnVisualNovelEnded;

        public string VisualNovelPath;

        private Vector2 LeftPosition;
        private Vector2 RightPosition;

        public const int VNBoxHeight = 128;

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

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            fntMultiDialogFont = Content.Load<SpriteFont>("Fonts/VisualNovelMultiDialogFont");

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

                    Dictionary<string, CutsceneScript> Scripts = CutsceneScriptHolder.LoadAllScripts();

                    NewDialog.CutsceneBefore = new Cutscene(null, BR, Scripts);
                    NewDialog.CutsceneAfter = new Cutscene(null, BR, Scripts);
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
                if (ShowSumary)
                {//Change the CurrentDialog to the one selected in the sumary(chosed by TimelineIndex).
                    TimelineIndex -= TimelineIndex > 0 ? 1 : 0;
                    CurrentDialog = Timeline[TimelineIndex];
                    ShowSumary = true;
                }
                else
                {
                    DialogChoice -= DialogChoice > 0 ? 1 : 0;
                    if (DialogChoice <= DialogChoiceMinIndex)
                        DialogChoiceMinIndex = DialogChoice;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (ShowSumary)
                {
                    //Change the CurrentDialog to the one selected in the sumary(chosed by TimelineIndex).
                    TimelineIndex += TimelineIndex < TimelineIndexMax - 1 ? 1 : 0;
                    CurrentDialog = Timeline[TimelineIndex];
                    ShowSumary = true;
                }
                else
                {
                    DialogChoice += DialogChoice < CurrentDialog.ListNextDialog.Count - 1 ? 1 : 0;
                    if (DialogChoice - MaxDialogChoice >= DialogChoiceMinIndex)
                        DialogChoiceMinIndex = DialogChoice - MaxDialogChoice + 1;
                }
            }
            if (InputHelper.InputConfirmPressed())
            {
                if (CurrentDialog.CutsceneAfter != null)
                    PushScreen(CurrentDialog.CutsceneAfter);

                if (TimelineIndex < Timeline.Count)
                {
                    //If there is no dialog linked to the CurrentDialog.
                    if (CurrentDialog.ListNextDialog.Count == 0)
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
                        //If ListNextDialog.Count is not 0, it means there were linked dialogs, so change the current dialog to the one chosen.
                        CurrentDialog = ListDialog[CurrentDialog.ListNextDialog[DialogChoice]];
                        OnNewFrame();
                    }
                }

                if (CurrentDialog.CutsceneBefore != null)
                    PushScreen(CurrentDialog.CutsceneBefore);
            }
            else if (InputHelper.InputCancelPressed())
            {//Show/Hide the sumary.
                ShowSumary = !ShowSumary;
            }
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

            if (ShowSumary)
            {
                //Draw the VN sumary.
                g.Draw(sprPixel, new Rectangle(Constants.Width / 2 - 100, 100, 200, 150), Color.Gray);//Background
                int i = Math.Max(0, TimelineIndex - 5);
                int CurrentText = 0;
                //Draw a sumary of every text line up to MaxTime.
                while (CurrentText <= 5 && i < TimelineIndexMax)
                {//Draw cursor.
                    if (i == TimelineIndex)
                        g.Draw(sprPixel, new Rectangle(Constants.Width / 2 - 100, 100 + CurrentText * fntFinlanderFont.LineSpacing * 2, 200, fntFinlanderFont.LineSpacing * 2), Color.FromNonPremultiplied(255, 255, 255, 100));
                    //Crop the text before drawing it.
                    string TextBuffer = Timeline[i].Text.Substring(0, Math.Min(20, Timeline[i].Text.Length));
                    g.DrawString(fntFinlanderFont, TextBuffer, new Vector2(Constants.Width / 2 - 100, 100 + CurrentText * (fntFinlanderFont.LineSpacing * 2)), Color.White);
                    CurrentText++;
                    i++;
                }
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
                for (int i = DialogChoiceMinIndex, D = 0 ; D < MaxDialogChoice; ++D, ++i)
                {
                    g.Draw(sprPixel, new Rectangle((int)Position.X + 10, (int)Position.Y + 13 + TextEndY + D * 25, 5, 5), Color.Black);
                    g.DrawString(fntFinlanderFont, ListDialog[CurrentDialog.ListNextDialog[i]].TextPreview, new Vector2(Position.X + 20, Position.Y + TextEndY + D * 25), Color.White);
                    if (i == DialogChoice)
                    {
                        g.Draw(sprPixel, new Rectangle((int)Position.X + 20, (int)Position.Y + 5 + TextEndY + D * 25, 400, fntFinlanderFont.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 100));
                    }
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
        }
    }
}
