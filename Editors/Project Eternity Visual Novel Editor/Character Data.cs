using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class CharacterData : Form
    {
        private readonly List<SpeakerPriority> ListSpeakerPriority;

        public CharacterData(List<SpeakerPriority> ListSpeakerPriority)
        {
            InitializeComponent();

            this.ListSpeakerPriority = ListSpeakerPriority;
            for (int S = 0; S < ListSpeakerPriority.Count; ++S)
            {
                lstSpeakerPriority.Items.Add(ListSpeakerPriority[S]);
            }
        }

        private void btnAddNewCharacter_Click(object sender, EventArgs e)
        {
            List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacters, "Chose a new character.");

            if (Items == null || Items.Count == 0)
                return;

            string FullName = Items[0].Substring(0, Items[0].Length - 4).Substring(19);

            SpeakerPriority NewSpeakerPriority = new SpeakerPriority(SpeakerPriority.PriorityTypes.Character, FullName);

            lstSpeakerPriority.Items.Add(NewSpeakerPriority);
            ListSpeakerPriority.Add(NewSpeakerPriority);
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            AddCharacterLocationForm NewAddCharacterLocationForm = new AddCharacterLocationForm();

            if (NewAddCharacterLocationForm.ShowDialog() == DialogResult.OK)
            {
                string PriorityName = NewAddCharacterLocationForm.txtX.Value + ":" + NewAddCharacterLocationForm.txtY.Value;
                SpeakerPriority NewSpeakerPriority = new SpeakerPriority(SpeakerPriority.PriorityTypes.Location, PriorityName);

                lstSpeakerPriority.Items.Add(NewSpeakerPriority);
                ListSpeakerPriority.Add(NewSpeakerPriority);
            }
        }

        private void btnAddID_Click(object sender, EventArgs e)
        {
            AddCharacterIDForm NewAddCharacterIDForm = new AddCharacterIDForm();

            if (NewAddCharacterIDForm.ShowDialog() == DialogResult.OK)
            {
                string PriorityName = NewAddCharacterIDForm.txtID.Value.ToString();
                SpeakerPriority NewSpeakerPriority = new SpeakerPriority(SpeakerPriority.PriorityTypes.ID, PriorityName);

                lstSpeakerPriority.Items.Add(NewSpeakerPriority);
                ListSpeakerPriority.Add(NewSpeakerPriority);
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstSpeakerPriority.SelectedIndex >= 0)
            {
                ListSpeakerPriority.RemoveAt(lstSpeakerPriority.SelectedIndex);
                lstSpeakerPriority.Items.RemoveAt(lstSpeakerPriority.SelectedIndex);
            }
        }
    }
}
