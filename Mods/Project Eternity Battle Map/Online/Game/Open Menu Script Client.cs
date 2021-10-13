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
        private ActionPanel ActivePanel;

        public OpenMenuScriptClient(BattleMapOnlineClient Owner, Dictionary<string, ActionPanel> DicActionPanel)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.DicActionPanel = DicActionPanel;
        }

        public OpenMenuScriptClient(ActionPanel ActivePanel)
            : base(ScriptName)
        {
            this.ActivePanel = ActivePanel;
        }

        public override OnlineScript Copy()
        {
            return new OpenMenuScriptClient(Owner, DicActionPanel);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            List<ActionPanel> ListActionPanel = ActivePanel.GetActionPanels();

            WriteBuffer.AppendInt32(ListActionPanel.Count);
            foreach (ActionPanel ActiveActionPanel in ListActionPanel)
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
