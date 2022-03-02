using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class IsCloseToPoint : DeathmatchScript, ScriptEvaluator
        {
            private float _MaxDistance;

            public IsCloseToPoint()
                : base(150, 50, "Is Close To Point", new string[2] { "Condition is true", "Condition is false" }, new string[1] { "Point" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Vector3 Target = (Vector3)ArrayReferences[0].ReferencedScript.GetContent();
                List<MovementAlgorithmTile> ListMovement = Info.Map.GetMVChoicesTowardPoint(Info.ActiveSquad, Target, true);
                List<MovementAlgorithmTile> ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);

                bool IsEverythingCompleted = true;
                if (ListMovement.Count > 0 && ListMovement.Count < _MaxDistance)
                {
                    ExecuteFollowingScripts(0, gameTime, null, out IsCompleted, out Result);
                    IsEverythingCompleted &= IsCompleted;
                }
                else
                {
                    ExecuteFollowingScripts(1, gameTime, null, out IsCompleted, out Result);
                    IsEverythingCompleted &= IsCompleted;
                }

                IsCompleted = false;
                Result = new List<object>() { "break" };
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _MaxDistance = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_MaxDistance);
            }

            public override AIScript CopyScript()
            {
                return new IsCloseToPoint();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public float MaxDistance
            {
                get
                {
                    return _MaxDistance;
                }
                set
                {
                    _MaxDistance = value;
                }
            }
        }
    }
}
