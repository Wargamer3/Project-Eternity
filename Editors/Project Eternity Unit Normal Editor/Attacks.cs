using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Item;
using ProjectEternity.Editors.AttackEditor;

namespace ProjectEternity.Editors.UnitNormalEditor
{
    public partial class Attacks : Form
    {
        private enum ItemSelectionChoices { Attack, Animation };

        private ItemSelectionChoices ItemSelectionChoice;

        public string UnitName;
        private List<Attack> _ListAttack;
        public List<Attack> ListAttack
        {
            get
            {
                return _ListAttack;
            }
            set
            {
                _ListAttack = value;
                lstAttack.Items.Clear();
                for (int A = 0; A < _ListAttack.Count; A++)
                {
                    if (_ListAttack[A].IsExternal)
                    {
                        lstAttack.Items.Add(_ListAttack[A].ItemName);
                    }
                    else
                    {
                        lstAttack.Items.Add(new ProjectEternityAttackEditor(_ListAttack[A]));
                    }
                }
            }
        }

        public byte AttackUpgradesValueIndex;
        public byte AttackUpgradesCostIndex;
        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;

        public Attacks()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            _ListAttack = new List<Attack>();
            AttackUpgradesValueIndex = 0;
            AttackUpgradesCostIndex = 0;
        }

        private void btnCreateAttack_Click(object sender, EventArgs e)
        {
            Attack NewAttack = new Attack();
            NewAttack.ItemName = "New Item";
            NewAttack.FullName = "New Item";

            NewAttack.Animations.Start = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/Start");
            NewAttack.Animations.EndHit = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Hit");
            NewAttack.Animations.EndMiss = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Miss");
            NewAttack.Animations.EndDestroyed = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Destroyed");
            NewAttack.Animations.EndBlocked = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Blocked");
            NewAttack.Animations.EndParried = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Parried");
            NewAttack.Animations.EndShootDown = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Shoot Down");
            NewAttack.Animations.EndNegated = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Negated");

            _ListAttack.Add(NewAttack);
            lstAttack.Items.Add(new ProjectEternityAttackEditor());
        }

        private void btnRemoveAttack_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0)
            {
                _ListAttack.RemoveAt(lstAttack.SelectedIndex);
                lstAttack.Items.RemoveAt(lstAttack.SelectedIndex);
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex - 1 >= 0)
            {
                int CurrentIndex = lstAttack.SelectedIndex;
                int NewIndex = lstAttack.SelectedIndex - 1;

                object ListItemToMove = lstAttack.Items[CurrentIndex];
                Attack AttackToMove = ListAttack[CurrentIndex];

                lstAttack.Items.RemoveAt(CurrentIndex);
                ListAttack.RemoveAt(CurrentIndex);

                lstAttack.Items.Insert(NewIndex, ListItemToMove);
                ListAttack.Insert(NewIndex, AttackToMove);

                lstAttack.SelectedIndex = NewIndex;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttack.SelectedIndex + 1 < lstAttack.Items.Count)
            {
                int CurrentIndex = lstAttack.SelectedIndex;
                int NewIndex = lstAttack.SelectedIndex + 1;

                object ListItemToMove = lstAttack.Items[CurrentIndex];
                Attack AttackToMove = ListAttack[CurrentIndex];

                lstAttack.Items.RemoveAt(CurrentIndex);
                ListAttack.RemoveAt(CurrentIndex);

                lstAttack.Items.Insert(NewIndex, ListItemToMove);
                ListAttack.Insert(NewIndex, AttackToMove);

                lstAttack.SelectedIndex = NewIndex;
            }
        }

        private void btnAddAttack_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Attack;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttacks));
        }

        private void btnSelectAnimation_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttackAnimations.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Animation;
                ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimations));
            }
        }

        private void lstAttack_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstAttackAnimations.SelectedIndex = -1;
        }

        private void lstAttack_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttack.Items[lstAttack.SelectedIndex] is ProjectEternityAttackEditor)
            {
                ProjectEternityAttackEditor ActiveEditor = (ProjectEternityAttackEditor)lstAttack.Items[lstAttack.SelectedIndex];
                ActiveEditor.ShowDialog();

                _ListAttack[lstAttack.SelectedIndex].ItemName = ActiveEditor.txtName.Text;
                _ListAttack[lstAttack.SelectedIndex].FullName = ActiveEditor.txtName.Text;
                //Refresh text
                lstAttack.Items[lstAttack.SelectedIndex] = lstAttack.SelectedItem;
            }
        }

        private void lstAttackAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttackAnimations.SelectedIndex >= 0)
            {
                txtAnimationName.Text = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackAnimations.SelectedIndex].AnimationName;
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
                    case ItemSelectionChoices.Attack:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(16);
                        if (Name != null)
                        {
                            if (lstAttack.Items.Contains(Name))
                            {
                                MessageBox.Show("This attack is already listed.\r\n" + Name);
                                return;
                            }
                            Attack NewAttack = new Attack(Name, DicRequirement, DicEffect);

                            NewAttack.Animations.Start = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/Start");
                            NewAttack.Animations.EndHit = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Hit");
                            NewAttack.Animations.EndMiss = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Miss");
                            NewAttack.Animations.EndDestroyed = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Destroyed");
                            NewAttack.Animations.EndBlocked = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Blocked");
                            NewAttack.Animations.EndParried = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Parried");
                            NewAttack.Animations.EndShootDown = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Shoot Down");
                            NewAttack.Animations.EndNegated = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Negated");

                            _ListAttack.Add(NewAttack);
                            lstAttack.Items.Add(Name);
                        }
                        break;

                    case ItemSelectionChoices.Animation:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(19);
                        _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackAnimations.SelectedIndex] = new Core.Units.AnimationInfo(Name);
                        txtAnimationName.Text = Name;
                        break;
                }
            }
        }

        private void cbUpgradeValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            AttackUpgradesValueIndex = (byte)cbUpgradeValues.SelectedIndex;
        }

        private void cbUpgradeCost_SelectedIndexChanged(object sender, EventArgs e)
        {
            AttackUpgradesCostIndex = (byte)cbUpgradeCost.SelectedIndex;
        }
    }
}
