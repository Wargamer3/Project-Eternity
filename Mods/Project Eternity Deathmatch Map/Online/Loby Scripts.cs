using System;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen.Online
{
    public sealed class DeathmatchLobyScriptHolder : OnlineScriptHolder
    {
        public override KeyValuePair<string, List<OnlineScript>> GetNameAndContent(params object[] args)
        {
            List<OnlineScript> ListOnlineScript = ReflectionHelper.GetNestedTypes<OnlineScript>(typeof(DeathmatchLobyScriptHolder), args);
            return new KeyValuePair<string, List<OnlineScript>>("Deathmatch Loby", ListOnlineScript);
        }

        public abstract class LobyScript : OnlineScript
        {
            protected readonly MultiplayerScreen Loby;

            protected LobyScript(string Name, MultiplayerScreen Loby)
                : base(Name)
            {
                this.Loby = Loby;
            }
        }

        public class LobyMessageScript : LobyScript
        {
            private String Text;

            public LobyMessageScript()
                : this(null, null)
            {
            }

            public LobyMessageScript(MultiplayerScreen Loby)
                : this(Loby, null)
            {
            }

            public LobyMessageScript(MultiplayerScreen Loby, string Text)
                : base("Message", Loby)
            {
                this.Text = Text;
            }

            public override OnlineScript Copy()
            {
                return new LobyMessageScript(Loby);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Loby.Messenger.Add(Text);
            }

            protected override void Read(OnlineReader Sender)
            {
                Text = Sender.ReadString();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendString(Text);
            }
        }
    }
}
