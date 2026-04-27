using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AICharacterAction
    {
        public string Name;
        public string Description;
        public List<AIAction> ListAIAction;

        public AICharacterAction(string FilePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("Content/Life Sim/AI Actions/" + FilePath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListAIActionCount = BR.ReadByte();
            for (int A = 0; A < ListAIActionCount; ++A)
            {
                string AIActionPath = BR.ReadString();

                if (!string.IsNullOrEmpty(AIActionPath))
                {
                    ListAIAction.Add(AIAction.DicDefaultAction[AIActionPath].Copy());
                }
            }

            BR.Close();
            FS.Close();
        }
    }

    public abstract class AIAction
    {
        public static readonly Dictionary<string, AIAction> DicDefaultAction = new Dictionary<string, AIAction>();//When you just need a placeholder outside of a game.

        public readonly string AIGoal;
        public int ActionCost;
        public int Urgency;
        public int TimeRequired;

        public int AIWeight;

        public string AILogicPath;

        public abstract ActionPanelLifeSim GetActionPanel();

        public AIAction(string AIGoal)
        {
            this.AIGoal = AIGoal;
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
        public abstract List<AIAction> GetAIExecutionPlan(NavMapGameManager Map);

        /// <summary>
        /// //Called when something happen, could be a certain time passed or an interaction.
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="Map"></param>
        public abstract void UpdatePrecondition(string Event, NavMapGameManager Map);
    }

    public abstract class LifeSimAIAction : AIAction
    {
        public PlayerCharacter Owner;

        protected LifeSimAIAction(string AIGoal, PlayerCharacter Owner)
            : base(AIGoal)
        {
            this.Owner = Owner;
        }
    }
}
