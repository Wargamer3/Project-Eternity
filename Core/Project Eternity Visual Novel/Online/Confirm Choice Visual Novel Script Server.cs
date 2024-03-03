using System;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.VisualNovelScreen.Online
{
    public class ConfirmChoiceVisualNovelScriptServer : OnlineScript
    {
        public const string ScriptName = "Confirm Choice Visual Novel";

        public GameScreen Owner;
        public GameClientGroup GameGroup;

        private string VisualNovelPath;
        private int DialogIndex;
        private int DialogChoice;

        public Dictionary<int, int> DicDialogChoiceConfirmation;

        public ConfirmChoiceVisualNovelScriptServer(string VisualNovelPath, int DialogIndex, Dictionary<int, int> DicDialogChoiceConfirmation)
            : base(ScriptName)
        {
            this.VisualNovelPath = VisualNovelPath;
            this.DialogIndex = DialogIndex;
            this.DicDialogChoiceConfirmation = DicDialogChoiceConfirmation;
        }

        public ConfirmChoiceVisualNovelScriptServer(GameScreen Owner, GameClientGroup GameGroup)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.GameGroup = GameGroup;
        }

        public override OnlineScript Copy()
        {
            return new ConfirmChoiceVisualNovelScriptServer(Owner, GameGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(VisualNovelPath);
            WriteBuffer.AppendInt32(DialogIndex);
            WriteBuffer.AppendInt32(DicDialogChoiceConfirmation.Count);
            foreach (KeyValuePair<int, int> ActiveChoice in DicDialogChoiceConfirmation)
            {
                WriteBuffer.AppendInt32(ActiveChoice.Key);
                WriteBuffer.AppendInt32(ActiveChoice.Value);
            }
        }

        protected override void Execute(IOnlineConnection Host)
        {
            for (int S = Owner.ListGameScreen.Count - 1; S >= 0; S--)
            {
                GameScreen ActiveScreen = Owner.ListGameScreen[S];
                VisualNovel ActiveVisualNovel = ActiveScreen as VisualNovel;
                if (ActiveVisualNovel != null && ActiveVisualNovel.VisualNovelPath == VisualNovelPath)
                {
                    int TotalChoiceConfirmation;
                    if (!ActiveVisualNovel.DicDialogChoiceConfirmation.TryGetValue(DialogChoice, out TotalChoiceConfirmation))
                    {
                        ActiveVisualNovel.DicDialogChoiceConfirmation.Add(DialogChoice, 0);
                        TotalChoiceConfirmation = 0;
                    }

                    ++TotalChoiceConfirmation;
                    ActiveVisualNovel.DicDialogChoiceConfirmation[DialogChoice] = TotalChoiceConfirmation;

                    int Highest = -1;
                    bool MoreThanOneHighest = false;
                    int Total = 0;
                    foreach (KeyValuePair<int, int> ChoiceCount in ActiveVisualNovel.DicDialogChoiceConfirmation)
                    {
                        Total += ChoiceCount.Value;
                        if (Highest == -1 || ChoiceCount.Value > ActiveVisualNovel.DicDialogChoiceConfirmation[Highest])
                        {
                            Highest = ChoiceCount.Key;
                            MoreThanOneHighest = false;
                        }
                        else if (ChoiceCount.Value == ActiveVisualNovel.DicDialogChoiceConfirmation[Highest])
                        {
                            MoreThanOneHighest = true;
                        }
                    }

                    if (Total >= GameGroup.Room.ListUniqueOnlineConnection.Count)
                    {
                        if (MoreThanOneHighest)//Random pick
                        {
                            List<int> ListPossibibleChoice = new List<int>();
                            foreach (KeyValuePair<int, int> ChoiceCount in ActiveVisualNovel.DicDialogChoiceConfirmation)
                            {
                                if (ChoiceCount.Value == ActiveVisualNovel.DicDialogChoiceConfirmation[Highest])
                                {
                                    ListPossibibleChoice.Add(ChoiceCount.Key);
                                }
                            }

                            if (GameGroup.Room.ListUniqueOnlineConnection.Count > 1)
                            {
                                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListUniqueOnlineConnection)
                                {
                                    ActivePlayer.Send(new ProceedVisualNovelChoiceScriptServer(VisualNovelPath, DialogIndex, ListPossibibleChoice[RandomHelper.Next(ListPossibibleChoice.Count)]));
                                }
                            }
                        }
                        else
                        {
                            if (GameGroup.Room.ListUniqueOnlineConnection.Count > 1)
                            {
                                foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListUniqueOnlineConnection)
                                {
                                    ActivePlayer.Send(new ProceedVisualNovelChoiceScriptServer(VisualNovelPath, DialogIndex, Highest));
                                }
                            }
                        }

                        ActiveVisualNovel.AdvanceDialog();
                    }
                    else
                    {
                        foreach (IOnlineConnection ActivePlayer in GameGroup.Room.ListUniqueOnlineConnection)
                        {
                            ActivePlayer.Send(new ConfirmChoiceVisualNovelScriptServer(VisualNovelPath, DialogIndex, ActiveVisualNovel.DicDialogChoiceConfirmation));
                        }
                    }
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
