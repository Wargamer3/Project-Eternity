using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.Editors.PartsEditor
{
    public partial class ProjectEternityStandardPartEditor : BaseEditor
    {
        private UnitStandardPart ActiveSkill;
        private bool AllowEvents;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        private Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;

        public ProjectEternityStandardPartEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();

            AllowEvents = true;
            cboEffectType.Items.AddRange(DicEffect.Values.ToArray());
            cboRequirementType.Items.AddRange(DicRequirement.Values.ToArray());
        }

        public ProjectEternityStandardPartEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                ActiveSkill = new UnitStandardPart();
                SaveItem(FilePath, FilePath);
            }

            LoadPart(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnitParts, GUIRootPathUnitStandardParts }, "Units/Standard Parts/", new string[] { ".pep" }, typeof(ProjectEternityStandardPartEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtDescription.Text);

            BW.Write(ActiveSkill.Skill.ListSkillLevel[0].ActivationsCount);
            BW.Write(ActiveSkill.Skill.ListSkillLevel[0].ListActivation.Count);
            for (int R = 0; R < ActiveSkill.Skill.ListSkillLevel[0].ListActivation.Count; R++)
            {
                ActiveSkill.Skill.ListSkillLevel[0].ListActivation[R].Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadPart(string PartPath)
        {
            string Name = FilePath.Substring(0, FilePath.Length - 4).Substring(29);
            this.Text = Name + " - Project Eternity Standard Part Editor";

            ActiveSkill = new UnitStandardPart(PartPath, DicRequirement, DicEffect, DicAutomaticSkillTarget);

            txtDescription.Text = ActiveSkill.Skill.Description;

            for (int L = 0; L < ActiveSkill.Skill.ListSkillLevel[0].ListActivation.Count; L++)
            {
                lstActivations.Items.Add("Activation " + (lstActivations.Items.Count + 1));
            }
        }

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0 && lstRequirements.SelectedItems.Count > 0)
            {
                BaseSkillRequirement NewSkillRequirement = ((BaseSkillRequirement)cboRequirementType.SelectedItem).Copy();

                ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex] = NewSkillRequirement;
                lstRequirements.Items[lstRequirements.SelectedIndex] = NewSkillRequirement.SkillRequirementName;
            }
        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lstActivations.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
                BaseEffect OldSkillEffect = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListEffect[lstEffects.SelectedIndex];

                NewSkillEffect.CopyMembers(OldSkillEffect);

                ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListEffect[lstEffects.SelectedIndex] = NewSkillEffect;
                lstEffects.Items[lstEffects.SelectedIndex] = NewSkillEffect.ToString();
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void btnAddActivation_Click(object sender, EventArgs e)
        {
            ActiveSkill.Skill.ListSkillLevel[0].ListActivation.Add(new BaseSkillActivation());
            lstActivations.Items.Add("Activation " + (lstActivations.Items.Count + 1));
            lstActivations.SelectedIndex = lstActivations.Items.Count - 1;
        }

        private void btnRemoveActivation_Click(object sender, EventArgs e)
        {
            if (lstRequirements.SelectedItems.Count > 0)
            {
                lstRequirements.Items.RemoveAt(lstRequirements.SelectedIndex);
            }
        }

        private void btnAddRequirement_Click(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0)
            {
                ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListRequirement.Add(new PassiveRequirement());
                lstRequirements.Items.Add("Passive activation");
                lstRequirements.SelectedIndex = lstRequirements.Items.Count - 1;
            }
        }

        private void btnRemoveRequirement_Click(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0 && lstRequirements.SelectedItems.Count > 0)
            {
                lstRequirements.Items.RemoveAt(lstRequirements.SelectedIndex);
            }
        }

        private void btnAddEffects_Click(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex];
                BaseEffect NewEffect = DicEffect.First().Value;
                NewEffect.LifetimeType = SkillEffect.LifetimeTypePermanent;

                ActiveSkillActivation.ListEffect.Add(NewEffect);
                ActiveSkillActivation.ListEffectTarget.Add(new List<string>());
                lstEffects.Items.Add("Auto Dodge effect");
                lstEffects.SelectedIndex = lstEffects.Items.Count - 1;
            }
        }

        private void btnRemoveEffect_Click(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex];

                ActiveSkillActivation.ListEffect.RemoveAt(lstEffects.SelectedIndex);
                ActiveSkillActivation.ListEffectTarget.RemoveAt(lstEffects.SelectedIndex);
                lstEffects.Items.RemoveAt(lstEffects.SelectedIndex);
            }
        }

        private void lstActivations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0)
            {
                gbRequirements.Enabled = true;
                gbEffects.Enabled = true;
                lstRequirements.Items.Clear();
                lstEffects.Items.Clear();

                BaseSkillActivation ActiveSkillActivation = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex];
                for (int A = 0; A < ActiveSkillActivation.ListRequirement.Count; A++)
                {
                    lstRequirements.Items.Add(ActiveSkillActivation.ListRequirement[A].SkillRequirementName);
                }
                for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; E++)
                {
                    lstEffects.Items.Add(ActiveSkillActivation.ListEffect[E].ToString());
                }
                if (lstRequirements.Items.Count > 0)
                    lstRequirements.SelectedIndex = 0;
                if (lstEffects.Items.Count > 0)
                    lstEffects.SelectedIndex = 0;
            }
            else
            {
                gbRequirements.Enabled = false;
                gbEffects.Enabled = false;
            }
        }

        private void lstRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRequirements.SelectedItems.Count > 0)
            {
                cboRequirementType.Enabled = true;
                pgRequirement.Enabled = true;

                pgRequirement.SelectedObject = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex];
                cboRequirementType.Text = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex].SkillRequirementName;
            }
            else
            {
                cboRequirementType.Enabled = false;
                pgRequirement.Enabled = false;
            }
        }

        private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;

            if (lstEffects.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0)
            {
                gbAffectedTypes.Enabled = true;
                gbLifetimeTypes.Enabled = true;

                BaseSkillActivation ActiveSkillActivation = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex];
                BaseEffect ActiveEffect = ActiveSkillActivation.ListEffect[lstEffects.SelectedIndex];
                List<string> ListEffectActivation = ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex];
                pgEffect.SelectedObject = ActiveEffect;
                cboEffectType.Text = pgEffect.SelectedObject.ToString();

                //Affect types.
                cbAffectSelf.Checked = ListEffectActivation.Contains(cbAffectSelf.Text);
                cbAffectSquad.Checked = ListEffectActivation.Contains(cbAffectSquad.Text);
                cbAffectAura.Checked = ListEffectActivation.Contains(cbAffectAura.Text);
                cbAffectEnemy.Checked = ListEffectActivation.Contains(cbAffectEnemy.Text);
                cbAffectSquadEnemy.Checked = ListEffectActivation.Contains(cbAffectSquadEnemy.Text);
                cbAffectAuraEnemy.Checked = ListEffectActivation.Contains(cbAffectAuraEnemy.Text);
                cbAffectAll.Checked = ListEffectActivation.Contains(cbAffectAll.Text);
                cbAffectALLAllies.Checked = ListEffectActivation.Contains(cbAffectALLAllies.Text);
                cbAffectAllEnemy.Checked = ListEffectActivation.Contains(cbAffectAllEnemy.Text);

                txtRangeValue.Enabled = cbAffectAura.Checked | cbAffectAuraEnemy.Checked;
                if (txtRangeValue.Enabled)
                    txtRangeValue.Value = ActiveEffect.Range;

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

                if (ActiveEffect.IsStacking)
                {
                    cbLifetimeStacking.Checked = true;
                    txtMaximumStack.Enabled = true;
                    txtMaximumStack.Value = ActiveEffect.MaximumStack;
                }
                else
                {
                    cbLifetimeStacking.Checked = false;
                    txtMaximumStack.Value = 0;
                    txtMaximumStack.Enabled = false;
                }
            }
            else
            {
                gbAffectedTypes.Enabled = false;
                gbLifetimeTypes.Enabled = false;
            }

            AllowEvents = true;
        }

        private void UpdateEffect(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lstEffects.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.Skill.ListSkillLevel[0].ListActivation[lstActivations.SelectedIndex];
                BaseEffect ActiveEffect = ActiveSkillActivation.ListEffect[lstEffects.SelectedIndex];

                ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Clear();

                if (cbAffectSelf.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectSelf.Text);
                if (cbAffectSquad.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectSquad.Text);
                if (cbAffectAura.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectAura.Text);
                if (cbAffectEnemy.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectEnemy.Text);
                if (cbAffectSquadEnemy.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectSquadEnemy.Text);
                if (cbAffectAuraEnemy.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectAuraEnemy.Text);
                if (cbAffectAll.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectAll.Text);
                if (cbAffectALLAllies.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectALLAllies.Text);
                if (cbAffectAllEnemy.Checked)
                    ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex].Add(cbAffectAllEnemy.Text);

                if (rbLifetimePermanent.Checked)
                {
                    ActiveEffect.LifetimeType = SkillEffect.LifetimeTypePermanent;
                    txtMaximumLifetime.Enabled = false;
                }
                else if (rbLifetimeTurns.Checked)
                {
                    ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeTurns;
                    ActiveEffect.LifetimeTypeValue = (int)txtMaximumLifetime.Value;
                    txtMaximumLifetime.Enabled = true;
                }
                else if (rbLifetimeBattle.Checked)
                {
                    ActiveEffect.LifetimeType = SkillEffect.LifetimeTypeBattle;
                    ActiveEffect.LifetimeTypeValue = (int)txtMaximumLifetime.Value;
                    txtMaximumLifetime.Enabled = true;
                }

                ActiveEffect.IsStacking = cbLifetimeStacking.Checked;
                if (ActiveEffect.IsStacking)
                {
                    ActiveEffect.MaximumStack = (int)txtMaximumStack.Value;
                    txtMaximumStack.Enabled = true;
                }
                else
                {
                    cbLifetimeStacking.Checked = false;
                    txtMaximumStack.Value = 0;
                    txtMaximumStack.Enabled = false;
                }
            }
        }
    }
}
