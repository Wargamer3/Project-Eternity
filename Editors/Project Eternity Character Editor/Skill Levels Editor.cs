using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class SkillLevelsEditor : Form
    {
        public SkillLevelsEditor(string SkillName)
        {
            InitializeComponent();

            lblName.Text = SkillName;
            dgvLevels.Rows.Clear();

            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill("Content/Characters/Skills/" + SkillName + ".pecs", SkillName, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            for(int L = 0; L < NewSkill.ListSkillLevel.Count; ++L)
            {
                dgvLevels.Rows.Add(L + 1, L + 1);
            }
        }

        public SkillLevelsEditor(string SkillName, Core.Characters.Character.SkillLevels SelectedSkillLevels)
        {
            InitializeComponent();
            lblName.Text = SkillName;
            dgvLevels.Rows.Clear();

            foreach (KeyValuePair<int, int> SkillLevelPerCharacterLevel in SelectedSkillLevels.DicSkillLevelPerCharacterLevel)
            {
                dgvLevels.Rows.Add(SkillLevelPerCharacterLevel.Key, SkillLevelPerCharacterLevel.Value);
            }
        }

        public void Save(BinaryWriter BW)
        {
            Dictionary<int, int> DicSkillLevelPerCharacterLevel = new Dictionary<int, int>();

            for (int R = 0; R < dgvLevels.Rows.Count - 1; ++R)
            {
                if (dgvLevels.Rows[R].Cells[0].Value == null || dgvLevels.Rows[R].Cells[1].Value == null)
                {
                    continue;
                }

                DicSkillLevelPerCharacterLevel.Add(Convert.ToInt32(dgvLevels.Rows[R].Cells[0].Value), Convert.ToInt32(dgvLevels.Rows[R].Cells[1].Value));
            }

            BW.Write(DicSkillLevelPerCharacterLevel.Count);
            foreach (KeyValuePair<int, int> SkillLevel in DicSkillLevelPerCharacterLevel)
            {
                BW.Write(SkillLevel.Key);
                BW.Write(SkillLevel.Value);
            }
        }
    }
}
