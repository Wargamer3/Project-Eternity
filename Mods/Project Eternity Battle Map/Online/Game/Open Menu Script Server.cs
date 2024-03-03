using System;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class OpenMenuScriptServer : OnlineScript
    {
        public const string ScriptName = "Open Menu";

        private readonly BattleMapClientGroup ActiveGroup;
        private readonly Dictionary<string, ActionPanel> DicActionPanel;
        private ActionPanel[] ArrayActionPanel;
        private bool SendBackToSender;

        public OpenMenuScriptServer(BattleMapClientGroup ActiveGroup, Dictionary<string, ActionPanel> DicActionPanel)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
            this.DicActionPanel = DicActionPanel;
        }

        public OpenMenuScriptServer(ActionPanel[] ArrayActionPanel)
            : base(ScriptName)
        {
            this.ArrayActionPanel = ArrayActionPanel;
        }

        public override OnlineScript Copy()
        {
            return new OpenMenuScriptServer(ActiveGroup, DicActionPanel);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ArrayActionPanel.Length);
            foreach (ActionPanel ActiveActionPanel in ArrayActionPanel)
            {
                ActiveActionPanel.Write(WriteBuffer);
            }
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            for (int P = 0; P < ActiveGroup.Room.ListUniqueOnlineConnection.Count; P++)
            {
                if (SendBackToSender || ActiveGroup.Room.ListUniqueOnlineConnection[P] != Sender)
                {
                    ActiveGroup.Room.ListUniqueOnlineConnection[P].Send(new OpenMenuScriptServer(ArrayActionPanel));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            SendBackToSender = false;
            int ListActionPanelCount = Sender.ReadInt32();
            ArrayActionPanel = new ActionPanel[ListActionPanelCount];
            for (int A = 0; A < ListActionPanelCount; ++A)
            {
                ArrayActionPanel[A] = ActionPanel.Read(Sender, DicActionPanel);
                if (ArrayActionPanel[A].SendBackToSender)
                {
                    SendBackToSender = true;
                }
            }
        }
    }
}
