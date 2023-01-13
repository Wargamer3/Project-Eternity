using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.VisualNovelScreen.Online
{
    public class ProceedVisualNovelChoiceScriptClient : OnlineScript
    {
        public const string ScriptName = "Proceed Visual Novel Choice";

        private GameScreen Owner;

        private string VisualNovelPath;
        private int DialogIndex;
        private int DialogChoice;

        public ProceedVisualNovelChoiceScriptClient(GameScreen Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public ProceedVisualNovelChoiceScriptClient(string VisualNovelPath, int DialogIndex, int DialogChoice)
            : base(ScriptName)
        {
            this.VisualNovelPath = VisualNovelPath;
            this.DialogIndex = DialogIndex;
            this.DialogChoice = DialogChoice;
        }

        public override OnlineScript Copy()
        {
            return new ProceedVisualNovelChoiceScriptClient(Owner);
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
                    if (DialogIndex > 0)
                    {
                        ActiveVisualNovel.CurrentDialog = ActiveVisualNovel.ListDialog[DialogIndex];
                    }

                    ActiveVisualNovel.AdvanceDialog();
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            VisualNovelPath = Sender.ReadString();
            DialogIndex = Sender.ReadInt32();
            DialogChoice = Sender.ReadInt32();
        }
    }
}
