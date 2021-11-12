using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed partial class GameScriptHolder
    {
        public class UseCombo : TripleThunderScript, ScriptEvaluator
        {
            private int _WeaponIndex;

            public UseCombo()
                : base(100, 50, "Use Combo", new string[0], new string[0])
            {
                _WeaponIndex = -1;
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                CurrentRobot.ActiveAttackStance = "Walking";

                if (_WeaponIndex == -1)
                {
                    CurrentRobot.InitiateAttack(gameTime, AttackInputs.LightPress);
                }
                else
                {
                    CurrentRobot.PrimaryWeapons.ActiveWeapons[_WeaponIndex].InitiateAttack(gameTime, AttackInputs.LightPress, CurrentRobot.CurrentMovementInput, CurrentRobot.ActiveMovementStance, false, CurrentRobot);
                }

                Result = new List<object>();
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);
                _WeaponIndex = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_WeaponIndex);
            }

            public override AIScript CopyScript()
            {
                return new UseCombo();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public int WeaponIndex
            {
                get
                {
                    return _WeaponIndex;
                }
                set
                {
                    _WeaponIndex = value;
                }
            }
        }
    }
}
