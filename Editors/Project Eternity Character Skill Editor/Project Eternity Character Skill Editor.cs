using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.Editors.CharacterSkillEditor
{
    public partial class ProjectEternityCharacterSkillEditor : BaseEditor
    {
        private BaseAutomaticSkill ActiveSkill;
        private bool AllowEvents;

        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;

        public ProjectEternityCharacterSkillEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            AllowEvents = true;
            cboEffectType.Items.AddRange(DicEffect.Values.OrderBy(x => x.EffectTypeName).ToArray());
            cboRequirementType.Items.AddRange(DicRequirement.Values.OrderBy(x => x.SkillRequirementName).ToArray());
        }

        public ProjectEternityCharacterSkillEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                ActiveSkill = new BaseAutomaticSkill();
                SaveItem(FilePath, FilePath);
            }

            LoadSkill(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathCharacterSkills }, "Characters/Skills/", new string[] { ".pecs" }, typeof(ProjectEternityCharacterSkillEditor), true, null, true),
                new EditorInfo(new string[] { GUIRootPathAttackAttributes }, "Attacks/Attributes/", new string[] { ".peaa" }, typeof(ProjectEternityCharacterSkillEditor), true, null, true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            ActiveSkill.Description = txtDescription.Text;

            ActiveSkill.Save(BW);

            FS.Close();
            BW.Close();
        }
        
        private void LoadSkill(string SkillPath)
        {
            string Name = FilePath.Substring(0, FilePath.Length - 5).Substring(26);
            this.Text = Name + " - Project Eternity Skill Editor";

            ActiveSkill = new BaseAutomaticSkill(SkillPath, DicRequirement, DicEffect);

            txtDescription.Text = ActiveSkill.Description;

            for (int L = 0; L < ActiveSkill.ListSkillLevel.Count; L++)
            {
                lstLevels.Items.Add("Level " + (lstLevels.Items.Count + 1));
            }

            if (lstLevels.Items.Count > 0)
            {
                lstLevels.SelectedIndex = 0;
            }
        }

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0 && lstRequirements.SelectedItems.Count > 0)
            {
                BaseSkillRequirement NewSkillRequirement = ((BaseSkillRequirement)cboRequirementType.SelectedItem).Copy();

                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex] = NewSkillRequirement;
                lstRequirements.Items[lstRequirements.SelectedIndex] = NewSkillRequirement.SkillRequirementName;
            }
        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
                BaseEffect OldSkillEffect = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListEffect[lstEffects.SelectedIndex];
                
                NewSkillEffect.CopyMembers(OldSkillEffect);

                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListEffect[lstEffects.SelectedIndex] = NewSkillEffect;
                lstEffects.Items[lstEffects.SelectedIndex] = NewSkillEffect.ToString();
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void btnAddLevel_Click(object sender, EventArgs e)
        {
            ActiveSkill.ListSkillLevel.Add(new BaseSkillLevel());
            lstLevels.Items.Add("Level " + (lstLevels.Items.Count + 1));

            lstLevels.SelectedIndex = lstLevels.Items.Count - 1;
        }

        private void btnRemoveLevel_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                lstLevels.Items.RemoveAt(lstLevels.SelectedIndex);
            }
        }

        private void btnAddActivation_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation.Add(new BaseSkillActivation());
                lstActivations.Items.Add("Activation " + (lstActivations.Items.Count + 1));
                lstActivations.SelectedIndex = lstActivations.Items.Count - 1;
            }
        }

        private void txtActivationChance_ValueChanged(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0)
            {
                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ActivationPercentage = (byte)txtActivationChance.Value;
            }
        }

        private void btnRemoveActivation_Click(object sender, EventArgs e)
        {
            if (lstActivations.SelectedItems.Count > 0)
            {
                lstActivations.Items.RemoveAt(lstActivations.SelectedIndex);
            }
        }

        private void btnAddRequirement_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0)
            {
                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListRequirement.Add(new PassiveRequirement());
                lstRequirements.Items.Add("Passive activation");
                lstRequirements.SelectedIndex = lstRequirements.Items.Count - 1;
            }
        }

        private void btnRemoveRequirement_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0 && lstRequirements.SelectedItems.Count > 0)
            {
                lstRequirements.Items.RemoveAt(lstRequirements.SelectedIndex);
            }
        }

        private void btnAddEffects_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex];
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
            if (lstLevels.SelectedItems.Count > 0 && lstActivations.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex];

                ActiveSkillActivation.ListEffect.RemoveAt(lstEffects.SelectedIndex);
                ActiveSkillActivation.ListEffectTarget.RemoveAt(lstEffects.SelectedIndex);
                lstEffects.Items.RemoveAt(lstEffects.SelectedIndex);
            }
        }

        private void lstLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                gbActivations.Enabled = true;
                lstActivations.Items.Clear();

                BaseSkillLevel ActiveSkillLevel = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex];
                txtUpgradePrice.Value = ActiveSkillLevel.Price;
                for (int A = 0; A < ActiveSkillLevel.ListActivation.Count; A++)
                {
                    lstActivations.Items.Add("Activation " + (lstActivations.Items.Count + 1));
                }

                if (lstActivations.Items.Count > 0)
                    lstActivations.SelectedIndex = 0;
                else
                {
                    lstRequirements.Items.Clear();
                    lstEffects.Items.Clear();
                }
            }
            else
            {
                gbActivations.Enabled = false;
            }
        }

        private void txtUpgradePrice_ValueChanged(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                gbActivations.Enabled = true;
                lstActivations.Items.Clear();

                BaseSkillLevel ActiveSkillLevel = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex];
                ActiveSkillLevel.Price = (int)txtUpgradePrice.Value;
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

                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex];
                txtActivationChance.Value = ActiveSkillActivation.ActivationPercentage;
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

                AllowEvents = false;

                pgRequirement.SelectedObject = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex];
                cboRequirementType.Text = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex].ListRequirement[lstRequirements.SelectedIndex].SkillRequirementName;

                AllowEvents = true;
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

                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex];
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
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[lstActivations.SelectedIndex];
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

                if (txtRangeValue.Enabled)
                    ActiveEffect.Range = (int)txtRangeValue.Value;

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
