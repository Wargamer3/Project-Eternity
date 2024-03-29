﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanel
    {
        public const int PannelHeight = 32;
        public const int MinActionMenuWidth = 90;
        public int ActionMenuWidth;

        public readonly string Name;
        protected readonly List<ActionPanel> ListNextChoice;//Array of choices to display.
        public int ActionMenuCursor;
        private readonly bool CanCancel;
        public bool IsEnabled;
        public bool SendBackToSender;

        // List of ActionPanel shared through every ActionPanel objects.
        protected readonly ActionPanelHolder ListActionMenuChoice;

        public int BaseMenuX;
        public int BaseMenuY;
        public int FinalMenuX;
        public int FinalMenuY;
        public int MenuHeight;

        public ActionPanel(string Name, ActionPanelHolder ListActionMenuChoice, bool CanCancel)
        {
            this.Name = Name;
            this.ListActionMenuChoice = ListActionMenuChoice;
            this.CanCancel = CanCancel;

            ActionMenuWidth = MinActionMenuWidth;
            IsEnabled = true;
            SendBackToSender = false;

            ListNextChoice = new List<ActionPanel>();
            if (ListActionMenuChoice.HasMainPanel)
            {
                ActionPanel LastPanel = ListActionMenuChoice.Last();

                BaseMenuX = LastPanel.BaseMenuX;
                BaseMenuY = LastPanel.BaseMenuY;
                FinalMenuX = LastPanel.BaseMenuX;
                FinalMenuX = LastPanel.BaseMenuY;
            }
        }

        public virtual void AddChoiceToCurrentPanel(ActionPanel Panel)
        {
            ListNextChoice.Add(Panel);
        }

        public virtual void AddChoicesToCurrentPanel(ActionPanel[] ArrayPanel)
        {
            ListNextChoice.AddRange(ArrayPanel);
        }

        public void AddToPanelListAndSelect(ActionPanel Panel)
        {
            ListActionMenuChoice.AddToPanelListAndSelect(Panel);
        }

        public void AddToPanelList(ActionPanel Panel)
        {
            ListActionMenuChoice.Add(Panel);
        }

        public void RemoveFromPanelList(ActionPanel Panel)
        {
            ListActionMenuChoice.Remove(Panel);
        }

        public void ReplaceChoiceInCurrentPanel(ActionPanel Panel, Type PanelToReplace)
        {
            int CurrentIndex = -1;

            for (int P = 0; P < ListActionMenuChoice.Last().ListNextChoice.Count; P++)
            {
                if (ListActionMenuChoice.Last().ListNextChoice[P].GetType() == PanelToReplace)
                {
                    CurrentIndex = P;
                    break;
                }
            }

            if (CurrentIndex >= 0)
                ListActionMenuChoice.Last().ListNextChoice[CurrentIndex] = Panel;
        }

        /// <summary>
        /// This will crash the game if an action panel is not added immediately after.
        /// </summary>
        public void RemoveAllActionPanels()
        {
            ListActionMenuChoice.RemoveAllActionPanels();
        }

        public void RemoveAllSubActionPanels()
        {
            ListActionMenuChoice.RemoveAllSubActionPanels();
        }

        public override string ToString()
        {
            return Name;
        }

        public abstract void OnSelect();

        public void Update(GameTime gameTime)
        {
            if (CanCancel && InputHelper.InputCancelPressed())
            {
                CancelPanel();
            }
            else
            {
                DoUpdate(gameTime);
            }
        }

        /// <summary>
        /// UpdatePassive is called when Update is not
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void UpdatePassive(GameTime gameTime)
        {
        }

        public abstract void DoUpdate(GameTime gameTime);

        public void CancelPanel()
        {
            OnCancelPanel();

            RemoveFromPanelList(this);

            if (ListActionMenuChoice.HasSubPanels)
            {
                ListActionMenuChoice.Last().OnSelect();
            }
        }

        public List<ActionPanel> GetActionPanels()
        {
            return ListActionMenuChoice.GetActionPanels();
        }

        protected abstract void OnCancelPanel();

        public static ActionPanel Read(ByteReader BR, Dictionary<string, ActionPanel> DicActionPanel)
        {
            string NewActionPanelName = BR.ReadString();

            ActionPanel NewActionPanel = DicActionPanel[NewActionPanelName].Copy();

            NewActionPanel.DoRead(BR);

            int ListActionPanelCount = BR.ReadInt32();
            ActionPanel[] ListActionPanel = new ActionPanel[ListActionPanelCount];
            for (int A = 0; A < ListActionPanelCount; ++A)
            {
                ListActionPanel[A] = Read(BR, DicActionPanel);
            }

            NewActionPanel.AddChoicesToCurrentPanel(ListActionPanel);

            return NewActionPanel;

        }

        public abstract void DoRead(ByteReader BR);

        public virtual void ExecuteUpdate(byte[] ArrayUpdateData)
        {
        }

        public void Write(ByteWriter BW)
        {
            BW.AppendString(Name);
            DoWrite(BW);
            BW.AppendInt32(ListNextChoice.Count);
            foreach (ActionPanel ActiveNextActionPanel in ListNextChoice)
            {
                ActiveNextActionPanel.Write(BW);
            }
        }

        public abstract void DoWrite(ByteWriter BW);

        public void WriteUpdate(ByteWriter BW)
        {
            BW.AppendString(Name);
            BW.AppendByteArray(DoWriteUpdate());
        }

        public virtual byte[] DoWriteUpdate()
        {
            return new byte[0];
        }

        protected abstract ActionPanel Copy();

        public virtual void BeginDraw(CustomSpriteBatch g)
        {
        }

        public abstract void Draw(CustomSpriteBatch g);
    }
}
