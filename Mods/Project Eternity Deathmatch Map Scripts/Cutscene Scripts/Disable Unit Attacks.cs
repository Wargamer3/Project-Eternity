using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptDisableUnitAttacks : DeathmatchMapScript
        {
            private uint _UnitToDisableID;

            public ScriptDisableUnitAttacks()
                : this(null)
            {
                _UnitToDisableID = 0;
            }

            public ScriptDisableUnitAttacks(DeathmatchMap Map)
                : base(Map, 100, 50, "Diable Unit Attacks", new string[] { "Disable" }, new string[] { "Attacks Diabled" })
            {
                _UnitToDisableID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToDisableID)
                            continue;

                        Map.ListPlayer[P].ListSquad[U].CurrentLeader.UnitStat.RegularAttackDisabled = true;
                        break;
                    }
                }
                IsEnded = true;
                ExecuteEvent(this, 0);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            public override void Load(BinaryReader BR)
            {
                _UnitToDisableID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitToDisableID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptDisableUnitAttacks(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToDisableID
            {
                get
                {
                    return _UnitToDisableID;
                }
                set
                {
                    _UnitToDisableID = value;
                }
            }

            #endregion
        }
    }
}
