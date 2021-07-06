using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Skill
{
    public abstract class ManualSkillTarget
    {
        public readonly string TargetType;
        public readonly bool MustBeUsedAlone;

        protected ManualSkillTarget(string TargetType, bool MustBeUsedAlone)
        {
            this.TargetType = TargetType;
            this.MustBeUsedAlone = MustBeUsedAlone;
        }

        public abstract ManualSkillTarget Copy();

        public abstract void ActivateSkillFromMenu(ManualSkill ActiveSkill);

        public abstract bool CanActivateOnTarget(ManualSkill ActiveSkill);

        public void AddAndExecuteEffect(ManualSkill ActiveSkill, EffectHolder Effects)
        {
            for (int E = ActiveSkill.ListEffect.Count - 1; E >= 0; --E)
            {
                //Init Skill.
                Effects.AddAndExecuteEffect(ActiveSkill.ListEffect[E], ActiveSkill.Name);
            }
        }

        public static ManualSkillTarget LoadCopy(BinaryReader BR, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string TargetType = BR.ReadString();

            ManualSkillTarget NewManualSkillTarget = DicManualSkillTarget[TargetType].Copy();

            return NewManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadAllTargetTypes()
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories));

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssembly(Assembly ActiveAssembly, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();

            List<ManualSkillTarget> ListActivation = ReflectionHelper.GetObjectsFromBaseTypes<ManualSkillTarget>(typeof(ManualSkillTarget), ActiveAssembly.GetTypes(), Args);

            foreach (ManualSkillTarget Instance in ListActivation)
            {
                DicManualSkillTarget.Add(Instance.TargetType, Instance);
            }

            return DicManualSkillTarget;
        }

        public static Dictionary<string, ManualSkillTarget> LoadFromAssemblyFiles(string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveRequirement in LoadFromAssembly(ActiveAssembly, Args))
                {
                    DicManualSkillTarget.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicManualSkillTarget;
        }
    }

    public class ManualSkill
    {
        public int Range;// Limit the area of effect of the Skill, or -1 for infinite.
        public int SPCost;
        public int LevelRequirement;
        public readonly string Name;
        public readonly string FullName;
        public ManualSkillTarget Target;//Tells who to use the Skill on.
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

            FullName = SkillPath.Substring(0, SkillPath.Length - 5).Substring(27);
            Name = Path.GetFileNameWithoutExtension(SkillPath);

            FileStream FS = new FileStream(SkillPath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Range = BR.ReadInt32();
            Target = ManualSkillTarget.LoadCopy(BR, DicManualSkillTarget);

            Description = BR.ReadString();

            int ListEffectCount = BR.ReadInt32();
            ListEffect = new List<BaseEffect>(ListEffectCount);
            for (int i = 0; i < ListEffectCount; i++)
                ListEffect.Add(BaseEffect.FromFile(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));

            FS.Close();
            BR.Close();
        }
        
        public void ActiveSkillFromMenu(Character ActiveCharacter, Squad ActiveSquad)
        {
            Target.ActivateSkillFromMenu(this);
        }

        public void UpdateSkillActivation()
        {
            _CanActivate = Target.CanActivateOnTarget(this);
        }

        public void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Target = DicManualSkillTarget[Target.TargetType].Copy();

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
