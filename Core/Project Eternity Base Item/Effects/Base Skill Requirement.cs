using Roslyn;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public abstract class BaseSkillRequirement
    {
        public static readonly Dictionary<string, BaseSkillRequirement> DicDefaultRequirement = new Dictionary<string, BaseSkillRequirement>();//When you just need a placeholder outside of a game.

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

        public virtual BaseSkillRequirement CopyAndReload(string ParamsID)
        {
            BaseSkillRequirement NewCopy = Copy();
            NewCopy.CopyMembers(NewCopy);
            NewCopy.DoReload(ParamsID);

            return NewCopy;
        }

        protected virtual void DoReload(string ParamsID)
        {
        }

        public override string ToString()
        {
            return SkillRequirementName;
        }

        public static Dictionary<string, BaseSkillRequirement> LoadAllRequirements()
        {
            HashSet<Assembly> SeenAssemblies = new HashSet<Assembly>();
            Dictionary<string, BaseSkillRequirement> DicRequirement = LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll", SearchOption.AllDirectories), SeenAssemblies, typeof(BaseSkillRequirement));

            ReflectionHelper.ForEachUnseenAssembly(SeenAssemblies, delegate (Assembly ActiveAssembly)
            {
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in LoadFromAssembly(ActiveAssembly, typeof(BaseSkillRequirement)))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            });

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                if (!ReflectionHelper.TrackPluginAssembly(SeenAssemblies, ActiveAssembly))
                {
                    continue;
                }

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
            return LoadFromAssemblyFiles(ArrayFilePath, new HashSet<Assembly>(), TypeOfRequirement, Args);
        }

        public static Dictionary<string, BaseSkillRequirement> LoadFromAssemblyFiles(string[] ArrayFilePath, HashSet<Assembly> SeenAssemblies, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BaseSkillRequirement> DicRequirement = new Dictionary<string, BaseSkillRequirement>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = ReflectionHelper.LoadPluginAssembly(ArrayFilePath[F]);
                if (!ReflectionHelper.TrackPluginAssembly(SeenAssemblies, ActiveAssembly))
                {
                    continue;
                }
                foreach(KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicRequirement;
        }
    }
}
