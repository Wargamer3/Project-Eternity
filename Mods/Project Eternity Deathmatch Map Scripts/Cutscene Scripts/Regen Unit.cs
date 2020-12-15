using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptRegenUnit : DeathmatchMapScript
        {
            private uint _UnitToRegenID;
            private float _HPRegenPercent;

            public ScriptRegenUnit()
                : this(null)
            {
                _UnitToRegenID = 0;
            }

            public ScriptRegenUnit(DeathmatchMap Map)
                : base(Map, 100, 50, "Regen Unit", new string[] { "Regen" }, new string[] { "Unit Regenerated" })
            {
                _UnitToRegenID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsEnded = true;
                ExecuteEvent(this, 0);
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToRegenID)
                            continue;

                        Map.ListPlayer[P].ListSquad[U].CurrentLeader.HealUnit((int)(Map.ListPlayer[P].ListSquad[U].CurrentLeader.HP * _HPRegenPercent));
                        break;
                    }
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                UnitToRegenID = BR.ReadUInt32();
                HPRegenPercent = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToRegenID);
                BW.Write(HPRegenPercent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptRegenUnit(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToRegenID
            {
                get
                {
                    return _UnitToRegenID;
                }
                set
                {
                    _UnitToRegenID = value;
                }
            }

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("Percentage of the HP to regen.")]
            public float HPRegenPercent
            {
                get
                {
                    return _HPRegenPercent;
                }
                set
                {
                    _HPRegenPercent = value;
                }
            }

            #endregion
        }
    }
}
