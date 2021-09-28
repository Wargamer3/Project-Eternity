using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class ChatManager
    {
        public enum MessageColors { White }
        public struct ChatTab
        {
            public readonly string Name;
            public readonly Dictionary<string, MessageColors> ListChatHistory;

            public ChatTab(string Name)
            {
                this.Name = Name;
                ListChatHistory = new Dictionary<string, MessageColors>();
            }
        }

        private Dictionary<string, ChatTab> DicChatHistory;
        private string _ActiveChatTabID;

        public string ActiveTabID { get { return _ActiveChatTabID; } }
        public string ActiveTabName { get { return DicChatHistory[_ActiveChatTabID].Name; } }
        public IEnumerable<string> ListActiveTabHistory { get { return DicChatHistory[_ActiveChatTabID].ListChatHistory.Keys; } }
        public IEnumerable<KeyValuePair<string, ChatTab>> DicTab { get { return DicChatHistory; } }

        public ChatManager()
        {
            DicChatHistory = new Dictionary<string, ChatTab>();
            DicChatHistory.Add("Global", new ChatTab("Chat"));
            _ActiveChatTabID = "Global";
        }

        public void OpenTab(string TabID, string TabName)
        {
            DicChatHistory.Add(TabID, new ChatTab(TabName));
        }

        public void SelectTab(string ActiveTabID)
        {
            _ActiveChatTabID = ActiveTabID;
        }

        public void AddMessage(string Source, string Message, MessageColors MessageColor)
        {
            ChatTab ActiveChatTab;
            if (DicChatHistory.TryGetValue(Source, out ActiveChatTab))
            {
                ActiveChatTab.ListChatHistory.Add(Message, MessageColor);
            }
            else
            {
                DicChatHistory.Add(Source, new ChatTab(Source));
                DicChatHistory[Source].ListChatHistory.Add(Message, MessageColor);
            }
        }
    }
}
