using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnSquad : DeathmatchMapScript
        {
            ScriptSpawnSquadHelper UnitSpawner;

            public ScriptSpawnSquad()
                : this(null)
            {
            }

            public ScriptSpawnSquad(DeathmatchMap Map)
                : base(Map, 140, 70, "Spawn Squad", new string[] { "Spawn" }, new string[] { "Timer Ended", "Animation Ended", "SFX Ended" })
            {
                UnitSpawner = new ScriptSpawnSquadHelper(Map, this);
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    UnitSpawner.LeaderToSpawn = (ScriptUnit)GetDataContainerByID(LeaderToSpawnID, ScriptUnit.ScriptName);
                    UnitSpawner.WingmanAToSpawn = (ScriptUnit)GetDataContainerByID(WingmanAToSpawnID, ScriptUnit.ScriptName);
                    UnitSpawner.WingmanBToSpawn = (ScriptUnit)GetDataContainerByID(WingmanBToSpawnID, ScriptUnit.ScriptName);

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
                UnitSpawner.Load(BR);
            }

            public override void Save(BinaryWriter BW)
            {
                UnitSpawner.Save(BW);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnSquad(Map);
            }
            
            #region Properties

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
            public UInt32 WingmanAToSpawnID
            {
                get
                {
                    return UnitSpawner.WingmanAToSpawnID;
                }
                set
                {
                    UnitSpawner.WingmanAToSpawnID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 WingmanBToSpawnID
            {
                get
                {
                    return UnitSpawner.WingmanBToSpawnID;
                }
                set
                {
                    UnitSpawner.WingmanBToSpawnID = value;
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
