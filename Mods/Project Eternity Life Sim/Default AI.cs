using System;
using System.Reflection;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class DefaultAI
    {
        public static Action<PlayerCharacter> Fill;

        public static void ApplyIfEmpty(PlayerCharacter Character)
        {
            if (Character == null || Character.HasAnyScript())
            {
                return;
            }

            EnsureBound();
            if (Fill != null)
            {
                Fill(Character);
            }
        }

        static void EnsureBound()
        {
            if (Fill != null)
            {
                return;
            }

            Assembly[] Loaded = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < Loaded.Length; i++)
            {
                if (Loaded[i].GetName().Name != "Project Eternity Life Sim Effects")
                {
                    continue;
                }

                Type Found = Loaded[i].GetType("ProjectEternity.GameScreens.LifeSimScreen.DefaultCharacterAI");
                MethodInfo Assign = Found == null ? null : Found.GetMethod("Assign", BindingFlags.Public | BindingFlags.Static);
                if (Assign != null)
                {
                    Fill = (Action<PlayerCharacter>)Delegate.CreateDelegate(typeof(Action<PlayerCharacter>), Assign);
                }
            }
        }
    }
}
