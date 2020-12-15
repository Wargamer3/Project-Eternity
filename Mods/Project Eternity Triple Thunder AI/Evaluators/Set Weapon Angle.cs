using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class SetWeaponAngle : TripleThunderScript, ScriptEvaluator
        {
            private int _WeaponIndex;

            public SetWeaponAngle()
                : base(100, 50, "Set Weapon Angle", new string[0], new string[1] { "Angle" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                double TargetAngle = (double)ArrayReferences[0].ReferencedScript.GetContent();
                RobotAnimation CurrentRobot = Info.Owner;

                float FinalAngle = MathHelper.ToRadians((float)TargetAngle);
                CurrentRobot.Weapons.ActivePrimaryWeapons[_WeaponIndex].WeaponAngle = FinalAngle;
                CurrentRobot.UpdatePrimaryWeaponAngle(FinalAngle, _WeaponIndex);

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
                return new SetWeaponAngle();
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
