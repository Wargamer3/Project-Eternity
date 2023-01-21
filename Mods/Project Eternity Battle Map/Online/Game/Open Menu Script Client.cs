using System;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class OpenMenuScriptClient : OnlineScript
    {
        public const string ScriptName = "Open Menu";

        private readonly BattleMapOnlineClient Owner;
        private readonly Dictionary<string, ActionPanel> DicActionPanel;
        private ActionPanel[] ArrayActionPanel;

        public OpenMenuScriptClient(BattleMapOnlineClient Owner, Dictionary<string, ActionPanel> DicActionPanel)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.DicActionPanel = DicActionPanel;
        }

        public OpenMenuScriptClient(ActionPanel ActivePanel)
            : base(ScriptName)
        {
            List<ActionPanel> ListActionPanel = ActivePanel.GetActionPanels();

            ArrayActionPanel = new ActionPanel[ListActionPanel.Count];
            for (int A = 0; A < ListActionPanel.Count; ++A)
            {
                ArrayActionPanel[A] = ListActionPanel[A];
            }
        }

        public override OnlineScript Copy()
        {
            return new OpenMenuScriptClient(Owner, DicActionPanel);
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
            Owner.BattleMapGame.ListActionMenuChoice.Set(ArrayActionPanel);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListActionPanelCount = Sender.ReadInt32();
            ArrayActionPanel = new ActionPanel[ListActionPanelCount];
            for (int A = 0; A < ListActionPanelCount; ++A)
            {
                ArrayActionPanel[A] = ActionPanel.Read(Sender, DicActionPanel);
            }
        }
    }
}
