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
                int Depth = 0;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                    ++Depth;
                }

                SelectSkill(ActiveSkill);

                if (Depth < lstRequirements.Items.Count)
                {
                    lstRequirements.SelectedIndex = Depth;
                }
                if (SelectedNode.Nodes.Count == 0)
                {
                    lstEffects.SelectedIndex = e.Node.Parent.Nodes.IndexOf(e.Node);
                }
            }

            AllowEvent = true;
        }

        private void SelectSkill(BaseAutomaticSkill ActiveSkill)
        {
            gbRequirements.Enabled = true;
            gbEffects.Enabled = true;
            lstRequirements.Items.Clear();
            lstEffects.Items.Clear();

            cboRequirementType.Text = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0].SkillRequirementName;
            pgRequirement.SelectedObject = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[0];

            for (int R = 0; R < ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement.Count; R++)
            {
                lstRequirements.Items.Add(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[R].ToString());
            }

            if (lstRequirements.Items.Count > 0)
            {
                lstRequirements.SelectedIndex = 0;
            }

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
            TreeNode NewNode = null;
            for (int R = 0; R < ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement.Count; ++R)
            {
                if (NewNode != null)
                {
                    Nodes = NewNode.Nodes;
                }
                NewNode = new TreeNode(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[R].SkillRequirementName);
                if (R == 0)
                {
                    NewNode.Tag = ActiveSkill;
                }
                Nodes.Add(NewNode);
            }

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
            TreeNode RootNode = tvSkills.SelectedNode;

            if (RootNode != null)
            {
                TreeNode RequirementNode = tvSkills.SelectedNode;
                BaseAutomaticSkill RootSkill = RootNode.Tag as BaseAutomaticSkill;
                while (RootSkill == null)
                {
                    RootNode = RootNode.Parent;
                    RootSkill = RootNode.Tag as BaseAutomaticSkill;
                }
                while (RequirementNode.Nodes.Count > 0)
                {
                    RequirementNode = RequirementNode.Nodes[0];
                }

                RequirementNode = RequirementNode.Parent;

                BaseEffect NewEffect = ((BaseEffect)cboEffectType.Items[0]).Copy();
                RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Add(NewEffect);
                RootSkill.ListSkillLevel[0].ListActivation[0].ListEffectTarget.Add(new List<string>());

                TreeNode EffectNode = new TreeNode(NewEffect.EffectTypeName);
                RequirementNode.Nodes.Insert(RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Count - 1, EffectNode);
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
            if (SelectedNode != null && SelectedNode.Parent != null)
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

        #region Requirements

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode RootNode = tvSkills.SelectedNode;
            if (RootNode != null && AllowEvent)
            {
                TreeNode RequirementNode = tvSkills.SelectedNode;
                BaseAutomaticSkill RootSkill = RootNode.Tag as BaseAutomaticSkill;
                while (RootSkill == null)
                {
                    RootNode = RootNode.Parent;
                    RootSkill = RootNode.Tag as BaseAutomaticSkill;
                }
                while (RequirementNode.Nodes.Count > 0)
                {
                    RequirementNode = RequirementNode.Nodes[0];
                }

                RequirementNode = RequirementNode.Parent;

                BaseSkillRequirement NewSkillRequirement = ((BaseSkillRequirement)cboRequirementType.SelectedItem).Copy();
                pgRequirement.SelectedObject = NewSkillRequirement;

                RootSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[lstRequirements.SelectedIndex] = NewSkillRequirement;
                RequirementNode.Text = RootSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[lstRequirements.SelectedIndex].SkillRequirementName;
            }
        }

        private void lstRequirements_SelectedIndexChanged(object sender, EventArgs e)
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

                BaseSkillRequirement ActiveRequirement = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement[lstRequirements.SelectedIndex];

                AllowEvent = false;
                cboRequirementType.Text = ActiveRequirement.ToString();
                pgRequirement.SelectedObject = ActiveRequirement;
                AllowEvent = true;
            }
        }

        private void btnAddRequirement_Click(object sender, EventArgs e)
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

                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListRequirement.Add(NewSkillRequirement);
                pgRequirement.SelectedObject = NewSkillRequirement;
                lstRequirements.Items.Add(NewSkillRequirement.ToString());
                TreeNode EffectNode = new TreeNode(NewSkillRequirement.ToString());
                List<TreeNode> ListNodeToMove = new List<TreeNode>();

                foreach (TreeNode ActiveNode in SelectedNode.Nodes)
                {
                    ListNodeToMove.Add(ActiveNode);
                }

                SelectedNode.Nodes.Clear();

                foreach (TreeNode ActiveNode in ListNodeToMove)
                {
                    EffectNode.Nodes.Add(ActiveNode);
                }

                SelectedNode.Nodes.Add(EffectNode);
            }
        }

        private void btnRemoveRequirement_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode RootNode = tvSkills.SelectedNode;
            if (lstEffects.SelectedIndex >= 0 && RootNode != null)
            {
                BaseAutomaticSkill RootSkill = RootNode.Tag as BaseAutomaticSkill;
                while (RootSkill == null)
                {
                    RootNode = RootNode.Parent;
                    RootSkill = RootNode.Tag as BaseAutomaticSkill;
                }

                BaseEffect ActiveEffect = RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex];

                AllowEvent = false;
                cboEffectType.Text = ActiveEffect.ToString();
                pgEffect.SelectedObject = ActiveEffect;
                AllowEvent = true;
            }
        }

        private void btnAddEffects_Click(object sender, EventArgs e)
        {
            TreeNode RootNode = tvSkills.SelectedNode;

            if (RootNode != null)
            {
                TreeNode RequirementNode = tvSkills.SelectedNode;
                BaseAutomaticSkill RootSkill = RootNode.Tag as BaseAutomaticSkill;
                while (RootSkill == null)
                {
                    RootNode = RootNode.Parent;
                    RootSkill = RootNode.Tag as BaseAutomaticSkill;
                }
                while (RequirementNode.Nodes.Count > 0)
                {
                    RequirementNode = RequirementNode.Nodes[0];
                }

                RequirementNode = RequirementNode.Parent;

                BaseEffect NewEffect = ((BaseEffect)cboEffectType.Items[0]).Copy();
                RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Add(NewEffect);
                RootSkill.ListSkillLevel[0].ListActivation[0].ListEffectTarget.Add(new List<string>());

                pgRequirement.SelectedObject = NewEffect;
                lstEffects.Items.Add(NewEffect.ToString());

                TreeNode EffectNode = new TreeNode(NewEffect.EffectTypeName);
                RequirementNode.Nodes.Insert(RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Count - 1, EffectNode);
            }
        }

        private void btnRemoveEffect_Click(object sender, EventArgs e)
        {

        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode RootNode = tvSkills.SelectedNode;
            if (RootNode != null && AllowEvent)
            {
                TreeNode RequirementNode = tvSkills.SelectedNode;
                BaseAutomaticSkill RootSkill = RootNode.Tag as BaseAutomaticSkill;
                while (RootSkill == null)
                {
                    RootNode = RootNode.Parent;
                    RootSkill = RootNode.Tag as BaseAutomaticSkill;
                }
                while (RequirementNode.Nodes.Count > 0)
                {
                    RequirementNode = RequirementNode.Nodes[0];
                }

                RequirementNode = RequirementNode.Parent;

                if (lstEffects.SelectedItems.Count > 0)
                {
                    BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
                    pgEffect.SelectedObject = NewSkillEffect;

                    RootSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex] = NewSkillEffect;
                    lstEffects.Items[lstEffects.SelectedIndex] = NewSkillEffect.ToString();
                    RequirementNode.Nodes[lstEffects.SelectedIndex].Text = NewSkillEffect.EffectTypeName;
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
    }
}
