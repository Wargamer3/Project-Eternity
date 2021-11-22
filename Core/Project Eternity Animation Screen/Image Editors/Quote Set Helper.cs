using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class QuoteSetHelper : Form
    {
        public Quote ActiveQuote;
        
        public QuoteSetHelper(Quote ActiveQuote)
        {
            InitializeComponent();
            this.ActiveQuote = ActiveQuote;
            for (int Q = 0; Q < ActiveQuote.ListQuoteSet.Count; Q++)
            {
                lstQuoteSet.Items.Add("Quote Set " + (lstQuoteSet.Items.Count + 1));
            }

            lstQuoteSet.SelectedIndex = 0;

            if (ActiveQuote.Target == Quote.Targets.Attacker)
            {
                rbAttacker.Checked = true;
            }
            else if (ActiveQuote.Target == Quote.Targets.Defender)
            {
                rbDefender.Checked = true;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAttacker.Checked)
            {
                ActiveQuote.Target = Quote.Targets.Attacker;
            }
            else if (rbDefender.Checked)
            {
                ActiveQuote.Target = Quote.Targets.Defender;
            }
        }

        private void btnAddQuoteSet_Click(object sender, EventArgs e)
        {
            ActiveQuote.ListQuoteSet.Add(new QuoteSet());
            lstQuoteSet.Items.Add("Quote Set " + (lstQuoteSet.Items.Count + 1));
        }

        private void btnRemoveQuoteSet_Click(object sender, EventArgs e)
        {
            if (lstQuoteSet.SelectedIndex >= 0)
            {
                ActiveQuote.ListQuoteSet.RemoveAt(lstQuoteSet.SelectedIndex);
                lstQuoteSet.Items.RemoveAt(lstQuoteSet.Items.Count - 1);
                for (int Q = 0; Q < lstQuoteSet.Items.Count; Q++)
                {
                    lstQuoteSet.Items[Q] = "Quote Set " + (lstQuoteSet.Items.Count + 1);
                }
            }
        }

        private void lstQuoteSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQuoteSet.SelectedIndex >= 0)
            {
                ActiveQuote.SelectedQuoteSet = lstQuoteSet.SelectedIndex;
                if (ActiveQuote.ActiveQuoteSet.QuoteStyle == QuoteSet.QuoteStyles.BattleStart)
                {
                    rbQuoteSetMoveIn.Checked = true;
                }
                else if (ActiveQuote.ActiveQuoteSet.QuoteStyle == QuoteSet.QuoteStyles.Reaction)
                {
                    rbQuoteSetReaction.Checked = true;
                }
                else if (ActiveQuote.ActiveQuoteSet.QuoteStyle == QuoteSet.QuoteStyles.QuoteSet)
                {
                    rbQuoteSetQuoteSet.Checked = true;
                }
                else if (ActiveQuote.ActiveQuoteSet.QuoteStyle == QuoteSet.QuoteStyles.Custom)
                {
                    rbQuoteSetCustomText.Checked = true;
                }
            }
        }

        private void txtQuoteSet_TextChanged(object sender, EventArgs e)
        {
            ActiveQuote.ActiveQuoteSet.QuoteSetName = txtQuoteSetName.Text;
        }

        private void ckUseLast_CheckedChanged(object sender, EventArgs e)
        {
            ActiveQuote.ActiveQuoteSet.QuoteSetUseLast = ckUseLast.Checked;
        }

        private void rbQuoteChoiceRandom_CheckedChanged(object sender, EventArgs e)
        {
            if (lstQuoteSet.SelectedIndex >= 0)
            {
                ActiveQuote.ActiveQuoteSet.QuoteSetChoice = QuoteSet.QuoteSetChoices.Random;
                txtQuoteChoiceValue.Value = 0;
                txtQuoteChoiceValue.Enabled = false;
            }
        }

        private void rbQuoteChoiceFixed_CheckedChanged(object sender, EventArgs e)
        {
            if (lstQuoteSet.SelectedIndex >= 0)
            {
                ActiveQuote.ActiveQuoteSet.QuoteSetChoice = QuoteSet.QuoteSetChoices.Fixed;
                txtQuoteChoiceValue.Value = ActiveQuote.ActiveQuoteSet.QuoteSetChoiceValue;
                txtQuoteChoiceValue.Enabled = true;
            }
        }

        private void txtQuoteChoiceValue_ValueChanged(object sender, EventArgs e)
        {
            if (lstQuoteSet.SelectedIndex >= 0)
            {
                ActiveQuote.ActiveQuoteSet.QuoteSetChoiceValue = (int)txtQuoteChoiceValue.Value;
            }
        }

        private void rbQuoteSet_CheckedChanged(object sender, EventArgs e)
        {
            gbQuoteSet.Enabled = false;
            gbCustomText.Enabled = false;
            txtCustomText.Text = "";

            if (rbQuoteSetQuoteSet.Checked)
            {
                ActiveQuote.ActiveQuoteSet.QuoteStyle = QuoteSet.QuoteStyles.QuoteSet;
                gbQuoteSet.Enabled = true;
                txtQuoteSetName.Text = ActiveQuote.ActiveQuoteSet.QuoteSetName;
                ckUseLast.Checked = ActiveQuote.ActiveQuoteSet.QuoteSetUseLast;

                if (ActiveQuote.ActiveQuoteSet.QuoteSetChoice == QuoteSet.QuoteSetChoices.Random)
                {
                    rbQuoteChoiceRandom.Checked = true;
                }
                else if (ActiveQuote.ActiveQuoteSet.QuoteSetChoice == QuoteSet.QuoteSetChoices.Fixed)
                {
                    rbQuoteChoiceFixed.Checked = true;
                }
            }
            else if (rbQuoteSetCustomText.Checked)
            {
                ActiveQuote.ActiveQuoteSet.QuoteStyle = QuoteSet.QuoteStyles.Custom;
                gbCustomText.Enabled = true;
                txtCustomText.Text = ActiveQuote.ActiveQuoteSet.CustomText;
            }
            else if (rbQuoteSetMoveIn.Checked)
            {
                ActiveQuote.ActiveQuoteSet.QuoteStyle = QuoteSet.QuoteStyles.BattleStart;
            }
            else if (rbQuoteSetReaction.Checked)
            {
                ActiveQuote.ActiveQuoteSet.QuoteStyle = QuoteSet.QuoteStyles.Reaction;
            }
        }

        private void txtCustomText_TextChanged(object sender, EventArgs e)
        {
            ActiveQuote.ActiveQuoteSet.CustomText = txtCustomText.Text;
        }

        private void btnSelectPortrait_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVisualNovelCharacters, "Select a Portrait to use", false));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(8);
                if (Name != null)
                {
                    txtPortraitPath.Text = Name;
                    ActiveQuote.PortraitPath = Name;
                }
            }
        }
    }
}