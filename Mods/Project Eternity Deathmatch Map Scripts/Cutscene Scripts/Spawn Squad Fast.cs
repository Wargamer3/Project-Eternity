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
        public class ScriptSpawnSquadFast : DeathmatchMapScript
        {
            ScriptSpawnSquadHelper UnitSpawner;
            ScriptUnit Leader;
            ScriptUnit WingmanA;
            ScriptUnit WingmanB;

            public ScriptSpawnSquadFast()
                : this(null)
            {
            }

            public ScriptSpawnSquadFast(DeathmatchMap Map)
                : base(Map, 140, 70, "Spawn Squad Fast", new string[] { "Spawn" }, new string[] { "Timer Ended", "Animation Ended", "SFX Ended" })
            {
                UnitSpawner = new ScriptSpawnSquadHelper(Map, this);
                Leader = new ScriptUnit(Map);
                WingmanA = new ScriptUnit(Map);
                WingmanB = new ScriptUnit(Map);
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    UnitSpawner.LeaderToSpawn = Leader;
                    UnitSpawner.LeaderToSpawnID = Leader.ID;
                    if (!string.IsNullOrEmpty(WingmanA.SpawnUnitName))
                        UnitSpawner.WingmanAToSpawn = WingmanA;
                    if (!string.IsNullOrEmpty(WingmanB.SpawnUnitName))
                        UnitSpawner.WingmanBToSpawn = WingmanB;

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

                Leader.ID = BR.ReadUInt32();
                Leader.Load(BR);

                WingmanA.ID = BR.ReadUInt32();
                WingmanA.Load(BR);
                
                WingmanB.ID = BR.ReadUInt32();
                WingmanB.Load(BR);
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

                BW.Write(Leader.ID);
                Leader.Save(BW);
                BW.Write(WingmanA.ID);
                WingmanA.Save(BW);
                BW.Write(WingmanB.ID);
                WingmanB.Save(BW);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnSquadFast(Map);
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
                    return Leader.ID;
                }
                set
                {
                    Leader.ID = value;
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

            #region WingmanA

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public uint WingmanAUnitId
            {
                get
                {
                    return WingmanA.ID;
                }
                set
                {
                    WingmanA.ID = value;
                }
            }

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string WingmanAUnitName
            {
                get
                {
                    return WingmanA.SpawnUnitName;
                }
                set
                {
                    WingmanA.SpawnUnitName = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanAUnitStatsUpgrade
            {
                get
                {
                    return WingmanA.SpawnUnitStatsUpgrade;
                }
                set
                {
                    WingmanA.SpawnUnitStatsUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanAUnitAttackUpgrade
            {
                get
                {
                    return WingmanA.SpawnUnitAttackUpgrade;
                }
                set
                {
                    WingmanA.SpawnUnitAttackUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanACharacterLevel
            {
                get
                {
                    return WingmanA.SpawnCharacterLevel;
                }
                set
                {
                    if (value < 1)
                        WingmanA.SpawnCharacterLevel = 1;
                    else
                        WingmanA.SpawnCharacterLevel = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string[] WingmanACharacter
            {
                get
                {
                    return WingmanA.SpawnCharacter;
                }
                set
                {
                    WingmanA.SpawnCharacter = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute("Destroy the Unit after the mission is completed. If true, this Unit will not be allowed to be used outside of the active Battle Map.")]
            public bool WingmanADeleteAfterMission
            {
                get
                {
                    return WingmanA.DeleteAfterMission;
                }
                set
                {
                    WingmanA.DeleteAfterMission = value;
                }
            }

            #endregion

            #region WingmanB

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public uint WingmanBUnitId
            {
                get
                {
                    return WingmanB.ID;
                }
                set
                {
                    WingmanB.ID = value;
                }
            }

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string WingmanBUnitName
            {
                get
                {
                    return WingmanB.SpawnUnitName;
                }
                set
                {
                    WingmanB.SpawnUnitName = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanBUnitStatsUpgrade
            {
                get
                {
                    return WingmanB.SpawnUnitStatsUpgrade;
                }
                set
                {
                    WingmanB.SpawnUnitStatsUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanBUnitAttackUpgrade
            {
                get
                {
                    return WingmanB.SpawnUnitAttackUpgrade;
                }
                set
                {
                    WingmanB.SpawnUnitAttackUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int WingmanBCharacterLevel
            {
                get
                {
                    return WingmanB.SpawnCharacterLevel;
                }
                set
                {
                    if (value < 1)
                        WingmanB.SpawnCharacterLevel = 1;
                    else
                        WingmanB.SpawnCharacterLevel = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string[] WingmanBCharacter
            {
                get
                {
                    return WingmanB.SpawnCharacter;
                }
                set
                {
                    WingmanB.SpawnCharacter = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute("Destroy the Unit after the mission is completed. If true, this Unit will not be allowed to be used outside of the active Battle Map.")]
            public bool WingmanBDeleteAfterMission
            {
                get
                {
                    return WingmanB.DeleteAfterMission;
                }
                set
                {
                    WingmanB.DeleteAfterMission = value;
                }
            }

            #endregion

            #endregion
        }
    }
}
