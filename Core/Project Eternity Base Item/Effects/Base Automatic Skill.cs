using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class BaseAutomaticSkill
    {
        public string Name;
        public string RelativePath;
        public string Description;
        public int CurrentLevel;
        public List<BaseSkillLevel> ListSkillLevel;

        public BaseSkillLevel CurrentSkillLevel { get { return ListSkillLevel[CurrentLevel - 1]; } }

        public BaseAutomaticSkill()
        {
            Description = string.Empty;
            ListSkillLevel = new List<BaseSkillLevel>();
        }

        public BaseAutomaticSkill(BaseAutomaticSkill Clone)
        {
            Name = Clone.Name;
            RelativePath = Clone.RelativePath;
            Description = Clone.Description;
            CurrentLevel = Clone.CurrentLevel;

            ListSkillLevel = new List<BaseSkillLevel>(Clone.ListSkillLevel.Count);
            foreach (BaseSkillLevel ActiveSkillLevel in Clone.ListSkillLevel)
            {
                ListSkillLevel.Add(new BaseSkillLevel(ActiveSkillLevel));
            }
        }

        public BaseAutomaticSkill(string FullPath, string RelativePath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this()
        {
            this.RelativePath = RelativePath;
            Name = Path.GetFileNameWithoutExtension(FullPath);
            CurrentLevel = 1;

            FileStream FS = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Load(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);

            FS.Close();
            BR.Close();
        }

        public static BaseAutomaticSkill CreateDummy(string Name)
        {
            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill();
            NewSkill.Name = Name;
            NewSkill.RelativePath = Name;
            NewSkill.ListSkillLevel.Add(new BaseSkillLevel());
            NewSkill.CurrentLevel = 1;
            BaseSkillActivation NewActivation = new BaseSkillActivation();
            NewSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            return NewSkill;
        }

        public BaseAutomaticSkill(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Name = "N/A";
            RelativePath = "N/A";
            CurrentLevel = 1;

            Load(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        private void Load(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Description = BR.ReadString();

            int ListSkillLevelCount = BR.ReadInt32();
            ListSkillLevel = new List<BaseSkillLevel>(ListSkillLevelCount);

            for (int L = 0; L < ListSkillLevelCount; L++)
            {
                ListSkillLevel.Add(new BaseSkillLevel(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Description);

            BW.Write(ListSkillLevel.Count);
            for (int L = 0; L < ListSkillLevel.Count; L++)
            {
                ListSkillLevel[L].Save(BW);
            }
        }

        public void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            foreach (BaseSkillLevel ActiveSkillLevel in ListSkillLevel)
            {
                foreach (BaseSkillActivation ActiveActivation in ActiveSkillLevel.ListActivation)
                {
                    for (int R = 0; R < ActiveActivation.ListRequirement.Count; R++)
                    {
                        BaseSkillRequirement NewRequirement = DicRequirement[ActiveActivation.ListRequirement[R].SkillRequirementName].Copy();
                        NewRequirement.CopyMembers(ActiveActivation.ListRequirement[R]);
                        ActiveActivation.ListRequirement[R] = NewRequirement;
                    }

                    ActiveActivation.ListEffectTargetReal.Clear();
                    foreach (List<string> ListActiveTarget in ActiveActivation.ListEffectTarget)
                    {
                        List<AutomaticSkillTargetType> NewListEffectTargetReal = new List<AutomaticSkillTargetType>(ListActiveTarget.Count);
                        ActiveActivation.ListEffectTargetReal.Add(NewListEffectTargetReal);
                        foreach (string ActiveTarget in ListActiveTarget)
                        {
                            NewListEffectTargetReal.Add(DicAutomaticSkillTarget[ActiveTarget].Copy());
                        }
                    }

                    for (int E = 0; E < ActiveActivation.ListEffect.Count; E++)
                    {
                        BaseEffect NewEffect = DicEffect[ActiveActivation.ListEffect[E].EffectTypeName].Copy();
                        NewEffect.CopyMembers(ActiveActivation.ListEffect[E]);
                        ActiveActivation.ListEffect[E] = NewEffect;

                        foreach (BaseAutomaticSkill ActiveFollowingSkill in ActiveActivation.ListEffect[E].ListFollowingSkill)
                        {
                            ActiveFollowingSkill.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                        }
                    }
                }
            }
        }

        public bool CanAddSkillEffectsToTarget(string SkillRequirementToActivate)
        {
            if (CurrentSkillLevel.ActivationsCount == 0)
            {
                return false;
            }

            for (int A = 0; A < CurrentSkillLevel.ListActivation.Count; A++)
            {
                if (CurrentSkillLevel.ListActivation[A].CanActivate(SkillRequirementToActivate, Name))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddSkillEffectsToTarget(string SkillRequirementToActivate)
        {
            //No activations remaining.
            if (CurrentSkillLevel.ActivationsCount == 0)
                return;

            BaseSkillActivation SkillActivation = null;
            for (int A = 0; A < CurrentSkillLevel.ListActivation.Count; A++)
            {
                bool HasActivated = CurrentSkillLevel.ListActivation[A].CanActivate(SkillRequirementToActivate, Name);

                if (HasActivated)
                {
                    if (CurrentSkillLevel.ListActivation[A].Weight == -1)
                    {
                        CurrentSkillLevel.ListActivation[A].Activate(Name);
                    }
                    else if (SkillActivation == null || CurrentSkillLevel.ListActivation[A].Weight > SkillActivation.Weight)
                    {
                        SkillActivation = CurrentSkillLevel.ListActivation[A];
                    }

                    if (CurrentSkillLevel.ActivationsCount > 0)
                        CurrentSkillLevel.ActivationsCount--;
                }
            }

            if (SkillActivation != null)
            {
                SkillActivation.Activate(Name);
            }
        }
    }
}
