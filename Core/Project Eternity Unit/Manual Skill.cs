using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Skill
{
    public class ManualSkill
    {
        public int Range;// Limit the area of effect of the Skill, or -1 for infinite.
        public int ActivationCost;
        public int LevelRequirement;
        public readonly string Name;
        public readonly string FullName;
        public ManualSkillTarget Target;//Tells who to use the Skill on.
        public List<BaseSkillRequirement> ListRequirement;//List of every requirement criterias needed to active the Skill.
        public readonly string Description;
        private bool _CanActivate;
        public bool CanActivate { get { return _CanActivate; } }
        public bool IsUnlocked;

        public List<BaseEffect> ListEffect;//List of Effect to execute once activated.

        public ManualSkill()
        {

        }

        public ManualSkill(string SkillPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            _CanActivate = false;
            IsUnlocked = false;

            FullName = SkillPath.Substring(0, SkillPath.Length - 4).Substring(8);
            Name = Path.GetFileNameWithoutExtension(SkillPath);

            FileStream FS = new FileStream(SkillPath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Range = BR.ReadInt32();
            Target = ManualSkillTarget.LoadCopy(BR, DicManualSkillTarget);

            Description = BR.ReadString();

            int ListRequirementCount = BR.ReadInt32();
            ListRequirement = new List<BaseSkillRequirement>(ListRequirementCount);
            for (int R = 0; R < ListRequirementCount; R++)
            {
                ListRequirement.Add(BaseSkillRequirement.LoadCopy(BR, DicRequirement));
            }

            int ListEffectCount = BR.ReadInt32();
            ListEffect = new List<BaseEffect>(ListEffectCount);
            for (int E = 0; E < ListEffectCount; E++)
            {
                ListEffect.Add(BaseEffect.FromFile(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }

            FS.Close();
            BR.Close();
        }

        public ManualSkill(ManualSkill Clone)
        {
            _CanActivate = false;
            IsUnlocked = false;

            FullName = Clone.FullName;
            Name = Clone.Name;

            Range = Clone.Range;
            Target = Clone.Target;

            Description = Clone.Description;

            ListRequirement = new List<BaseSkillRequirement>(Clone.ListRequirement.Count);
            for (int R = 0; R < Clone.ListRequirement.Count; R++)
            {
                ListRequirement.Add(Clone.ListRequirement[R].Copy());
            }

            ListEffect = new List<BaseEffect>(Clone.ListEffect.Count);
            for (int E = 0; E < Clone.ListEffect.Count; E++)
            {
                ListEffect.Add(Clone.ListEffect[E].Copy());
            }
        }

        public void ActiveSkillFromMenu()
        {
            Target.ActivateSkillFromMenu(this);
        }

        public void UpdateSkillActivation()
        {
            _CanActivate = Target.CanActivateOnTarget(this);
        }

        /// <summary>
        /// Reload skill from dictionnary to update their Params
        /// </summary>
        /// <param name="DicRequirement"></param>
        /// <param name="DicEffect"></param>
        /// <param name="DicAutomaticSkillTarget"></param>
        /// <param name="DicManualSkillTarget"></param>
        public void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            ManualSkillTarget NewTarget = DicManualSkillTarget[Target.TargetType].Copy();
            NewTarget.CopyMembers(Target);

            Target = NewTarget;

            for (int R = 0; R < ListRequirement.Count; R++)
            {
                BaseSkillRequirement NewRequirement = DicRequirement[ListRequirement[R].SkillRequirementName].Copy();
                NewRequirement.CopyMembers(ListRequirement[R]);
                ListRequirement[R] = NewRequirement;
            }

            for (int E = 0; E < ListEffect.Count; E++)
            {
                BaseEffect NewEffect = DicEffect[ListEffect[E].EffectTypeName].Copy();
                NewEffect.CopyMembers(ListEffect[E]);
                ListEffect[E] = NewEffect;

                foreach (BaseAutomaticSkill ActiveFollowingSkill in ListEffect[E].ListFollowingSkill)
                {
                    ActiveFollowingSkill.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                }
            }
        }

        public bool CanActivateEffectsOnTarget(EffectHolder Effects)
        {
            //Make sure the PilotSkill can be used on this Character.
            for (int E = ListEffect.Count - 1; E >= 0; --E)
            {
                if (Effects.CanAddEffect(ListEffect[E], Name) && ListEffect[E].CanActivate())
                    return true;
            }
            
            return false;
        }
    }
}
