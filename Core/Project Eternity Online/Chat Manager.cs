using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ProjectEternity.Core.Online
{
    public class ChatManager
    {
        public enum MessageColors : byte { White, Info = 255 }

        public struct ChatMessage
        {
            public readonly DateTime Date;
            public readonly string Message;
            public readonly MessageColors MessageColor;

            public ChatMessage(DateTime Date, string Message, MessageColors MessageColor)
            {
                this.Date = Date;
                this.Message = Message;
                this.MessageColor = MessageColor;
            }
        }

        public class ChatTab
        {
            public readonly string Name;
            public readonly List<ChatMessage> ListChatHistory;
            public bool HasUnreadMessages;
            public int ChatScrollUpValueInPixel;

            public ChatTab(string Name)
            {
                this.Name = Name;
                ListChatHistory = new List<ChatMessage>();
                HasUnreadMessages = false;
            }
        }

        private Dictionary<string, ChatTab> DicChatHistory;
        private string _ActiveChatTabID;

        public string ActiveTabID { get { return _ActiveChatTabID; } }
        public string ActiveTabName { get { return DicChatHistory[_ActiveChatTabID].Name; } }
        public int ActiveTabScrollUpValue { get { return DicChatHistory[_ActiveChatTabID].ChatScrollUpValueInPixel; } }
        public List<ChatMessage> ListActiveTabHistory { get { return DicChatHistory[_ActiveChatTabID].ListChatHistory; } }
        public IEnumerable<KeyValuePair<string, ChatTab>> DicTab { get { return DicChatHistory; } }

        public ChatManager()
        {
            DicChatHistory = new Dictionary<string, ChatTab>();
            OpenGlobalTab();
        }

        public void InsertTab(string TabID, string TabName)
        {
            if (!DicChatHistory.ContainsKey(TabID))
            {
                Dictionary<string, ChatTab> NewDicChatHistory = new Dictionary<string, ChatTab>();
                NewDicChatHistory.Add(TabID, new ChatTab(TabName));
                foreach (KeyValuePair<string, ChatTab> ActiveTab in DicChatHistory)
                {
                    NewDicChatHistory.Add(ActiveTab.Key, ActiveTab.Value);
                }

                DicChatHistory = NewDicChatHistory;
            }

            _ActiveChatTabID = TabID;
        }

        public void OpenTab(string TabID, string TabName)
        {
            if (!DicChatHistory.ContainsKey(TabID))
            {
                DicChatHistory.Add(TabID, new ChatTab(TabName));
            }

            _ActiveChatTabID = TabID;
        }

        public void OpenGlobalTab()
        {
            DicChatHistory.Add("Global", new ChatTab("Chat"));
            _ActiveChatTabID = "Global";
        }

        public void SelectTab(string ActiveTabID)
        {
            _ActiveChatTabID = ActiveTabID;
            DicChatHistory[_ActiveChatTabID].HasUnreadMessages = false;
        }

        public void CloseTab(string ActiveTabID)
        {
            DicChatHistory.Remove(ActiveTabID);
        }

        public void AddMessage(string Source, ChatMessage NewMessage)
        {
            ChatTab ActiveChatTab;
            if (!DicChatHistory.TryGetValue(Source, out ActiveChatTab))
            {
                ActiveChatTab = new ChatTab(Source);
                DicChatHistory.Add(Source, ActiveChatTab);
            }

            ActiveChatTab.ListChatHistory.Add(NewMessage);

            if (Source != _ActiveChatTabID)
            {
                DicChatHistory[Source].HasUnreadMessages = true;
            }
        }

        public void InsertMessages(string Source, List<ChatMessage> ListChatHistoryToInsert)
        {
            ChatTab ActiveChatTab;
            if (!DicChatHistory.TryGetValue(Source, out ActiveChatTab))
            {
                ActiveChatTab = new ChatTab(Source);
                DicChatHistory.Add(Source, ActiveChatTab);
            }
            ActiveChatTab.ListChatHistory.InsertRange(0, ListChatHistoryToInsert);
        }

        public void SaveMessage(string Source, ChatMessage NewMessage)
        {
            using (StreamWriter file = new StreamWriter("Logs/" + Source + ".txt", append: true))
            {
                string Date = "[" + NewMessage.Date.ToString(DateTimeFormatInfo.InvariantInfo) + "]";
                string Color = "[" + NewMessage.MessageColor + "]";
                file.WriteLineAsync(Date + Color + NewMessage.Message);
            }
        }
    }
}
