using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetSpellEditor
{
    public partial class SpellEditor : BaseEditor
    {
        private bool AllowEvents;

        public SpellEditor()
        {
            InitializeComponent();
            List<string> ListAllowedTargets = new List<string>() { "ManualSkillActivationSorcererStreet" };
            List<string> ListAllowedRequirements = new List<string>() { "SorcererStreetRequirement" };
            List<string> ListAllowedEffect = new List<string>() { "SorcererStreetEffect" };

            cboTargetType.Items.AddRange(ManualSkillTarget.DicDefaultTarget.Values.Where(T => ListAllowedTargets.Contains(T.GetType().BaseType.Name)).OrderBy(T => T.ToString()).ToArray());
            cboRequirementType.Items.AddRange(BaseSkillRequirement.DicDefaultRequirement.Values.Where(R => ListAllowedRequirements.Contains(R.GetType().BaseType.Name)).OrderBy(R => R.ToString()).ToArray());
            cboEffectType.Items.AddRange(BaseEffect.DicDefaultEffect.Values.Where(E => ListAllowedEffect.Contains(E.GetType().BaseType.Name)).OrderBy(E => E.ToString()).ToArray());
        }

        public SpellEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                cboTargetType.SelectedIndex = 0;
                SaveItem(FilePath, FilePath);
            }

            LoadSkill(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathSorcererStreetSpells }, "Sorcerer Street/Spells/", new string[] { ".pes" }, typeof(SpellEditor), true, null, true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Spell Editor";

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write((int)txtRange.Value);

            ((ManualSkillTarget)cboTargetType.SelectedItem).Save(BW);

            BW.Write(txtDescription.Text);

            BW.Write(lvRequirements.Items.Count);
            for (int R = 0; R < lvRequirements.Items.Count; R++)
                ((BaseSkillRequirement)lvRequirements.Items[R].Tag).Save(BW);

            BW.Write(lvEffects.Items.Count);
            for (int E = 0; E < lvEffects.Items.Count; E++)
               ((BaseEffect)lvEffects.Items[E].Tag).WriteEffect(BW);

            FS.Close();
            BW.Close();
        }

        private void LoadSkill(string SkillPath)
        {
            ManualSkill ActiveSkill = new ManualSkill(SkillPath, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = ActiveSkill.Name + " - Spell Editor";

            txtRange.Text = ActiveSkill.Range.ToString();
            txtDescription.Text = ActiveSkill.Description;
            cboTargetType.Text = ActiveSkill.Target.TargetType;

            for (int R = 0; R < ActiveSkill.ListRequirement.Count; R++)
            {
                ListViewItem NewListViewItem = new ListViewItem(ActiveSkill.ListRequirement[R].SkillRequirementName);
                NewListViewItem.Tag = ActiveSkill.ListRequirement[R];
                lvRequirements.Items.Add(NewListViewItem);
            }

            for (int E = 0; E < ActiveSkill.ListEffect.Count; E++)
            {
                ListViewItem NewListViewItem = new ListViewItem(ActiveSkill.ListEffect[E].EffectTypeName);
                NewListViewItem.Tag = ActiveSkill.ListEffect[E];
                lvEffects.Items.Add(NewListViewItem);
            }
        }

        private void cboTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgTargetType.SelectedObject = cboTargetType.SelectedItem;
        }

        private void btnAddRequirement_Click(object sender, EventArgs e)
        {
            BaseSkillRequirement NewRequirement = BaseSkillRequirement.DicDefaultRequirement.First().Value.Copy();
            ListViewItem NewListViewItem = new ListViewItem(NewRequirement.SkillRequirementName);
            NewListViewItem.Tag = NewRequirement;
            lvRequirements.Items.Add(NewListViewItem);
        }

        private void btnRemoveRequirement_Click(object sender, EventArgs e)
        {
            if (lvRequirements.SelectedItems.Count == 0)
                return;

            lvRequirements.Items.Remove(lvRequirements.SelectedItems[0]);
        }

        private void lvRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;
            if (lvRequirements.SelectedItems.Count == 0)
            {
                gbRequirementInformation.Enabled = false;
                AllowEvents = true;
                return;
            }

            pgRequirement.SelectedObject = lvRequirements.SelectedItems[0].Tag;
            cboRequirementType.Text = lvRequirements.SelectedItems[0].Text;

            gbRequirementInformation.Enabled = true;
        }

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvRequirements.SelectedItems.Count == 0)
                return;

            BaseSkillRequirement NewSkillEffect = ((BaseSkillRequirement)cboEffectType.SelectedItem).Copy();
            BaseSkillRequirement OldSkillRequirement = (BaseSkillRequirement)lvRequirements.SelectedItems[0].Tag;

            NewSkillEffect.CopyMembers(OldSkillRequirement);

            lvRequirements.SelectedItems[0].Tag = NewSkillEffect;
            lvRequirements.SelectedItems[0].Text = cboEffectType.Text;
            pgRequirement.SelectedObject = NewSkillEffect;

            AllowEvents = true;
        }

        private void btnAddEffect_Click(object sender, EventArgs e)
        {
            BaseEffect NewEffect = BaseEffect.DicDefaultEffect.First().Value.Copy();
            NewEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypePermanent;
            ListViewItem NewListViewItem = new ListViewItem(NewEffect.EffectTypeName);
            NewListViewItem.Tag = NewEffect;
            lvEffects.Items.Add(NewListViewItem);
            lvEffects.Items[lvEffects.Items.Count - 1].Selected = true;
            lvEffects.Select();
        }

        private void btnRemoveEffect_Click(object sender, EventArgs e)
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

            cboEffectType.Text = lvEffects.SelectedItems[0].Text;
            gbEffectInformation.Enabled = true;
            gbLifetimeTypes.Enabled = true;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            //Lifetime types.
            if (ActiveEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypePermanent)
            {
                rbLifetimePermanent.Checked = true;
                txtMaximumLifetime.Enabled = false;
            }
            else if (ActiveEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                rbLifetimeTurns.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.Lifetime[0].LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeBattle)
            {
                rbLifetimeBattle.Checked = true;
                txtMaximumLifetime.Value = ActiveEffect.Lifetime[0].LifetimeTypeValue;
                txtMaximumLifetime.Enabled = true;
            }
            else if (ActiveEffect.Lifetime[0].LifetimeType == ActionPanelCastlePhase.CastleReached)
            {
                rbCastleReached.Checked = true;
                txtMaximumLifetime.Enabled = false;
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

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimePermanent.Checked)
            {
                ActiveEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypePermanent;
                txtMaximumLifetime.Enabled = false;
            }
        }

        private void rbLifetimeTurns_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeTurns.Checked)
            {
                ActiveEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbLifetimeBattle_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            if (rbLifetimeBattle.Checked)
            {
                ActiveEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeBattle;
                txtMaximumLifetime.Enabled = true;
            }
        }

        private void rbCastleReached_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            if (rbCastleReached.Checked)
            {
                ActiveEffect.Lifetime[0].LifetimeType = ActionPanelCastlePhase.CastleReached;
                txtMaximumLifetime.Enabled = false;
            }
        }

        private void cbLifetimeStacking_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

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

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            ActiveEffect.Lifetime[0].LifetimeTypeValue = (int)txtMaximumLifetime.Value;
        }

        private void txtMaximumStack_ValueChanged(object sender, EventArgs e)
        {
            if (!AllowEvents || lvEffects.SelectedItems.Count == 0)
                return;

            BaseEffect ActiveEffect = (BaseEffect)lvEffects.SelectedItems[0].Tag;

            ActiveEffect.MaximumStack = (int)txtMaximumStack.Value;
        }

        #endregion

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }
    }
}
