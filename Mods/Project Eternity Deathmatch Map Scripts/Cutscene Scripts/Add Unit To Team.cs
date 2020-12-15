using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptAddUnitToTeam : DeathmatchMapScript
        {
            private UInt32 _UnitID;

            private bool _IsPresent;
            private bool _IsEvent;

            public ScriptAddUnitToTeam()
                : this(null)
            {
                _UnitID = 0;
            }

            public ScriptAddUnitToTeam(DeathmatchMap Map)
                : base(Map, 140, 70, "Add Unit To Team", new string[] { "Add unit" }, new string[] { "Unit added" })
            {
                _UnitID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                ScriptUnit UnitToSpawn = (ScriptUnit)GetDataContainerByID(_UnitID, ScriptUnit.ScriptName);

                if (UnitToSpawn != null && !UnitToSpawn.DeleteAfterMission)
                {
                    if (_IsPresent)
                    {
                        UnitToSpawn.SpawnUnit.TeamTags.AddTag("Present");
                    }
                    if (_IsEvent)
                    {
                        UnitToSpawn.SpawnUnit.TeamTags.AddTag("Event");
                    }
                    Map.PlayerRoster.TeamUnits.Add(UnitToSpawn.SpawnUnit);
                }

                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _UnitID = BR.ReadUInt32();
                _IsPresent = BR.ReadBoolean();
                _IsEvent = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitID);
                BW.Write(_IsPresent);
                BW.Write(_IsEvent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddUnitToTeam(Map);
            }

            #region Properties

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public UInt32 UnitID
            {
                get
                {
                    return _UnitID;
                }
                set
                {
                    _UnitID = value;
                }
            }


            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public bool IsPresent
            {
                get
                {
                    return _IsPresent;
                }
                set
                {
                    _IsPresent = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public bool IsEvent
            {
                get
                {
                    return _IsEvent;
                }
                set
                {
                    _IsEvent = value;
                }
            }

            #endregion
        }
    }
}
