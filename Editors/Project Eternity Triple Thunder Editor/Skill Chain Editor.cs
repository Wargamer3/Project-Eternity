using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.Editors.TripleThunderSkillChainEditor
{
    public partial class TripleThunderSkillChainEditor : SkillChainEditor.SkillChainEditor
    {
        public TripleThunderSkillChainEditor()
            : base()
        {
            tvSkills.NodeMouseClick += (sender, args) => tvSkills.SelectedNode = args.Node;
            cboEffectType.Items.AddRange(BaseEffect.DicDefaultEffect.Values.ToArray());
            cboRequirementType.Items.AddRange(BaseSkillRequirement.DicDefaultRequirement.Values.Where(R => R is TripleThunderRobotRequirement || R is TripleThunderAttackRequirement).ToArray());

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in Projectile2D.GetCoreProjectileEffects(null))
            {
                cboEffectType.Items.Add(ActiveEffect.Value);
            }
        }

        public TripleThunderSkillChainEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathTripleThunderSkillChains }, "Triple Thunder/Skill Chains/", new string[] { ".pesc" }, typeof(TripleThunderSkillChainEditor), true, null, true)
            };

            return Info;
        }

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
                BaseAutomaticSkill ActiveSkill = new BaseAutomaticSkill(BR, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
                CreateTree(ActiveSkill, tvSkills.Nodes);
            }

            BR.Close();
            FS.Close();

            AllowEvent = true;
        }
    }
}
