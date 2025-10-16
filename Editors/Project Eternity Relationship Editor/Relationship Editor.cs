using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.Editors.RelationshipEditor
{
    public partial class ProjectEternityRelationshipEditor : BaseEditor
    {
        private BaseAutomaticSkill ActiveSkill;
        private bool AllowEvents;

        public ProjectEternityRelationshipEditor()
        {
            InitializeComponent();

            AllowEvents = true;
            cboEffectType.Items.AddRange(BaseEffect.DicDefaultEffect.Values.OrderBy(x => x.EffectTypeName).ToArray());
        }

        public ProjectEternityRelationshipEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathCharacterRelationships }, "Deathmatch/Characters/Relationships/", new string[] { ".pecr" }, typeof(ProjectEternityRelationshipEditor), true, null, false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            ActiveSkill.Description = txtDescription.Text;

            ActiveSkill.Save(BW);

            FS.Close();
            BW.Close();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void LoadSkill(string SkillPath)
        {
            string Name = FilePath.Substring(0, FilePath.Length - 5).Substring(26);
            this.Text = Name + " - Project Eternity Relationship Editor";

            ActiveSkill = new BaseAutomaticSkill(SkillPath, Name, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

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

        private void btnAddLevel_Click(object sender, EventArgs e)
        {
            BaseSkillLevel NewSkillLevel = new BaseSkillLevel();
            BaseSkillActivation NewSkillActivation = new BaseSkillActivation();
            NewSkillLevel.ListActivation.Add(NewSkillActivation);
            
            ActiveSkill.ListSkillLevel.Add(NewSkillLevel);
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
        
        private void btnAddEffects_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0];
                BaseEffect NewEffect = BaseEffect.DicDefaultEffect.First().Value;
                NewEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypePermanent;

                ActiveSkillActivation.ListEffect.Add(NewEffect);
                ActiveSkillActivation.ListEffectTarget.Add(new List<string>());
                lstEffects.Items.Add("Auto Dodge effect");
                lstEffects.SelectedIndex = lstEffects.Items.Count - 1;
            }
        }

        private void btnRemoveEffect_Click(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0];

                ActiveSkillActivation.ListEffect.RemoveAt(lstEffects.SelectedIndex);
                ActiveSkillActivation.ListEffectTarget.RemoveAt(lstEffects.SelectedIndex);
                lstEffects.Items.RemoveAt(lstEffects.SelectedIndex);
            }
        }

        private void lstLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLevels.SelectedItems.Count > 0)
            {
                gbEffects.Enabled = true;
                lstEffects.Items.Clear();

                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0];
                for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; E++)
                {
                    lstEffects.Items.Add(ActiveSkillActivation.ListEffect[E].ToString());
                }
                if (lstEffects.Items.Count > 0)
                    lstEffects.SelectedIndex = 0;
            }
            else
            {
                gbEffects.Enabled = false;
            }
        }

        private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;

            if (lstEffects.SelectedItems.Count > 0)
            {
                BaseSkillActivation ActiveSkillActivation = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0];
                BaseEffect ActiveEffect = ActiveSkillActivation.ListEffect[lstEffects.SelectedIndex];
                List<string> ListEffectActivation = ActiveSkillActivation.ListEffectTarget[lstEffects.SelectedIndex];
                pgEffect.SelectedObject = ActiveEffect;
                cboEffectType.Text = pgEffect.SelectedObject.ToString();
            }

            AllowEvents = true;
        }

        private void cboEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lstLevels.SelectedItems.Count > 0 && lstEffects.SelectedItems.Count > 0)
            {
                BaseEffect NewSkillEffect = ((BaseEffect)cboEffectType.SelectedItem).Copy();
                BaseEffect OldSkillEffect = ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0].ListEffect[lstEffects.SelectedIndex];

                NewSkillEffect.CopyMembers(OldSkillEffect);

                ActiveSkill.ListSkillLevel[lstLevels.SelectedIndex].ListActivation[0].ListEffect[lstEffects.SelectedIndex] = NewSkillEffect;
                lstEffects.Items[lstEffects.SelectedIndex] = NewSkillEffect.ToString();
            }
        }
    }
}
