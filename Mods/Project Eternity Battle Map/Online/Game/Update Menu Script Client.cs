using System;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class UpdateMenuScriptClient : OnlineScript
    {
        public const string ScriptName = "Update Menu";

        private readonly BattleMapOnlineClient Owner;
        private ActionPanel ActivePanel;

        private string PanelName;
        private byte[] ArrayUpdateData;

        public UpdateMenuScriptClient(BattleMapOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public UpdateMenuScriptClient(ActionPanel ActivePanel)
            : base(ScriptName)
        {
            this.ActivePanel = ActivePanel;
        }

        public override OnlineScript Copy()
        {
            return new UpdateMenuScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            ActivePanel.WriteUpdate(WriteBuffer);
        }

        protected override void Execute(IOnlineConnection ActivePlayer)
        {
            Owner.BattleMapGame.ListActionMenuChoice.ExecuteUpdate(PanelName, ArrayUpdateData);
        }

        protected override void Read(OnlineReader Sender)
        {
            PanelName = Sender.ReadString();
            ArrayUpdateData = Sender.ReadByteArray();
        }
    }
}
