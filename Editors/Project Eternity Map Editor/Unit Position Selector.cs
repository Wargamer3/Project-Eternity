using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class Unit_Position_Selector : BaseEditor
    {
        private enum ItemSelectionChoices { Unit, Character };
        ItemSelectionChoices ItemSelectionChoice;
        SpawnUnitPositionEvent Event;

        public Unit_Position_Selector(SpawnUnitPositionEvent Event)
        {
            InitializeComponent();
            this.Event = Event;
            for (int P = 0; P < Event.ListPosition.Count; P++)
                lstPosition.Items.Add(Event.ListPosition[P].X + ", " + Event.ListPosition[P].Y);
            txtUnit.Text = Event.UnitName;
            for (int P = 0; P < Event.ListCharacterName.Count; P++)
                lstPosition.Items.Add(Event.ListCharacterName[P]);
        }
        public Unit_Position_Selector()
        {
            InitializeComponent();
        }
        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public void AddPosition(Point NewPosition)
        {
            lstPosition.Items.Add(NewPosition.X + ", " + NewPosition.Y);
            Event.ListPosition.Add(new Microsoft.Xna.Framework.Point(NewPosition.X, NewPosition.Y));
        }

        public void RemovePosition(int Index)
        {
            lstPosition.Items.RemoveAt(Index);
            Event.ListPosition.RemoveAt(Index);
        }

        private void bthChooseUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitsNormal, null, false));
        }

        private void btnAddCharacter_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Character;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int Index;
            string CharacterName;

            if (lstCharacters.SelectedIndex > 0)
            {
                CharacterName = (string)lstCharacters.Items[lstCharacters.SelectedIndex];
                Index = lstCharacters.SelectedIndex - 1;

                lstCharacters.Items.RemoveAt(lstCharacters.SelectedIndex);
                //Selected Index is now -1.
                lstCharacters.Items.Insert(Index, CharacterName);

                lstCharacters.SelectedIndex = Index;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            string CharacterName;
            int Index;

            if (lstCharacters.SelectedIndex >= 0 && lstCharacters.SelectedIndex < lstCharacters.Items.Count - 1)
            {
                CharacterName = (string)lstCharacters.Items[lstCharacters.SelectedIndex];
                Index = lstCharacters.SelectedIndex + 1;

                lstCharacters.Items.RemoveAt(lstCharacters.SelectedIndex);
                //Selected Index is now -1.
                lstCharacters.Items.Insert(Index, CharacterName);

                lstCharacters.SelectedIndex = Index;
            }
        }

        private void btnRemoveCharacter_Click(object sender, EventArgs e)
        {
            lstCharacters.Items.RemoveAt(lstCharacters.SelectedIndex);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Unit:
                        break;

                    case ItemSelectionChoices.Character:
                        Character NewCharacter = new Character(Items[I]);
                        if (NewCharacter != null)
                            lstCharacters.Items.Add(NewCharacter.Name);
                        Event.ListCharacterName.Add(NewCharacter.Name);
                        break;
                }
            }
        }
    }
}