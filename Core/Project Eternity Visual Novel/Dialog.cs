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
}
