using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.SkillChainEditor
{
    public partial class SkillChainEditor : BaseEditor
    {
        private bool AllowEvent;
        Dictionary<string, BaseSkillRequirement> DicRequirement;
        Dictionary<string, BaseEffect> DicEffect;

        public SkillChainEditor()
        {
            InitializeComponent();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in Projectile.GetCoreProjectileEffects(null))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            tvSkills.NodeMouseClick += (sender, args) => tvSkills.SelectedNode = args.Node;
            cboEffectType.Items.AddRange(DicEffect.Values.ToArray());
            cboRequirementType.Items.AddRange(DicRequirement.Values.ToArray());
        }

        public SkillChainEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                SaveItem(FilePath, "New Item");
            }

            LoadSkillChain(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathTripleThunderSkillChains }, "Triple Thunder/Skill Chains/", new string[] { ".pesc" }, typeof(SkillChainEditor), true, null, true)
            };

            return Info;
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

        /// <summary>
        /// Load a SkillChain at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Combo.</param>
        private void LoadSkillChain(string SkillChainPath)
        {
            AllowEvent = false;

            string Name = SkillChainPath.Substring(0, SkillChainPath.Length - 5).Substring(36);
            this.Text = Name + " - Project Eternity Triple Thunder Skill Chain Editor";

            FileStream FS = new FileStream("Content/Triple Thunder/Skill Chains/" + Name + ".pesc", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int tvSkillsNodesCount = BR.ReadInt32();
            for (int N = 0; N < tvSkillsNodesCount; ++N)
            {
                BaseAutomaticSkill ActiveSkill = new BaseAutomaticSkill(BR, DicRequirement, DicEffect);
                CreateTree(ActiveSkill, tvSkills.Nodes);
            }

            BR.Close();
            FS.Close();

            AllowEvent = true;
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

        private void AddSkillNode()
        {
            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill();
            NewSkill.Name = "Dummy Skill";
            NewSkill.ListSkillLevel.Add(new BaseSkillLevel());
            NewSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            NewSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(DicRequirement.First().Value.Copy());
            NewActivation.ListEffectTarget.Add(new List<string>());
            NewActivation.ListEffect.Add(DicEffect.First().Value.Copy());

            TreeNode SelectedNode = tvSkills.SelectedNode;
            if (SelectedNode != null)
            {
                BaseAutomaticSkill ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                while (ActiveSkill == null)
                {
                    SelectedNode = SelectedNode.Parent;
                    ActiveSkill = SelectedNode.Tag as BaseAutomaticSkill;
                }

                BaseEffect ActiveEffect = ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect[lstEffects.SelectedIndex];
                ActiveEffect.ListFollowingSkill.Add(NewSkill);
            }

            if (tvSkills.SelectedNode != null)
            {
                CreateTree(NewSkill, tvSkills.SelectedNode.Nodes);
            }
            else
            {
                CreateTree(NewSkill, tvSkills.Nodes);
            }
        }
        
        private void CreateTree(BaseAutomaticSkill ActiveSkill, TreeNodeCollection Nodes)
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
                    CreateTree(ActiveFollowingSkill, NewNode.Nodes);
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

                BaseEffect NewEffect = DicEffect.First().Value.Copy();
                BaseAutomaticSkill ActiveSkill = (BaseAutomaticSkill)ParentNode.Tag;
                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Add(NewEffect);
                ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffectTarget.Add(new List<string>());

                TreeNode EffectNode = new TreeNode(NewEffect.EffectTypeName);
                ParentNode.Nodes.Insert(ActiveSkill.ListSkillLevel[0].ListActivation[0].ListEffect.Count - 1, EffectNode);
            }
        }

        private void tsmNewRequirement_Click(object sender, EventArgs e)
        {
            AddSkillNode();
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
