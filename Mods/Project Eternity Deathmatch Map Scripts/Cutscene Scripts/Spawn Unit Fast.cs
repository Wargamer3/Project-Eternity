using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnUnitFast : DeathmatchMapScript
        {
            ScriptSpawnSquadHelper UnitSpawner;
            ScriptUnit Leader;

            public ScriptSpawnUnitFast()
                : this(null)
            {
            }

            public ScriptSpawnUnitFast(DeathmatchMap Map)
                : base(Map, 140, 70, "Spawn Unit Fast", new string[] { "Spawn" }, new string[] { "Timer Ended", "Animation Ended", "SFX Ended" })
            {
                UnitSpawner = new ScriptSpawnSquadHelper(Map, this);
                Leader = new ScriptUnit(Map);
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    UnitSpawner.LeaderToSpawn = Leader;

                    IsActive = true;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                UnitSpawner.Update(gameTime);
                IsEnded = UnitSpawner.IsEnded;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                UnitSpawner.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                SpawnPositionX = BR.ReadInt32();
                SpawnPositionY = BR.ReadInt32();

                SpawnPlayer = BR.ReadInt32();
                IsEventSquad = BR.ReadBoolean();
                IsPlayerControlled = BR.ReadBoolean();

                TimerType = (ScriptSpawnSquadHelper.TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
                AnimationPath = BR.ReadString();
                AnimationSpeed = BR.ReadSingle();
                SFXPath = BR.ReadString();
                AIPath = BR.ReadString();
                DefenseBattleBehavior = BR.ReadString();
                PartDropPath = BR.ReadString();

                LeaderUnitId = BR.ReadUInt32();
                Leader.Load(BR);
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(SpawnPositionX);
                BW.Write(SpawnPositionY);

                BW.Write(SpawnPlayer);
                BW.Write(IsEventSquad);
                BW.Write(IsPlayerControlled);

                BW.Write((int)TimerType);
                BW.Write(EndingValue);
                BW.Write(AnimationPath);
                BW.Write(AnimationSpeed);
                BW.Write(SFXPath);
                BW.Write(AIPath);
                BW.Write(DefenseBattleBehavior);
                BW.Write(PartDropPath);

                BW.Write(LeaderUnitId);
                Leader.Save(BW);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnUnitFast(Map);
            }

            #region Properties

            #region Spawn Attributes

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

            [Editor(typeof(Selectors.PartsSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Part Drop Path")]
            public string PartDropPath
            {
                get
                {
                    return UnitSpawner.PartDropPath;
                }
                set
                {
                    UnitSpawner.PartDropPath = value;
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

            #region Leader

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public uint LeaderUnitId
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

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string LeaderUnitName
            {
                get
                {
                    return Leader.SpawnUnitName;
                }
                set
                {
                    Leader.SpawnUnitName = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int LeaderUnitStatsUpgrade
            {
                get
                {
                    return Leader.SpawnUnitStatsUpgrade;
                }
                set
                {
                    Leader.SpawnUnitStatsUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int LeaderUnitAttackUpgrade
            {
                get
                {
                    return Leader.SpawnUnitAttackUpgrade;
                }
                set
                {
                    Leader.SpawnUnitAttackUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int LeaderCharacterLevel
            {
                get
                {
                    return Leader.SpawnCharacterLevel;
                }
                set
                {
                    if (value < 1)
                        Leader.SpawnCharacterLevel = 1;
                    else
                        Leader.SpawnCharacterLevel = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string[] LeaderCharacter
            {
                get
                {
                    return Leader.SpawnCharacter;
                }
                set
                {
                    Leader.SpawnCharacter = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute("Destroy the Unit after the mission is completed. If true, this Unit will not be allowed to be used outside of the active Battle Map.")]
            public bool LeaderDeleteAfterMission
            {
                get
                {
                    return Leader.DeleteAfterMission;
                }
                set
                {
                    Leader.DeleteAfterMission = value;
                }
            }

            #endregion

            #endregion
        }
    }
}
