using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.PartsEditor
{
    public partial class ProjectEternityConsumablePartEditor : BaseEditor
    {
        private bool AllowEvents;

        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;

        public ProjectEternityConsumablePartEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
        }

        public ProjectEternityConsumablePartEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadSkill(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnitParts, GUIRootPathUnitConsumableParts }, "Units/Consumable Parts/", new string[] { ".pep" }, typeof(ProjectEternityConsumablePartEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Project Eternity Consumable Part Editor";

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(Convert.ToInt32(txtRange.Text));

            #region Affect type

            if (rbAffectSelf.Checked)
                BW.Write(rbAffectSelf.Text);
            else if (rbAffectSelfSquad.Checked)
                BW.Write(rbAffectSelfSquad.Text);
            else if (rbAffectAlly.Checked)
                BW.Write(rbAffectAlly.Text);
            else if (rbAffectAllySquad.Checked)
                BW.Write(rbAffectAllySquad.Text);
            else if (rbAffectEnemy.Checked)
                BW.Write(rbAffectEnemy.Text);
            else if (rbAffectEnemySquad.Checked)
                BW.Write(rbAffectEnemySquad.Text);
            else if (rbAffectAllAllies.Checked)
                BW.Write(rbAffectAllAllies.Text);
            else if (rbAffectAllEnemies.Checked)
                BW.Write(rbAffectAllEnemies.Text);
            else if (rbAffectEveryone.Checked)
                BW.Write(rbAffectEveryone.Text);

            #endregion

            BW.Write(txtDescription.Text);

            BW.Write(lvEffects.Items.Count);
            for (int i = 0; i < lvEffects.Items.Count; i++)
                ((SkillEffect)lvEffects.Items[i].Tag).WriteEffect(BW);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Skill at selected path.
        /// </summary>
        /// <param name="SkillPath">Path from which to open the Skill.</param>
        private void LoadSkill(string SkillPath)
        {
            ManualSkill ActiveSkill = new ManualSkill(SkillPath, DicRequirement, DicEffect);

            txtRange.Text = ActiveSkill.Range.ToString();
            txtDescription.Text = ActiveSkill.Description;

            if (ActiveSkill.Target.TargetType == rbAffectSelf.Text)
                rbAffectSelf.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectSelfSquad.Text)
                rbAffectSelfSquad.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectAlly.Text)
                rbAffectAlly.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectAllySquad.Text)
                rbAffectAllySquad.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectEnemy.Text)
                rbAffectEnemy.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectEnemySquad.Text)
                rbAffectEnemySquad.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectAllAllies.Text)
                rbAffectAllAllies.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectAllEnemies.Text)
                rbAffectAllEnemies.Checked = true;
            else if (ActiveSkill.Target.TargetType == rbAffectEveryone.Text)
                rbAffectEveryone.Checked = true;

            for (int i = 0; i < ActiveSkill.ListEffect.Count; i++)
            {
                ListViewItem NewListViewItem = new ListViewItem(ActiveSkill.ListEffect[i].EffectTypeName);
                NewListViewItem.Tag = ActiveSkill.ListEffect[i];
                lvEffects.Items.Add(NewListViewItem);
            }
        }

        private void PopulateEffectType()
        {
            cboEffectType.Items.Add(new StatusEffect());
            cboEffectType.Items.Add(new WillEffect());
            cboEffectType.Items.Add(new WillLimitBreakEffect());
            cboEffectType.Items.Add(new SPEffect());
            cboEffectType.Items.Add(new MaxSPEffect());
            cboEffectType.Items.Add(new HitRateEffect());
            cboEffectType.Items.Add(new EvasionRateEffect());
            cboEffectType.Items.Add(new AutoDodgeEffect());
            cboEffectType.Items.Add(new BaseDamageEffect());
            cboEffectType.Items.Add(new FinalDamageEffect());
            cboEffectType.Items.Add(new NullifyDamageEffect());
            cboEffectType.Items.Add(new CriticalHitRateEffect());
            cboEffectType.Items.Add(new ENCostEffect());
            cboEffectType.Items.Add(new MaxAmmoEffect());
            cboEffectType.Items.Add(new MVEffect());
            cboEffectType.Items.Add(new PostMovementEffect());
            cboEffectType.Items.Add(new PostAttackEffect());
            cboEffectType.Items.Add(new ExtraMovementsPerTurnEffect());
            cboEffectType.Items.Add(new ActivateSpiritEffect());
            cboEffectType.Items.Add(new IgnoreEnemySkillEffect());
            cboEffectType.Items.Add(new HPRegenEffect());
            cboEffectType.Items.Add(new ENRegenEffect());
            cboEffectType.Items.Add(new AmmoRegenEffect());
            cboEffectType.Items.Add(new EXPEffect());
            cboEffectType.Items.Add(new MoneyEffect());
            cboEffectType.Items.Add(new AttackRangeEffect());
            cboEffectType.Items.Add(new DamageTakenEffect());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            BaseEffect NewEffect = DicEffect.First().Value.Copy();
            NewEffect.LifetimeType = SkillEffect.LifetimeTypePermanent;
            ListViewItem NewListViewItem = new ListViewItem(NewEffect.EffectTypeName);
            NewListViewItem.Tag = NewEffect;
            lvEffects.Items.Add(NewListViewItem);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvEffects.SelectedItems.Count == 0)
                return;

            lvEffects.Items.Remove(lvEffects.SelectedItems[0]);
        }

        private void lvEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;
            if (lvEffects.SelectedItems.Count == 0)
            {
                gbEffectInformation.Enabled = false;
                gbLifetimeTypes.Enabled = false;
                pgEffect.SelectedObject = null;
                AllowEvents = true;
                return;
            }

            pgEffect.SelectedObject = lvEffects.SelectedItems[0].Tag;

            cboEffectType.Items.Clear();
            PopulateEffectType();
            cboEffectType.Text = lvEffects.SelectedItems[0].Text;
            gbEffectInformation.Enabled = true;
            gbLifetimeTypes.Enabled = true;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            //Lifetime types.
            if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypePermanent)
            {
                rbLifetimePermanent.Checked = true;
                txtMaximumLifetime.Enabled = false;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                rbLifetimeTurns.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeBattle)
            {
                rbLifetimeBattle.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeOnHit)
            {
                rbLifetimeOnHit.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeOnEnemyHit)
            {
                rbLifetimeOnEnemyHit.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeOnAttack)
            {
                rbLifetimeOnAttack.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeOnEnemyAttack)
            {
                rbLifetimeOnEnemyAttack.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.LifetimeType == SkillEffect.LifetimeTypeOnAction)
            {
                rbLifetimeOnAction.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }

            if (ActiveEffect.IsStacking)
            {
                cbLifetimeStacking.Checked = true;
                txtMaximumStack.Value = ActiveEffect.MaximumStack;
                txtMaximumStack.Enabled = true;
            }
            else
                cbLifetimeStacking.Checked = false;

            AllowEvents = true;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
            BaseEffect OldSkillEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            NewSkillEffect.CopyMembers(OldSkillEffect);

            lvEffects.SelectedItems[0].Tag = NewSkillEffect;
            lvEffects.SelectedItems[0].Text = cboEffectType.Text;
            pgEffect.SelectedObject = NewSkillEffect;
        }

        #region Lifetime

        private void rbLifetimePermanent_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimePermanent.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypePermanent;
                txtMaximumLifetime.Enabled = false;
            }
        }

        private void rbLifetimeTurns_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeTurns.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeTurns;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeBattle_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeBattle.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeBattle;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeOnHit_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeOnHit.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeOnHit;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeOnEnemyHit_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeOnEnemyHit.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeOnEnemyHit;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeOnAttack_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeOnAttack.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeOnAttack;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeOnEnemyAttack_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeOnEnemyAttack.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeOnEnemyAttack;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeOnAction_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeOnAction.Checked)
            {
                ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeOnAction;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void cbLifetimeStacking_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            if (cbLifetimeStacking.Checked)
            {
                ActiveEffect.IsStacking = true;
                txtMaximumStack.Enabled = true;
            }
            else
            {
                ActiveEffect.IsStacking = false;
                txtMaximumStack.Text = "";
                txtMaximumStack.Enabled = false;
            }
        }

        private void txtMaximumLifetime_TextChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            ActiveEffect.LifetimeTypeValue = (int)txtMaximumLifetime.Value;
        }

        private void txtMaximumStack_ValueChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            SkillEffect ActiveEffect = (SkillEffect)lvEffects.SelectedItems[0].Tag;

            ActiveEffect.MaximumStack = (int)txtMaximumStack.Value;
        }

        #endregion
    }
}
