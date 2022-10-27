using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.SkillChainEditor
{
    public partial class SkillChainEditor : BaseEditor
    {
        protected bool AllowEvent;

        public SkillChainEditor()
        {
            InitializeComponent();
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(tvSkills.Nodes.Count);
            foreach (TreeNode ActiveNode in tvSkills.Nodes)
            {
                BaseAutomaticSkill ActiveSkill = (BaseAutomaticSkill)ActiveNode.Tag;
                ActiveSkill.Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        private void tvSkills_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AllowEvent = false;
            TreeNode SelectedNode = e.Node;

            if (SelectedNode.Tag is BaseAutomaticSkill)
            {
                SelectSkill((BaseAutomaticSkill)SelectedNode.Tag);
            }
            else
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                }

                SelectSkill(ActiveSkill);

                lstEffects.SelectedIndex = e.Node.Parent.Nodes.IndexOf(e.Node);
            }

            AllowEvent = true;
        }

        private void SelectSkill(BaseAutomaticSkill ActiveSkill)
        {
            gbRequirements.Enabled = true;
            gbEffects.Enabled = true;
            lstEffects.Items.Clear();

            cboRequirementType.Text = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0].SkillRequirementName;
            pgRequirement.SelectedObject = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0];

            for (int E = 0; E < ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Count; E++)
            {
                lstEffects.Items.Add(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect[E].ToString());
            }

            if (lstEffects.Items.Count > 0)
            {
                lstEffects.SelectedIndex = 0;
            }
        }

        protected void CreateTree(BaseAutomaticSkill ActiveSkill, TreeNodeCollection Nodes)
        {
            TreeNode NewNode = new TreeNode(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0].SkillRequirementName);
            NewNode.Tag = ActiveSkill;
            Nodes.Add(NewNode);

            foreach (BaseEffect ActiveEffect in ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect)
            {
                TreeNode EffectNode = new TreeNode(ActiveEffect.EffectTypeName);
                NewNode.Nodes.Add(EffectNode);

                foreach (BaseAutomaticSkill ActiveFollowingSkill in ActiveEffect.ListFollowingSkill)
                {
                    CreateTree(ActiveFollowingSkill, EffectNode.Nodes);
                }
            }
        }

        private void tsmNewEffect_Click(object sender, EventArgs e)
        {
            TreeNode SelectedNode = tvSkills.SelectedNode;
            if (SelectedNode != null)
            {
                TreeNode ParentNode = null;
                if (SelectedNode.Tag is BaseAutomaticSkill)
                {
                    ParentNode = SelectedNode;
                }
                else
                {
                    ParentNode = SelectedNode.Parent;
                }

                BaseEffect NewEffect = ((BaseEffect)cboEffectType.Items[0]).Copy();
                BaseAutomaticSkill ActiveSkill = (BaseAutomaticSkill)ParentNode.Tag;
                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Add(NewEffect);
                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffectTarget.Add(new List<string>());

                TreeNode EffectNode = new TreeNode(NewEffect.EffectTypeName);
                ParentNode.Nodes.Insert(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Count - 1, EffectNode);
            }
        }

        private void tsmNewRequirement_Click(object sender, EventArgs e)
        {
            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill();
            NewSkill.Name = "Dummy Skill";
            NewSkill.ListSkillLevel.Add(new BaseSkillLevel());
            NewSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            NewSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(((BaseSkillRequirement)cboRequirementType.Items[0]).Copy());
            NewActivation.ListEffectTarget.Add(new List<string>());
            NewActivation.ListEffect.Add(((BaseEffect)cboEffectType.Items[0]).Copy());

            TreeNode SelectedNode = tvSkills.SelectedNode;
            TreeNode EffectNode = null;
            if (SelectedNode != null)
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                if (ActiveSkill != null)
                {
                    EffectNode = SelectedNode.Parent;
                }
                else
                {
                    while (ActiveSkill == null)
                    {
                        ActiveSkill = SelectedNode.Parent.Tag as BaseAutomaticSkill;
                        if (ActiveSkill != null)
                        {
                            EffectNode = SelectedNode;
                        }

                        SelectedNode = SelectedNode.Parent;
                    }
                }

                BaseEffect ActiveEffect = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex];
                ActiveEffect.ListFollowingSkill.Add(NewSkill);
            }

            if (EffectNode != null)
            {
                CreateTree(NewSkill, EffectNode.Nodes);
            }
            else
            {
                CreateTree(NewSkill, tvSkills.Nodes);
            }
        }

        private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode SelectedNode = tvSkills.SelectedNode;
            if (lstEffects.SelectedIndex >= 0 && SelectedNode != null)
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                }
                BaseEffect ActiveEffect = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex];

                cboEffectType.Text = ActiveEffect.ToString();
                pgEffect.SelectedObject = ActiveEffect;
            }
        }

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode SelectedNode = tvSkills.SelectedNode;
            if (SelectedNode != null && AllowEvent)
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                }

                BaseSkillRequirement NewSkillRequirement = ((BaseSkillRequirement)cboRequirementType.SelectedItem).Copy();
                pgRequirement.SelectedObject = NewSkillRequirement;

                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0] = NewSkillRequirement;
                SelectedNode.Text = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0].SkillRequirementName;
            }
        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode SelectedNode = tvSkills.SelectedNode;
            if (SelectedNode != null && AllowEvent)
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                }

                if (lstEffects.SelectedItems.Count > 0)
                {
                    BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
                    pgEffect.SelectedObject = NewSkillEffect;

                    ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex] = NewSkillEffect;
                    lstEffects.Items[lstEffects.SelectedIndex] = NewSkillEffect.ToString();
                    SelectedNode.Nodes[lstEffects.SelectedIndex].Text = NewSkillEffect.EffectTypeName;
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmExpendAll_Click(object sender, EventArgs e)
        {
            tvSkills.ExpandAll();
        }

        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            tvSkills.CollapseAll();
        }

        private void ComboEditor_Shown(object sender, EventArgs e)
        {
        }
    }
}
