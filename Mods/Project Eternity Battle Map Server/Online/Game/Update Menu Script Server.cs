using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class UpdateMenuScriptServer : OnlineScript
    {
        public const string ScriptName = "Update Menu";

        private readonly BattleMapClientGroup ActiveGroup;
        private string PanelName;
        private byte[] ArrayUpdateData;
        private bool SendBackToSender;

        public UpdateMenuScriptServer(BattleMapClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public UpdateMenuScriptServer(string PanelName, byte[] ArrayUpdateData)
            : base(ScriptName)
        {
            this.PanelName = PanelName;
            this.ArrayUpdateData = ArrayUpdateData;
        }

        public override OnlineScript Copy()
        {
            return new UpdateMenuScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PanelName);
            WriteBuffer.AppendByteArray(ArrayUpdateData);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            for (int P = 0; P < ActiveGroup.Room.ListUniqueOnlineConnection.Count; P++)
            {
                if (SendBackToSender || ActiveGroup.Room.ListUniqueOnlineConnection[P] != Sender)
                {
                    ActiveGroup.Room.ListUniqueOnlineConnection[P].Send(new UpdateMenuScriptServer(PanelName, ArrayUpdateData));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            SendBackToSender = false;
            PanelName = Sender.ReadString();
            ArrayUpdateData = Sender.ReadByteArray();
        }
    }
}
