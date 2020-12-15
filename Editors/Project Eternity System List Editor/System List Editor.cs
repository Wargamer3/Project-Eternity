using ProjectEternity.Core.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ProjectEternity.Editors.SystemListEditor
{
    public partial class SystemListEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Parts, BuyableSkills, Spirits, Skills, Abilities };

        private ItemSelectionChoices ItemSelectionChoice;

        public SystemListEditor()
        {
            InitializeComponent();
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            StreamWriter SW = new StreamWriter("Content/Parts List.txt");

            foreach (var Part in lstParts.Items)
            {
                SW.WriteLine(Part);
            }

            SW.Close();
            SW = new StreamWriter("Content/Buyable Skills List.txt");

            foreach (var BuyableSkill in lstBuyableSkills.Items)
            {
                SW.WriteLine(BuyableSkill);
            }

            SW.Close();
            SW = new StreamWriter("Content/Spirits List.txt");

            foreach (var Spirit in lstSpirits.Items)
            {
                SW.WriteLine(Spirit);
            }

            SW.Close();
            SW = new StreamWriter("Content/Skills List.txt");

            foreach (var Skill in lstSkills.Items)
            {
                SW.WriteLine(Skill);
            }

            SW.Close();
            SW = new StreamWriter("Content/Abilities List.txt");

            foreach (var Ability in lstAbilities.Items)
            {
                SW.WriteLine(Ability);
            }

            SW.Close();
        }

        public void LoadData()
        {
            #region Parts

            if (File.Exists("Content/Parts List.txt"))
            {
                StreamReader SR = new StreamReader("Content/Parts List.txt");

                while (!SR.EndOfStream)
                {
                    string Line = SR.ReadLine();
                    lstParts.Items.Add(Line);
                }
                SR.Close();
            }
            else
            {
                IEnumerable<ItemContainer> Parts = GetItemsByRoot(GUIRootPathUnitParts);
                foreach (var Container in Parts)
                {
                    foreach (var Part in Container.ListItem)
                    {
                        lstParts.Items.Add(Part);
                    }
                }
            }

            #endregion

            #region Buyable Skills

            if (File.Exists("Content/Buyable Skills List.txt"))
            {
                StreamReader SR = new StreamReader("Content/Buyable Skills List.txt");

                while (!SR.EndOfStream)
                {
                    string Line = SR.ReadLine();
                    lstBuyableSkills.Items.Add(Line);
                }
                SR.Close();
            }
            else
            {
                IEnumerable<ItemContainer> BuyableSkills = GetItemsByRoot(GUIRootPathCharacterSkills);
                foreach (var Container in BuyableSkills)
                {
                    foreach (var Part in Container.ListItem.Keys)
                    {
                        if (Part == "None")
                            continue;

                        lstBuyableSkills.Items.Add(Part);
                    }
                }
            }

            #endregion

            #region Spirits

            if (File.Exists("Content/Spirits List.txt"))
            {
                StreamReader SR = new StreamReader("Content/Spirits List.txt");

                while (!SR.EndOfStream)
                {
                    string Line = SR.ReadLine();
                    lstSpirits.Items.Add(Line);
                }
                SR.Close();
            }
            else
            {
                IEnumerable<ItemContainer> Spirits = GetItemsByRoot(GUIRootPathCharacterSpirits);
                foreach (var Container in Spirits)
                {
                    foreach (var Part in Container.ListItem.Keys)
                    {
                        if (Part == "None")
                            continue;

                        lstSpirits.Items.Add(Part);
                    }
                }
            }

            #endregion

            #region Skills

            if (File.Exists("Content/Skills List.txt"))
            {
                StreamReader SR = new StreamReader("Content/Skills List.txt");

                while (!SR.EndOfStream)
                {
                    string Line = SR.ReadLine();
                    lstSkills.Items.Add(Line);
                }
                SR.Close();
            }
            else
            {
                IEnumerable<ItemContainer> Skills = GetItemsByRoot(GUIRootPathCharacterSkills);
                foreach (var Container in Skills)
                {
                    foreach (var Part in Container.ListItem.Keys)
                    {
                        if (Part == "None")
                            continue;

                        lstSkills.Items.Add(Part);
                    }
                }
            }

            #endregion

            #region Abilities

            if (File.Exists("Content/Abilities List.txt"))
            {
                StreamReader SR = new StreamReader("Content/Abilities List.txt");

                while (!SR.EndOfStream)
                {
                    string Line = SR.ReadLine();
                    lstAbilities.Items.Add(Line);
                }
                SR.Close();
            }
            else
            {
                IEnumerable<ItemContainer> Abilities = GetItemsByRoot(GUIRootPathUnitAbilities);
                foreach (var Container in Abilities)
                {
                    foreach (var Part in Container.ListItem.Keys)
                    {
                        if (Part == "None")
                            continue;

                        lstAbilities.Items.Add(Part);
                    }
                }
            }

            #endregion
        }

        private void RemoveItemFromList(ListBox Sender)
        {
            if (Sender.SelectedItem != null)
            {
                Sender.Items.Remove(Sender.SelectedItem);
            }
        }

        private void MoveUpItemFromList(ListBox Sender)
        {
            if (Sender.SelectedItem != null)
            {
                int Index = Sender.Items.IndexOf(Sender.SelectedItem);
                if (Index > 0)
                {
                    object SelectedItem = Sender.SelectedItem;
                    Sender.Items.Remove(Sender.SelectedItem);
                    Sender.Items.Insert(Index - 1, Sender.SelectedItem);
                }
            }
        }

        private void MoveDownItemFromList(ListBox Sender)
        {
            if (Sender.SelectedItem != null)
            {
                int Index = Sender.Items.IndexOf(Sender.SelectedItem);
                if (Index < Sender.Items.Count - 1)
                {
                    object SelectedItem = Sender.SelectedItem;
                    Sender.Items.Remove(Sender.SelectedItem);
                    Sender.Items.Insert(Index + 1, Sender.SelectedItem);
                }
            }
        }

        #region Parts

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Parts;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitParts));
        }

        private void btnRemoveParts_Click(object sender, EventArgs e)
        {
            RemoveItemFromList(lstParts);
        }

        private void btnMoveUpPart_Click(object sender, EventArgs e)
        {
            MoveUpItemFromList(lstParts);
        }

        private void btnMoveDownPart_Click(object sender, EventArgs e)
        {
            MoveDownItemFromList(lstParts);
        }

        #endregion

        #region Buyable Skill

        private void btnAddBuyableSkill_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BuyableSkills;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnRemoveBuyableSkill_Click(object sender, EventArgs e)
        {
            RemoveItemFromList(lstBuyableSkills);
        }

        private void btnMoveUpBuyableSkill_Click(object sender, EventArgs e)
        {
            MoveUpItemFromList(lstBuyableSkills);
        }

        private void btnMoveDownBuyableSkill_Click(object sender, EventArgs e)
        {
            MoveDownItemFromList(lstBuyableSkills);
        }

        #endregion

        #region Spirit

        private void btnAddSpirit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirits;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnRemoveSpirit_Click(object sender, EventArgs e)
        {
            RemoveItemFromList(lstSpirits);
        }

        private void btnMoveUpSpirit_Click(object sender, EventArgs e)
        {
            MoveUpItemFromList(lstSpirits);
        }

        private void btnMoveDownSpirit_Click(object sender, EventArgs e)
        {
            MoveDownItemFromList(lstSpirits);
        }

        #endregion

        #region Skills

        private void btnAddSkill_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skills;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnRemoveSkill_Click(object sender, EventArgs e)
        {
            RemoveItemFromList(lstSkills);
        }

        private void btnMoveUpSkill_Click(object sender, EventArgs e)
        {
            MoveUpItemFromList(lstSkills);
        }

        private void btnMoveDownSkill_Click(object sender, EventArgs e)
        {
            MoveDownItemFromList(lstSkills);
        }

        #endregion

        #region Abilities

        private void btnAddAbility_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Abilities;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitAbilities));
        }

        private void btnRemoveAbility_Click(object sender, EventArgs e)
        {
            RemoveItemFromList(lstAbilities);
        }

        private void btnMoveUpAbility_Click(object sender, EventArgs e)
        {
            MoveUpItemFromList(lstAbilities);
        }

        private void btnMoveDownAbility_Click(object sender, EventArgs e)
        {
            MoveDownItemFromList(lstAbilities);
        }

        #endregion

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Parts:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(29);
                        lstParts.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.BuyableSkills:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(15);
                        lstBuyableSkills.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Spirits:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(15);
                        lstSpirits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Skills:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(15);
                        lstSkills.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Abilities:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(15);
                        lstAbilities.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
