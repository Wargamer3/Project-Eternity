using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Transforming;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnTransformingUnitEventFast : DeathmatchMapScript
        {
            private ScriptSpawnSquadHelper UnitSpawner;

            private string _EventID;
            private int _TransformationIndex;
            private bool HasSpawned;

            public ScriptSpawnTransformingUnitEventFast()
                : this(null)
            {
            }

            public ScriptSpawnTransformingUnitEventFast(DeathmatchMap Map)
                : base(Map, 180, 70, "Spawn Transforming Unit Event Fast", new string[] { "Spawn" }, new string[] { "Timer Ended", "Animation Ended", "SFX Ended" })
            {
                _EventID = string.Empty;
                UnitSpawner = new ScriptSpawnSquadHelper(Map, this);
                UnitSpawner.WingmanAToSpawnID = 0;
                UnitSpawner.WingmanAToSpawnID = 0;
                _TransformationIndex = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    foreach (Unit ActiveUnit in Map.PlayerRoster.TeamUnits.GetAll())
                    {
                        if (ActiveUnit.ID == _EventID)
                        {
                            UnitSpawner.LeaderToSpawn = new ScriptUnit(Map);
                            UnitSpawner.LeaderToSpawn.OverrideUnit(ActiveUnit);
                            ActiveUnit.ReloadSkills(Map.Params.DicUnitType[ActiveUnit.UnitTypeName], Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                            break;
                        }
                    }

                    IsActive = true;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                UnitSpawner.Update(gameTime);
                UnitTransforming SpawnedUnit = UnitSpawner.LeaderToSpawn.SpawnUnit as UnitTransforming;
                if (SpawnedUnit != null && !HasSpawned)
                {
                    SpawnedUnit.ChangeUnit(_TransformationIndex);
                    HasSpawned = true;
                }

                IsEnded = UnitSpawner.IsEnded;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                UnitSpawner.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                _EventID = BR.ReadString();
                _TransformationIndex = BR.ReadInt32();
                UnitSpawner.Load(BR);
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_EventID);
                BW.Write(_TransformationIndex);
                UnitSpawner.Save(BW);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnTransformingUnitEventFast(Map);
            }

            #region Properties

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string EventID
            {
                get
                {
                    return _EventID;
                }
                set
                {
                    _EventID = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int TransformationIndex
            {
                get
                {
                    return _TransformationIndex;
                }
                set
                {
                    _TransformationIndex = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 LeaderToSpawnID
            {
                get
                {
                    return UnitSpawner.LeaderToSpawnID;
                }
                set
                {
                    UnitSpawner.LeaderToSpawnID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public int SpawnPositionX
            {
                get
                {
                    return UnitSpawner.SpawnPosition.X;
                }
                set
                {
                    UnitSpawner.SpawnPosition.X = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public int SpawnPositionY
            {
                get
                {
                    return UnitSpawner.SpawnPosition.Y;
                }
                set
                {
                    UnitSpawner.SpawnPosition.Y = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public int SpawnPlayer
            {
                get
                {
                    return UnitSpawner.SpawnPlayer;
                }
                set
                {
                    UnitSpawner.SpawnPlayer = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Decide if this Unit is important and shouldn't be destroyed.")]
            public bool IsEventSquad
            {
                get
                {
                    return UnitSpawner.IsEventSquad;
                }
                set
                {
                    UnitSpawner.IsEventSquad = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Decide if this Unit can be controlled by a player.")]
            public bool IsPlayerControlled
            {
                get
                {
                    return UnitSpawner.IsPlayerControlled;
                }
                set
                {
                    UnitSpawner.IsPlayerControlled = value;
                }
            }

            [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("The AI path"),
            DefaultValueAttribute(0)]
            public string AIPath
            {
                get
                {
                    return UnitSpawner.AIPath;
                }
                set
                {
                    UnitSpawner.AIPath = value;
                }
            }

            [TypeConverter(typeof(DefenseBattleBehaviorStringConverter)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Defense Battle Behavior."),
            DefaultValueAttribute("Smart Counterattack")]
            public string DefenseBattleBehavior
            {
                get
                {
                    return UnitSpawner.DefenseBattleBehavior;
                }
                set
                {
                    UnitSpawner.DefenseBattleBehavior = value;
                }
            }

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Frequency at which the Timer will updates."),
            DefaultValueAttribute(0)]
            public ScriptSpawnSquadHelper.TimerTypes TimerType
            {
                get
                {
                    return UnitSpawner.TimerType;
                }
                set
                {
                    UnitSpawner.TimerType = value;
                }
            }

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Number at which the Unit will spawn."),
            DefaultValueAttribute(1)]
            public int EndingValue
            {
                get
                {
                    return UnitSpawner.EndingValue;
                }
                set
                {
                    UnitSpawner.EndingValue = value;
                }
            }

            [Editor(typeof(Selectors.BitmapAnimationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawn Animation"),
            DescriptionAttribute("The Animation path."),
            DefaultValueAttribute(0)]
            public string AnimationPath
            {
                get
                {
                    return UnitSpawner.AnimationPath;
                }
                set
                {
                    UnitSpawner.AnimationPath = value;
                }
            }

            [CategoryAttribute("Spawn Animation"),
            DescriptionAttribute("The Animation speed.")]
            public float AnimationSpeed
            {
                get
                {
                    return UnitSpawner.AnimationSpeed;
                }
                set
                {
                    UnitSpawner.AnimationSpeed = value;
                }
            }

            [Editor(typeof(Selectors.SFXSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawn SFX"),
            DescriptionAttribute("The SFX path"),
            DefaultValueAttribute(0)]
            public string SFXPath
            {
                get
                {
                    return UnitSpawner.SFXPath;
                }
                set
                {
                    UnitSpawner.SFXPath = value;
                }
            }

            #endregion
        }
    }
}
