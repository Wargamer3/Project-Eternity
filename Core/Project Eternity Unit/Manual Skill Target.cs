using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

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
}
