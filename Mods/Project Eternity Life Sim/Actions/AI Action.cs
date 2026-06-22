using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AICharacterAction
    {
        public string Name;
        public string Description;
        public List<LifeSimAIAction> ListAIAction;//Is a list in case you want to regroup multiple AI action in the same file

        public AICharacterAction(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/AI Actions/" + FilePath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListAIActionCount = BR.ReadByte();
            ListAIAction = new List<LifeSimAIAction>(ListAIActionCount);
            for (int A = 0; A < ListAIActionCount; ++A)
            {
                string AIActionPath = BR.ReadString();

                if (!string.IsNullOrEmpty(AIActionPath))
                {
                    ListAIAction.Add((LifeSimAIAction)LifeSimCharacterParams.DicAIAction[AIActionPath].Copy());
                }
            }

            BR.Close();
            FS.Close();
        }
    }

    public abstract class AIAction
    {
        public readonly string AIGoal;
        public readonly string AIActionName;
        public int ActionCost;
        public int Urgency;
        public int TimeRequired;

        public int AIWeight;

        public string AILogicPath;

        public abstract ActionPanelLifeSimPlayer GetActionPanel();

        public AIAction(string AIGoal, string AIActionName)
        {
            this.AIGoal = AIGoal;
            this.AIActionName = AIActionName;
        }

        public virtual AIAction Copy()
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Map"></param>
        /// <returns>Has Ended</returns>
        public abstract bool Execute(GameTime gameTime, NavMapGameManager Map);

        public AIAction ForceEnd()
        {
            return null;
        }

        /// <summary>
        /// Called when action finished executing. If an action is complex, return other actions that need to be executed first.
        /// </summary>
        /// <param name="Map"></param>
        /// <returns></returns>
        public abstract List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map);

        /// <summary>
        /// //Called when something happen, could be a certain time passed or an interaction.
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="Map"></param>
        public abstract void UpdatePrecondition(string Event, NavMapGameManager Map);


        public static Dictionary<string, AIAction> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, AIAction> DicAIAction = new Dictionary<string, AIAction>();

            List<AIAction> ListAIAction = ReflectionHelper.GetObjectsFromBaseTypes<AIAction>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (AIAction ActiveAIAction in ListAIAction)
            {
                DicAIAction.Add(ActiveAIAction.AIActionName, ActiveAIAction);
            }

            return DicAIAction;
        }

        public static Dictionary<string, AIAction> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, AIAction> DicAIAction = new Dictionary<string, AIAction>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, AIAction> ActiveAIAction in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicAIAction.Add(ActiveAIAction.Key, ActiveAIAction.Value);
                }
            }

            return DicAIAction;
        }

        public override string ToString()
        {
            return AIActionName;
        }
    }

    public abstract class LifeSimAIAction : AIAction
    {
        protected LifeSimCharacterParams Params;

        protected LifeSimAIAction(string AIGoal, string AIActionName)
            : base(AIGoal, AIActionName)
        {
        }

        public void Init(LifeSimCharacterParams Params)
        {
            this.Params = Params;
        }
    }
}
