using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class CharacterQuotesEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Pilot, Portrait };

        private ItemSelectionChoices ItemSelectionChoice;

        public CharacterQuotesEditor()
        {
            InitializeComponent();

            ListViewItem NewItem;
            NewItem = new ListViewItem("Battle Start");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Dodge");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Damaged");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Destroyed");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Support Attack");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Support Defend");
            NewItem.Tag = new Character.QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        private void lvBaseQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1)
            {
                lstQuotes.Items.Clear();
                dgvQuoteSets.CurrentCell = null;

                txtQuoteEditor.Text = "";
                txtQuoteEditor.Enabled = false;

                List<string> Items;

                if (lsVersusQuotes.SelectedIndex >= 1)//Base + Versus quotes.
                    Items = ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuoteVersus;
                else//Base quotes.
                    Items = ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuote;

                for (int I = 0; I < Items.Count; I++)
                {
                    lstQuotes.Items.Add(Items[I]);
                }
            }
        }

        #region Versus Quotes

        private void lsVersusQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstQuotes.Items.Clear();

            txtQuoteEditor.Text = "";
            txtQuoteEditor.Enabled = false;

            //Pilot selected.
            if (lsVersusQuotes.SelectedIndex >= 1)
            {
                List<string> Items;

                if (lvBaseQuotes.SelectedIndices.Count == 1)//Base + Versus quotes.
                    Items = ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuoteVersus;
                else//Attack + Versus quotes.
                    Items = ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuoteVersus;

                for (int I = 0; I < Items.Count; I++)
                {
                    lstQuotes.Items.Add(Items[I]);
                }
            }
            else//No pilot selected.
            {
                List<string> Items;

                if (lvBaseQuotes.SelectedIndices.Count == 1)//Base + Versus quotes.
                    Items = ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuote;
                else//Attack + Versus quotes.
                    Items = ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuote;

                for (int I = 0; I < Items.Count; I++)
                {
                    lstQuotes.Items.Add(Items[I]);
                }
            }
        }

        private void btnAddVersusQuote_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Pilot;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacters));
        }

        private void btnDeleteVersusQuote_Click(object sender, EventArgs e)
        {
            if (lsVersusQuotes.SelectedIndex >= 1)
            {
                lsVersusQuotes.Items.RemoveAt(lsVersusQuotes.SelectedIndex);
            }
        }

        #endregion

        #region Quote interaction

        private void lstQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQuotes.SelectedIndex >= 0)
            {
                txtQuoteEditor.Enabled = true;
                txtQuoteEditor.Text = (string)lstQuotes.SelectedItem;

                if (lvBaseQuotes.SelectedIndices.Count == 1)
                {
                    txtPortraitPath.Text = ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).PortraitPath;
                }
                else if (dgvQuoteSets.CurrentRow != null && dgvQuoteSets.CurrentRow.Tag != null)
                {
                    txtPortraitPath.Text = ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).PortraitPath;
                }
            }
        }

        private void btnAddQuote_Click(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1)
            {
                lstQuotes.Items.Add("New Quote");

                if (lsVersusQuotes.SelectedIndex >= 1)//Base + Versus quotes.
                    ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuoteVersus.Add("New Quote");
                else//Base quotes.
                    ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuote.Add("New Quote");
            }
            else if (dgvQuoteSets.CurrentRow != null && dgvQuoteSets.CurrentRow.Tag != null)
            {
                lstQuotes.Items.Add("New Quote");

                if (lsVersusQuotes.SelectedIndex >= 1)//Attack + Versus quotes.
                    ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuoteVersus.Add("New Quote");
                else//Attack quotes.
                    ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuote.Add("New Quote");
            }
        }

        private void btnRemoveQuote_Click(object sender, EventArgs e)
        {
            if (lstQuotes.SelectedIndex >= 0)
            {
                if (lvBaseQuotes.SelectedIndices.Count == 1)
                {
                    if (lsVersusQuotes.SelectedIndex >= 1)//Base + Versus quotes.
                        ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuoteVersus.RemoveAt(lstQuotes.SelectedIndex);
                    else//Base quotes.
                        ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuote.RemoveAt(lstQuotes.SelectedIndex);
                }
                else if (dgvQuoteSets.CurrentRow != null && dgvQuoteSets.CurrentRow.Tag != null)
                {
                    if (lsVersusQuotes.SelectedIndex >= 1)//Attack + Versus quotes.
                        ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuoteVersus.RemoveAt(dgvQuoteSets.CurrentRow.Index);
                    else//Attack quotes.
                        ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuote.RemoveAt(dgvQuoteSets.CurrentRow.Index);
                }

                lstQuotes.Items.RemoveAt(lstQuotes.SelectedIndex);
            }
        }

        private void txtQuoteEditor_TextChanged(object sender, EventArgs e)
        {
            if (lstQuotes.SelectedIndex >= 0)
            {
                lstQuotes.Items[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                if (lvBaseQuotes.SelectedIndices.Count == 1)
                {
                    if (lsVersusQuotes.SelectedIndex >= 1)//Base + Versus quotes.
                        ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuoteVersus[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                    else//Base quotes.
                        ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).ListQuote[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                }
                else if (dgvQuoteSets.CurrentRow != null && dgvQuoteSets.CurrentRow.Tag != null)
                {
                    if (lsVersusQuotes.SelectedIndex >= 1)//Attack + Versus quotes.
                        ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuoteVersus[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                    else//Attack quotes.
                        ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuote[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                }
            }
        }

        #endregion

        private void btnSelectPortrait_Click(object sender, EventArgs e)
        {
            if (lstQuotes.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Portrait;
                ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathVisualNovelCharacters));
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Pilot:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Characters") + 11);
                        if (Name != null)
                        {
                            if (lsVersusQuotes.Items.Contains(Name))
                            {
                                MessageBox.Show("This pilot is already listed.\r\n" + Name);
                                return;
                            }

                            lsVersusQuotes.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.Portrait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(8);
                        if (lvBaseQuotes.SelectedIndices.Count == 1)
                        {
                            if (lsVersusQuotes.SelectedIndex >= 1)//Base + Versus quotes.
                                ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).PortraitPath = Name;
                            else//Base quotes.
                                ((Character.QuoteSet)lvBaseQuotes.SelectedItems[0].Tag).PortraitPath = Name;
                        }
                        else if (dgvQuoteSets.CurrentRow != null && dgvQuoteSets.CurrentRow.Tag != null)
                        {
                            if (lsVersusQuotes.SelectedIndex >= 1)//Attack + Versus quotes.
                                ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).PortraitPath = Name;
                            else//Attack quotes.
                                ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).PortraitPath = Name;
                        }
                        txtPortraitPath.Text = Name;
                        break;
                }
            }
        }

        private void dgvQuoteSets_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            dgvQuoteSets.CurrentRow.Tag = new Character.QuoteSet();
        }

        private void dgvQuoteSets_Click(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedItems.Count == 1)
                lvBaseQuotes.SelectedItems[0].Selected = false;
        }

        private void dgvQuoteSets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvQuoteSets.CurrentRow.IsNewRow)
                return;

            lstQuotes.Items.Clear();

            txtQuoteEditor.Text = "";
            txtQuoteEditor.Enabled = false;

            List<string> Items;

            if (lsVersusQuotes.SelectedIndex >= 1)//Attack + Versus quotes.
                Items = ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuoteVersus;
            else//Attack quotes.
                Items = ((Character.QuoteSet)dgvQuoteSets.CurrentRow.Tag).ListQuote;

            for (int I = 0; I < Items.Count; I++)
            {
                lstQuotes.Items.Add(Items[I]);
            }
        }
    }
}
