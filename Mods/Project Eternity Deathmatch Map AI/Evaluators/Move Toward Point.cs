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
        public class MoveTowardPoint : DeathmatchScript, ScriptEvaluator
        {
            private bool _AttackAfterMove;

            public MoveTowardPoint()
                : base(150, 50, "Move Toward Point", new string[0], new string[1] { "Point" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Vector3 Target = (Vector3)ArrayReferences[0].ReferencedScript.GetContent();
                List<MovementAlgorithmTile> ListMovement = Info.Map.GetMVChoicesTowardPoint(Info.ActiveSquad, Target, false);
                List<MovementAlgorithmTile> ListMVChoice = Info.Map.GetMVChoice(Info.ActiveSquad);

                if (_AttackAfterMove)
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIMoveBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                        ListMovement, ListMVChoice,
                        new ActionPanelAIAttackBehavior(Info.Map, Info.Map.ActivePlayerIndex, Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad))));
                }
                else
                {
                    Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIMoveBehavior(
                        Info.Map,
                        Info.Map.ActivePlayerIndex,
                        Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                        ListMovement, ListMVChoice));
                }

                IsCompleted = false;
                Result = new List<object>() { "break" };
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _AttackAfterMove = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_AttackAfterMove);
            }

            public override AIScript CopyScript()
            {
                return new MoveTowardPoint();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public bool AttackAfterMove
            {
                get
                {
                    return _AttackAfterMove;
                }
                set
                {
                    _AttackAfterMove = value;
                }
            }
        }
    }
}
