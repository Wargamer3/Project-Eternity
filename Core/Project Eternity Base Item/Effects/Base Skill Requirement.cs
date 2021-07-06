using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public abstract class BaseSkillRequirement
    {
        public static string OnCreatedRequirementName = "On Created";
        public static string AfterMovingRequirementName = "After Moving";

        public readonly string SkillRequirementName;

        public BaseSkillRequirement(string SkillRequirementName)
        {
            this.SkillRequirementName = SkillRequirementName;
        }

        protected abstract void Load(BinaryReader BR);

        public void Save(BinaryWriter BW)
        {
            BW.Write(SkillRequirementName);
            DoSave(BW);
        }

        public static BaseSkillRequirement LoadCopy(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement)
        {
            string RequirementType = BR.ReadString();
            BaseSkillRequirement NewSkillRequirement = DicRequirement[RequirementType].Copy();
            NewSkillRequirement.Load(BR);

            return NewSkillRequirement;
        }

        protected abstract void DoSave(BinaryWriter BW);

        public virtual bool CanActicateManually(string ManualActivationName)
        {
            return SkillRequirementName == ManualActivationName;
        }

        public abstract bool CanActivatePassive();

        public abstract BaseSkillRequirement Copy();

        public abstract void CopyMembers(BaseSkillRequirement Copy);

        public override string ToString()
        {
            return SkillRequirementName;
        }

        public static Dictionary<string, BaseSkillRequirement> LoadAllRequirements()
        {
            Dictionary<string, BaseSkillRequirement> DicRequirement = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), typeof(BaseSkillRequirement));

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in LoadFromAssembly(ActiveAssembly, typeof(BaseSkillRequirement)))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicRequirement;
        }

        public static Dictionary<string, BaseSkillRequirement> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BaseSkillRequirement> DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            List<BaseSkillRequirement> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<BaseSkillRequirement>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (BaseSkillRequirement Instance in ListSkillEffect)
            {
                DicRequirement.Add(Instance.SkillRequirementName, Instance);
            }

            return DicRequirement;
        }

        public static Dictionary<string, BaseSkillRequirement> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BaseSkillRequirement> DicRequirement = new Dictionary<string, BaseSkillRequirement>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach(KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicRequirement;
        }
    }
}
