using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ProjectEternity.Core
{
    public static class Operators
    {
        public enum LogicOperators : byte { Equal, NotEqual, Greater, GreaterOrEqual, Lower, LowerOrEqual };

        public enum SignOperators : byte { Equal, PlusEqual, MinusEqual, DividedEqual, MultiplicatedEqual, ModuloEqual };

        public enum NumberTypes : byte { Absolute = 0, Relative = 1 };

        public static bool CompareValue(LogicOperators Operator, int Value1, int Value2)
        {
            switch (Operator)
            {
                case LogicOperators.Equal:
                    if (Value1 == Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.Greater:
                    if (Value1 > Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.GreaterOrEqual:
                    if (Value1 >= Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.Lower:
                    if (Value1 < Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.LowerOrEqual:
                    if (Value1 <= Value2)
                        return true;
                    else
                        return false;

                case LogicOperators.NotEqual:
                    if (Value1 != Value2)
                        return true;
                    else
                        return false;
            }

            return false;
        }
    }

    public static class RandomHelper
    {
        public static Random Random = new Random();

        public static int Next(int MaxValue)
        {
            return Random.Next(MaxValue);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        /// <summary>
        /// Check if you pass a random check.
        /// </summary>
        /// <param name="ChanceToActivate">% of chance to activate</param>
        /// <returns>Returns true if the activation check passed</returns>
        public static bool RandomActivationCheck(int ChanceToActivate)
        {
            if (ChanceToActivate <= 0)
                return false;
            else if (ChanceToActivate >= 100)
                return true;
            else
            {
                float Precision = Random.Next(100) + 1;

                if (Precision <= ChanceToActivate)
                    return true;
            }
            return false;
        }
    }

    public abstract class Holder<T>
    {
        public abstract KeyValuePair<string, List<T>> GetNameAndContent(params object[] args);
    }

    public static class ReflectionHelper
    {
        public static List<T> GetObjectsFromBaseTypes<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract || !ActiveType.IsPublic)
                {
                    continue;
                }

                Type ObjectType = ActiveType.BaseType;
                bool InstanceIsBaseObject = false;
                while (ObjectType != null && !InstanceIsBaseObject)
                {
                    if (ObjectType == TypeOfT)
                        InstanceIsBaseObject = true;

                    ObjectType = ObjectType.BaseType;
                }

                if (InstanceIsBaseObject)
                {
                    T NewObject = (T)Activator.CreateInstance(ActiveType, Args);
                    ListObject.Add(NewObject);
                }
            }
            return ListObject;
        }
        public static List<T> GetObjectsFromBaseInterface<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract)
                {
                    continue;
                }

                foreach (Type ActiveInterface in ActiveType.GetInterfaces())
                {
                    Type ObjectType = ActiveInterface;
                    bool InstanceIsBaseObject = false;
                    while (ObjectType != null && !InstanceIsBaseObject)
                    {
                        if (ObjectType == TypeOfT)
                            InstanceIsBaseObject = true;

                        ObjectType = ObjectType.BaseType;
                    }

                    if (InstanceIsBaseObject)
                    {
                        T NewObject = (T)Activator.CreateInstance(ActiveType, Args);
                        ListObject.Add(NewObject);
                    }
                }
            }
            return ListObject;
        }

        public static List<T> GetObjectsFromTypes<T>(Type TypeOfT, Type[] ArrayTypes, params object[] Args)
        {
            List<T> ListObject = new List<T>();
            foreach (Type ActiveType in ArrayTypes)
            {
                if (ActiveType.IsAbstract)
                {
                    continue;
                }

                Type ObjectType = ActiveType.BaseType;

                if (ObjectType == TypeOfT)
                {
                    T NewObject = (T)Activator.CreateInstance(ActiveType, Args);
                    ListObject.Add(NewObject);
                }
            }
            return ListObject;
        }

        public static List<T> GetNestedTypes<T>(Type HolderType, params object[] Args)
        {
            return GetObjectsFromBaseTypes<T>(typeof(T), HolderType.GetNestedTypes(), Args);
        }

        public static Dictionary<string, List<T>> GetContentByName<T>(string[] ArrayFilePath, params object[] Args)
        {
            return GetContentByName<T>(typeof(Holder<T>), ArrayFilePath, Args);
        }

        public static Dictionary<string, List<T>> GetContentByName<T>(Type TypeOfHolder, string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, List<T>> DicGetContentByName = new Dictionary<string, List<T>>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                List<Holder<T>> ListHolder = GetObjectsFromBaseTypes<Holder<T>>(TypeOfHolder, ActiveAssembly.GetTypes());

                foreach (var NewHolder in ListHolder)
                {
                    KeyValuePair<string, List<T>> ContentByName = NewHolder.GetNameAndContent(Args);

                    if (!DicGetContentByName.ContainsKey(ContentByName.Key))
                    {
                        DicGetContentByName.Add(ContentByName.Key, ContentByName.Value);
                    }
                    else
                    {
                        DicGetContentByName[ContentByName.Key].AddRange(ContentByName.Value);
                    }
                }
            }
            return DicGetContentByName;
        }
    }
}
