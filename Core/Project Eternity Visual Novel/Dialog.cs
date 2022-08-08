using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
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

        public override string ToString()
        {
            string LeftCharacterText = "";
            string RightCharacterText = "";
            string BackgroundText = "";
            //Add a * before the LeftCharacter name to show it's the one selected.
            if (ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Left)
                LeftCharacterText += "*";
            //Add its name.
            if (LeftCharacter != null)
                LeftCharacterText += LeftCharacter.Name;

            //Add a * before the RightCharacter name to show it's the one selected.
            if (ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Right)
                RightCharacterText += "*";
            //Add its name.
            if (RightCharacter != null)
                RightCharacterText += RightCharacter.Name;

            //Add the Background name.
            if (Back != null)
                BackgroundText += Back.Name;
            //Set the final text in the lstDialogs.
            return LeftCharacterText + " - " + RightCharacterText + " - " + BackgroundText + Text;
        }
    }
}
