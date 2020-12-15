using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class MasterListScriptServer : OnlineScript
    {
        private List<IOnlineConnection> ListOtherMaster;

        public MasterListScriptServer(List<IOnlineConnection> ListOtherMaster)
            : base("Master List")
        {
            this.ListOtherMaster = ListOtherMaster;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListOtherMaster.Count);
            for (int M = 0; M < ListOtherMaster.Count; ++M)
            {
                WriteBuffer.AppendString(ListOtherMaster[M].IP);
            }
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
