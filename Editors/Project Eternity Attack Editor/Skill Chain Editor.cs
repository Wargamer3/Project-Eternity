using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.AttackSkillChainEditor
{
    public partial class AttackSkillChainEditor : SkillChainEditor.SkillChainEditor
    {
        public AttackSkillChainEditor()
            : base()
        {
            List<string> ListAllowedRequirements = new List<string>() { "SquadPERRequirement", "AttackPERRequirement" };
            List<string> ListAllowedEffect = new List<string>() { "DeathmatchSquadPEREffect", "DeathmatchAttackPEREffect" };

            tvSkills.NodeMouseClick += (sender, args) => tvSkills.SelectedNode = args.Node;
            cboRequirementType.Items.AddRange(BaseSkillRequirement.DicDefaultRequirement.Values.Where(E => ListAllowedRequirements.Contains(E.GetType().BaseType.Name)).ToArray());
            cboEffectType.Items.AddRange(BaseEffect.DicDefaultEffect.Values.Where(E => ListAllowedEffect.Contains(E.GetType().BaseType.Name)).ToArray());
        }

        public AttackSkillChainEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathAttackSkillChains }, "Deathmatch/Attacks/Skill Chains/", new string[] { ".pesc" }, typeof(AttackSkillChainEditor), true, null, true)
            };

            return Info;
        }

        private void LoadSkillChain(string SkillChainPath)
        {
            AllowEvent = false;

            string Name = SkillChainPath.Substring(0, SkillChainPath.Length - 5).Substring(29);
            this.Text = Name + " - Project Eternity Attack Skill Chain Editor";

            FileStream FS = new FileStream("Content/Deathmatch/Attacks/Skill Chains/" + Name + ".pesc", FileMode.Open, FileAccess.Read);
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
