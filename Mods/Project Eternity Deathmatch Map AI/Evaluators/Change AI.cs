using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class ChangeAI : DeathmatchScript, ScriptEvaluator
        {
            private string _AIPath;

            public ChangeAI()
                : base(150, 50, "Change AI Deathmatch", new string[0], new string[1] { "Unit" })
            {
                _AIPath = "";
            }
            
            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AIPath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_AIPath);
            }

            public override AIScript CopyScript()
            {
                return new ChangeAI();
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Squad ActiveSquad = (Squad)ArrayReferences[0].ReferencedScript.GetContent();

                ActiveSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Info.Map, ActiveSquad));
                ActiveSquad.SquadAI.Load(AIPath);

                IsCompleted = true;
                Result = new List<object>();
            }

            [Editor(typeof(Selectors.AISelector), typeof(UITypeEditor)),
            CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string AIPath
            {
                get
                {
                    return _AIPath;
                }
                set
                {
                    _AIPath = value;
                }
            }
        }
    }
}
