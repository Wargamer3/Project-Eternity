using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.VisualNovelScreen.Online
{
    public class ConfirmChoiceVisualNovelScriptClient : OnlineScript
    {
        public const string ScriptName = "Confirm Choice Visual Novel";

        private GameScreen Owner;

        private string VisualNovelPath;
        private int DialogIndex;
        private int DialogChoice;

        public Dictionary<int, int> DicDialogChoiceConfirmation;

        public ConfirmChoiceVisualNovelScriptClient(GameScreen Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
            DicDialogChoiceConfirmation = new Dictionary<int, int>();
        }

        public ConfirmChoiceVisualNovelScriptClient(string VisualNovelPath, int DialogIndex, int DialogChoice)
            : base(ScriptName)
        {
            this.VisualNovelPath = VisualNovelPath;
            this.DialogIndex = DialogIndex;
            this.DialogChoice = DialogChoice;
        }

        public override OnlineScript Copy()
        {
            return new ConfirmChoiceVisualNovelScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(VisualNovelPath);
            WriteBuffer.AppendInt32(DialogIndex);
            WriteBuffer.AppendInt32(DialogChoice);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            for (int S = Owner.ListGameScreen.Count - 1; S >= 0; S--)
            {
                GameScreen ActiveScreen = Owner.ListGameScreen[S];
                VisualNovel ActiveVisualNovel = ActiveScreen as VisualNovel;
                if (ActiveVisualNovel != null && ActiveVisualNovel.VisualNovelPath == VisualNovelPath)
                {
                    ActiveVisualNovel.DicDialogChoiceConfirmation = DicDialogChoiceConfirmation;
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            VisualNovelPath = Sender.ReadString();
            DialogIndex = Sender.ReadInt32();
            int DicDialogChoiceConfirmationCount = Sender.ReadInt32();
            for (int C = 0; C < DicDialogChoiceConfirmationCount; ++C)
            {
                DicDialogChoiceConfirmation.Add(Sender.ReadInt32(), Sender.ReadInt32());
            }
        }
    }
}
