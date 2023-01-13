using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.VisualNovelScreen.Online
{
    public class ProceedVisualNovelChoiceScriptServer : OnlineScript
    {
        public const string ScriptName = "Proceed Visual Novel Choice";

        public GameScreen Owner;
        public GameClientGroup GameGroup;

        private string VisualNovelPath;
        private int DialogIndex;
        private int DialogChoice;

        public ProceedVisualNovelChoiceScriptServer(GameScreen Owner, GameClientGroup GameGroup)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.GameGroup = GameGroup;
        }

        public ProceedVisualNovelChoiceScriptServer(string VisualNovelPath, int DialogIndex, int DialogChoice)
            : base(ScriptName)
        {
            this.VisualNovelPath = VisualNovelPath;
            this.DialogIndex = DialogIndex;
            this.DialogChoice = DialogChoice;
        }

        public override OnlineScript Copy()
        {
            return new ProceedVisualNovelChoiceScriptServer(Owner, GameGroup);
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

                    foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListOnlinePlayer)
                    {
                        ActivePlayer.Send(new ProceedVisualNovelChoiceScriptServer(VisualNovelPath, DialogIndex, DialogChoice));
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
