using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Item;
using ProjectEternity.Editors.AttackEditor;
using static ProjectEternity.Core.Attacks.AttackContext;

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
        }

        public byte AttackUpgradesValueIndex;
        public byte AttackUpgradesCostIndex;

        public Attacks()
        {
            InitializeComponent();

            _ListAttack = new List<Attack>();
            AttackUpgradesValueIndex = 0;
            AttackUpgradesCostIndex = 0;
        }

        public void SetAttacks(List<Attack> ListAttack)
        {
            _ListAttack = ListAttack;
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

        private void btnCreateAttack_Click(object sender, EventArgs e)
        {
            if (lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            Attack NewAttack = new Attack("New Item");
            NewAttack.ItemName = "New Item";

            NewAttack.Animations[0].Animations.Start = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/Start");
            NewAttack.Animations[0].Animations.EndHit = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Hit");
            NewAttack.Animations[0].Animations.EndMiss = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Miss");
            NewAttack.Animations[0].Animations.EndDestroyed = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Destroyed");
            NewAttack.Animations[0].Animations.EndBlocked = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Blocked");
            NewAttack.Animations[0].Animations.EndParried = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Parried");
            NewAttack.Animations[0].Animations.EndShootDown = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Shoot Down");
            NewAttack.Animations[0].Animations.EndNegated = new Core.Units.AnimationInfo(UnitName + "/" + Name + "/End Negated");

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

        private void lstAttack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1)
            {
                gbAttackContexts.Enabled = false;
                return;
            }

            Attack ActiveAttack = _ListAttack[lstAttack.SelectedIndex];

            gbAttackContexts.Enabled = true;

            lstAttackContexts.Items.Clear();
            foreach (AttackContext ActiveContext in ActiveAttack.Animations)
            {
                lstAttackContexts.Items.Add(ActiveContext.ContextName);
            }


            lstAttackAnimations.SelectedIndex = -1;
            lstAttackContexts.SelectedIndex = 0;
        }

        private void lstAttack_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttack.Items[lstAttack.SelectedIndex] is ProjectEternityAttackEditor)
            {
                ProjectEternityAttackEditor ActiveEditor = (ProjectEternityAttackEditor)lstAttack.Items[lstAttack.SelectedIndex];
                ActiveEditor.ShowDialog();

                _ListAttack[lstAttack.SelectedIndex].ItemName = ActiveEditor.txtName.Text;
                //Refresh text
                lstAttack.Items[lstAttack.SelectedIndex] = lstAttack.SelectedItem;
            }
        }

        #region Attack Context

        private void lstAttackContexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                gbAttackOriginContext.Enabled = false;
                gbAttackTargetContext.Enabled = false;
                gbAnimations.Enabled = false;
                return;
            }

            gbAttackOriginContext.Enabled = true;
            gbAttackTargetContext.Enabled = true;
            gbAnimations.Enabled = true;

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            txtAttackContextName.Text = ActiveContext.ContextName;
            txtAttackContextParserValue.Text = ActiveContext.ParserValue;

            cbAttackOriginType.SelectedIndex = (int)ActiveContext.AttackOriginType;
            cbAttackTargetType.SelectedIndex = (int)ActiveContext.AttackTargetType;

            txtAttackOriginCustomType.Text = ActiveContext.AttackOriginCustomType;
            txtAttackTargetCustomType.Text = ActiveContext.AttackTargetCustomType;
            txtAttackContextWeight.Value = ActiveContext.Weight;

            lstAttackAnimations.SelectedIndex = 0;
        }

        private void txtAttackContextName_TextChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.ContextName = txtAttackContextName.Text;
            lstAttackContexts.Items[lstAttackContexts.SelectedIndex] = txtAttackContextName.Text;
        }

        private void txtAttackContextParserValue_TextChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.ParserValue = txtAttackContextParserValue.Text;
        }

        private void txtAttackContextWeight_ValueChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.Weight = (int)txtAttackContextWeight.Value;
        }

        private void btnAddAttackContext_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1)
            {
                return;
            }

            Attack ActiveAttack = _ListAttack[lstAttack.SelectedIndex];
            AttackContext NewAttackContext = new AttackContext();
            NewAttackContext.Animations.Start = new Core.Units.AnimationInfo(Name + "/Start");
            NewAttackContext.Animations.EndHit = new Core.Units.AnimationInfo(Name + "/End Hit");
            NewAttackContext.Animations.EndMiss = new Core.Units.AnimationInfo(Name + "/End Miss");
            NewAttackContext.Animations.EndDestroyed = new Core.Units.AnimationInfo(Name + "/End Destroyed");
            NewAttackContext.Animations.EndBlocked = new Core.Units.AnimationInfo(Name + "/End Blocked");
            NewAttackContext.Animations.EndParried = new Core.Units.AnimationInfo(Name + "/End Parried");
            NewAttackContext.Animations.EndShootDown = new Core.Units.AnimationInfo(Name + "/End Shoot Down");
            NewAttackContext.Animations.EndNegated = new Core.Units.AnimationInfo(Name + "/End Negated");
            ActiveAttack.Animations.Add(NewAttackContext);
            lstAttackContexts.Items.Add("Any");
        }

        private void btnRemoveAttackContext_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.Items.Count <= 1)
            {
                return;
            }

            Attack ActiveAttack = _ListAttack[lstAttack.SelectedIndex];
            ActiveAttack.Animations.RemoveAt(lstAttackContexts.SelectedIndex);
            lstAttackContexts.Items.RemoveAt(lstAttackContexts.SelectedIndex);
        }

        private void cbAttackOriginType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.AttackOriginType = (ContextTypes)cbAttackOriginType.SelectedIndex;

            txtAttackOriginCustomType.ReadOnly = ActiveContext.AttackOriginType == ContextTypes.Custom;
        }

        private void cbAttackTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.AttackTargetType = (ContextTypes)cbAttackTargetType.SelectedIndex;
            txtAttackTargetCustomType.ReadOnly = ActiveContext.AttackTargetType == ContextTypes.Custom;
        }

        private void txtAttackOriginCustomType_TextChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.AttackOriginCustomType = txtAttackOriginCustomType.Text;
        }

        private void txtAttackTargetCustomType_TextChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex <= -1 || lstAttackContexts.SelectedIndex <= -1)
            {
                return;
            }

            AttackContext ActiveContext = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex];

            ActiveContext.AttackTargetCustomType = txtAttackTargetCustomType.Text;
        }

        #endregion

        private void lstAttackAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttackContexts.SelectedIndex >= 0 && lstAttackAnimations.SelectedIndex >= 0)
            {
                txtAnimationName.Text = _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex].Animations[lstAttackAnimations.SelectedIndex].AnimationName;
            }
        }

        private void btnSelectAnimation_Click(object sender, EventArgs e)
        {
            if (lstAttack.SelectedIndex >= 0 && lstAttackContexts.SelectedIndex >= 0 && lstAttackAnimations.SelectedIndex >= 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.Animation;
                ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimations));
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
                            string AttackName = Name;

                            if (UnitName.Contains(Name))
                            {
                                AttackName = Name.Substring(UnitName.Length + 1);
                            }

                            if (lstAttack.Items.Contains(Name))
                            {
                                MessageBox.Show("This attack is already listed.\r\n" + Name);
                                return;
                            }

                            Attack NewAttack = new Attack(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
                            NewAttack.Animations.Add(new AttackContext());

                            NewAttack.Animations[0].Animations.Start = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/Start");
                            NewAttack.Animations[0].Animations.EndHit = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Hit");
                            NewAttack.Animations[0].Animations.EndMiss = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Miss");
                            NewAttack.Animations[0].Animations.EndDestroyed = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Destroyed");
                            NewAttack.Animations[0].Animations.EndBlocked = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Blocked");
                            NewAttack.Animations[0].Animations.EndParried = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Parried");
                            NewAttack.Animations[0].Animations.EndShootDown = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Shoot Down");
                            NewAttack.Animations[0].Animations.EndNegated = new Core.Units.AnimationInfo(UnitName + "/Attacks/" + AttackName + "/End Negated");

                            _ListAttack.Add(NewAttack);
                            lstAttack.Items.Add(AttackName);
                        }
                        break;

                    case ItemSelectionChoices.Animation:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(19);
                        _ListAttack[lstAttack.SelectedIndex].Animations[lstAttackContexts.SelectedIndex].Animations[lstAttackAnimations.SelectedIndex] = new Core.Units.AnimationInfo(Name);
                        txtAnimationName.Text = Name;
                        break;
                }
            }
        }
    }
}
