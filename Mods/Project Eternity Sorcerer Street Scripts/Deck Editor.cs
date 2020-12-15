using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class DeckEditor : Form
    {
        public DeckEditor(object CurrentDeck)
        {
            InitializeComponent();
            
            foreach (string ActiveCard in (string[])CurrentDeck)
            {
                lstCards.Items.Add(ActiveCard);
            }
        }

        public string[] GetDeck()
        {
            string[] ArrayCard = new string[lstCards.Items.Count];
            for(int C = 0; C < lstCards.Items.Count; ++C)
            {
                ArrayCard[C] = lstCards.Items[C].ToString();
            }
            return ArrayCard;
        }

        private void btnAddCard_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSorcererStreetCards));
        }

        private void btnRemoveCard_Click(object sender, EventArgs e)
        {
            if (lstCards.SelectedIndex >= 0)
            {
                lstCards.Items.RemoveAt(lstCards.SelectedIndex);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;
            
            for (int I = 0; I < Items.Count; I++)
            {
                string Name = Items[I].Substring(0, Items[I].Length - 4).Substring(24);
                lstCards.Items.Add(Name);
            }
        }
    }
}
