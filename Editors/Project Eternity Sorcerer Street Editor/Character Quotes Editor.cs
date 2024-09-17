using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    public partial class CharacterQuotesEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Pilot, Portrait };

        private ItemSelectionChoices ItemSelectionChoice;

        public CharacterQuotesEditor()
        {
            InitializeComponent();

            ListViewItem NewItem;
            NewItem = new ListViewItem("Introduction");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Alliance Introduction");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Alliance Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Winning Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Winning Alliance Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Losing Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Major Losing Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Losing Alliance Banter");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Territory claiming");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Small chain");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Large chain");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Territory Level Up");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Territory High Level Up");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Successful Invasion");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Failed Invasion");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Successful Defense");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Failed Defense");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Small Money Loss");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Medium Money Loss");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Large Money Loss");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Small Money Gains");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Big Money Gains");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Opponent Achieve Objective");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Opponent Achieve Objective Alliance");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Achieve Objective");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Achieve Objective Alliance");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Won the match");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);

            NewItem = new ListViewItem("Won Alliance match");
            NewItem.Tag = new QuoteSet();
            lvBaseQuotes.Items.Add(NewItem);
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        private void lvBaseQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateQuotes();
        }

        private void UpdateQuotes()
        {
            lstQuotes.Items.Clear();

            txtQuoteEditor.Text = "";
            txtQuoteEditor.Enabled = false;

            if (lvBaseQuotes.SelectedIndices.Count == 1)
            {
                int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);

                lstQuotes.Items.Clear();

                txtQuoteEditor.Text = "";
                txtQuoteEditor.Enabled = false;

                QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                for (int I = 0; I < BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote.Count; I++)
                {
                    lstQuotes.Items.Add(BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote[I]);
                }
            }
        }

        #region Versus Quotes

        private void lsVersusQuotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateQuotes();
        }

        private void btnAddVersusQuote_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Pilot;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
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
                    int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                    int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);
                    QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                    txtPortraitPath.Text = BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListPortraitPath[lstQuotes.SelectedIndex];
                }
            }
        }

        private void btnAddQuote_Click(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1)
            {
                int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);
                QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                lstQuotes.Items.Add("New Quote");
                BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote.Add("New Quote");
                BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListPortraitPath.Add(string.Empty);
                lstQuotes.SelectedIndex = lstQuotes.Items.Count - 1;
            }
        }

        private void btnAddBatchQuote_Click(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1)
            {
                int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);
                QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                BatchQuoteForm AddBatchQuoteForm = new BatchQuoteForm();
                if (AddBatchQuoteForm.ShowDialog() == DialogResult.OK)
                {
                    foreach (string ActiveLine in AddBatchQuoteForm.txtQuotesToAdd.Lines)
                    {
                        lstQuotes.Items.Add(ActiveLine);
                        BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote.Add(ActiveLine);
                        BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListPortraitPath.Add(string.Empty);
                    }

                    lstQuotes.SelectedIndex = lstQuotes.Items.Count - 1;
                }
            }
        }

        private void btnRemoveQuote_Click(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1 && lstQuotes.SelectedIndex >= 0)
            {
                int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);
                QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote.RemoveAt(lstQuotes.SelectedIndex);
                lstQuotes.Items.RemoveAt(lstQuotes.SelectedIndex);
            }
        }

        private void txtQuoteEditor_TextChanged(object sender, EventArgs e)
        {
            if (lvBaseQuotes.SelectedIndices.Count == 1 && lstQuotes.SelectedIndex >= 0)
            {
                int MapIndex = Math.Max(0, lsMapQuotes.SelectedIndex);
                int VersusIndex = Math.Max(0, lsVersusQuotes.SelectedIndex);
                QuoteSet BaseQuote = (QuoteSet)lvBaseQuotes.SelectedItems[0].Tag;

                lstQuotes.Items[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
                BaseQuote.ListMapQuote[MapIndex].ListQuoteVersus[VersusIndex].ListQuote[lstQuotes.SelectedIndex] = txtQuoteEditor.Text;
            }
        }

        #endregion

        private void btnSelectPortrait_Click(object sender, EventArgs e)
        {
            if (lstQuotes.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Portrait;
                ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathVisualNovelCharacters));
            }
        }

        private void btnAddVersusQuote_Click_1(object sender, EventArgs e)
        {

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
                        txtPortraitPath.Text = Name;
                        break;
                }
            }
        }
    }
}
