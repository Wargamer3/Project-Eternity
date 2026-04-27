using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Unlockable
    {
        public UnlcokableItemType ItemToUnlock;
        public List<UnlockRequirementEvaluator> ListUnlockRequirement;

        public Unlockable()
        {
            ListUnlockRequirement = new List<UnlockRequirementEvaluator>();
        }

        public Unlockable(BinaryReader BR)
        {
            string UnlockableType = BR.ReadString();
            ItemToUnlock = LifeSimParams.DicUnlockableItemTypeByName[UnlockableType].LoadCopy(BR);

            byte ListUnlockRequirementCount = BR.ReadByte();
            ListUnlockRequirement = new List<UnlockRequirementEvaluator>();
            for (int R = 0; R < ListUnlockRequirementCount; ++R)
            {
                string RequirementName = BR.ReadString();

                UnlockRequirementEvaluator NewActionUnlockRequirement = LifeSimParams.DicRequirementByName[RequirementName].LoadCopy(BR);

                ListUnlockRequirement.Add(NewActionUnlockRequirement);
            }
        }

        public void Write(BinaryWriter BW)
        {
            ItemToUnlock.Write(BW);
            BW.Write((byte)ListUnlockRequirement.Count);

            foreach (UnlockRequirementEvaluator ActiveRequirement in ListUnlockRequirement)
            {
                ActiveRequirement.Write(BW);
            }
        }

        public void CheckUnlocks()
        {
            bool CanUnlock = true;

            foreach (UnlockRequirementEvaluator ActiveEvaluator in ListUnlockRequirement)
            {
                if (!ActiveEvaluator.CanBeUnlocked())
                {
                    CanUnlock = false;
                }
            }

            if (CanUnlock)
            {
                ItemToUnlock.Unlock();
            }
        }

        internal void Init(LifeSimParams Params, object Parent)
        {
            ItemToUnlock.Params = Params;
            ItemToUnlock.Parent = Parent;
            foreach (UnlockRequirementEvaluator ActiveRequirement in ListUnlockRequirement)
            {
                ActiveRequirement.Init(Params);
            }
        }

        public override string ToString()
        {
            return ItemToUnlock.UnlcokableTypeName;
        }
    }

    public abstract class UnlcokableItemType
    {
        public string UnlcokableTypeName;
        public LifeSimParams Params;
        public object Parent;

        protected UnlcokableItemType(string UnlcokableTypeName)
        {
            this.UnlcokableTypeName = UnlcokableTypeName;
        }

        public abstract void Unlock();

        public abstract UnlcokableItemType Copy();

        public abstract UnlcokableItemType LoadCopy(BinaryReader BR);

        public abstract void DoWrite(BinaryWriter BW);

        public void Write(BinaryWriter BW)
        {
            BW.Write(UnlcokableTypeName);

            DoWrite(BW);
        }

        public void Init(LifeSimParams Params)
        {
            this.Params = Params;
        }

        public override string ToString()
        {
            return UnlcokableTypeName;
        }

        public static Dictionary<string, UnlcokableItemType> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, UnlcokableItemType> DicEffect = new Dictionary<string, UnlcokableItemType>();

            List<UnlcokableItemType> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<UnlcokableItemType>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (UnlcokableItemType Instance in ListSkillEffect)
            {
                DicEffect.Add(Instance.UnlcokableTypeName, Instance);
            }

            return DicEffect;
        }

        public static Dictionary<string, UnlcokableItemType> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, UnlcokableItemType> DicEffect = new Dictionary<string, UnlcokableItemType>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, UnlcokableItemType> ActiveEffect in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicEffect;
        }
    }
}
