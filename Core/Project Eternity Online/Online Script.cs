using System;

namespace ProjectEternity.Core.Online
{
    public abstract class OnlineScript
    {
        public readonly string Name;

        public abstract OnlineScript Copy();
        protected internal abstract void Read(OnlineReader Sender);
        /// <summary>
        /// Execute code on every connected players
        /// </summary>
        protected internal abstract void Execute(IOnlineConnection ActivePlayer);
        protected abstract void DoWrite(OnlineWriter WriteBuffer);

        protected OnlineScript(string Name)
        {
            this.Name = Name;
        }

        internal void Write(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Name);
            DoWrite(WriteBuffer);
        }
    }
}
