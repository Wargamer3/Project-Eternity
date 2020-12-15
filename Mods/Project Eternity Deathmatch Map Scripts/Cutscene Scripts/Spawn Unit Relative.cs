using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnUnitRelative : DeathmatchMapScript
        {
            #region Timer

            public enum TimerTypes { Miliseconds, Seconds, Minutes };

            private int _EndingValue;
            private TimeSpan TimerValue;
            private TimerTypes _TimerType;

            #endregion

            #region Animation

            private string _AnimationPath;
            private float _AnimationSpeed;

            private Microsoft.Xna.Framework.Vector2 AnimationPosition;
            private AnimatedSprite AnimationSprite;

            #endregion

            #region SFX

            private string _SFXPath;
            private FMODSound ActiveSound;

            #endregion

            private uint _LeaderToSpawnID;
            private uint _WingmanAToSpawnID;
            private uint _WingmanBToSpawnID;
            private uint _RelativeSpawnUnitID;
            private Point _RelativeSpawnPosition;
            private int _SpawnPlayer;
            private bool _IsEventSquad;
            private bool _IsPlayerControlled;

            private bool UnitSpawned;
            private bool IsTimerEnded;

            public ScriptSpawnUnitRelative()
                : this(null)
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;
                IsEnded = false;
                IsTimerEnded = false;

                _LeaderToSpawnID = 0;
                _WingmanAToSpawnID = 0;
                _WingmanBToSpawnID = 0;
                _RelativeSpawnUnitID = 0;
                _SpawnPlayer = 0;
                _IsEventSquad = false;
                _IsPlayerControlled = true;

                _AnimationPath = "";
                _AnimationSpeed = 0.5f;

                _SFXPath = "";
            }

            public ScriptSpawnUnitRelative(DeathmatchMap Map)
                : base(Map, 100, 50, "Spawn Squad Relative", new string[] { "Spawn" }, new string[] { "Unit Spawned" })
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;
                IsEnded = false;
                IsTimerEnded = false;

                _LeaderToSpawnID = 0;
                _WingmanAToSpawnID = 0;
                _WingmanBToSpawnID = 0;
                _RelativeSpawnUnitID = 0;
                _SpawnPlayer = 0;
                _IsEventSquad = false;

                _AnimationPath = "";
                _AnimationSpeed = 0.5f;

                _SFXPath = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                    IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!UnitSpawned)
                {
                    UnitSpawned = true;
                    SpawnSquad();

                    if (File.Exists("Content/Animations/Bitmap Animations/" + _AnimationPath + ".xnb"))
                    {
                        IsDrawn = true;

                        AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath, new Microsoft.Xna.Framework.Vector2(AnimationPosition.X - Map.CameraPosition.X * Map.TileSize.X, AnimationPosition.Y - Map.CameraPosition.Y * Map.TileSize.Y), _AnimationSpeed);
                    }

                    if (File.Exists("Content/SFX/" + _SFXPath + ".mp3"))
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + _SFXPath + ".mp3");
                        ActiveSound.Play();
                    }
                }

                if (AnimationSprite != null)
                    AnimationSprite.Update(gameTime);
                if (ActiveSound != null)
                {
                    if (!ActiveSound.IsPlaying())
                    {
                        SoundSystem.ReleaseSound(ActiveSound);
                        ActiveSound = null;
                    }
                }

                if (!IsTimerEnded)
                {
                    TimerValue += gameTime.ElapsedGameTime;

                    switch (_TimerType)
                    {
                        case TimerTypes.Miliseconds:
                            if (TimerValue.Milliseconds >= _EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Seconds:
                            if (TimerValue.Seconds >= _EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Minutes:
                            if (TimerValue.Minutes >= _EndingValue)
                                IsTimerEnded = true;
                            break;
                    }
                }
                if (IsTimerEnded)
                {
                    //Timer Ended Trigger.
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                AnimationSprite.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                LeaderToSpawnID = BR.ReadUInt32();
                WingmanAToSpawnID = BR.ReadUInt32();
                WingmanBToSpawnID = BR.ReadUInt32();
                RelativeSpawnUnitID = BR.ReadUInt32();
                RelativeSpawnPosition = new Point(BR.ReadInt32(), BR.ReadInt32());

                SpawnPlayer = BR.ReadInt32();
                IsEventSquad = BR.ReadBoolean();
                IsPlayerControlled = BR.ReadBoolean();

                TimerType = (ScriptSpawnUnitRelative.TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(LeaderToSpawnID);
                BW.Write(WingmanAToSpawnID);
                BW.Write(WingmanBToSpawnID);
                BW.Write(RelativeSpawnUnitID);
                BW.Write(RelativeSpawnPosition.X);
                BW.Write(RelativeSpawnPosition.Y);

                BW.Write(SpawnPlayer);
                BW.Write(IsEventSquad);
                BW.Write(IsPlayerControlled);

                BW.Write((int)TimerType);
                BW.Write(EndingValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnUnitRelative(Map);
            }

            private void SpawnSquad()
            {
                ScriptUnit LeaderToSpawn = (ScriptUnit)GetDataContainerByID(_LeaderToSpawnID, ScriptUnit.ScriptName);
                ScriptUnit WingmanAToSpawn = (ScriptUnit)GetDataContainerByID(_WingmanAToSpawnID, ScriptUnit.ScriptName);
                ScriptUnit WingmanBToSpawn = (ScriptUnit)GetDataContainerByID(_WingmanBToSpawnID, ScriptUnit.ScriptName);

                Squad RelativeSpawnSquad = null;

                for (int P = 0; P < Map.ListPlayer.Count && RelativeSpawnSquad == null; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && RelativeSpawnSquad == null; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID == _RelativeSpawnUnitID)
                            RelativeSpawnSquad = Map.ListPlayer[P].ListSquad[U];
                        else
                            continue;
                    }
                }

                if (Map.ListPlayer.Count > SpawnPlayer)
                {
                    //Don't spawn the unit if there's already on with this ID.
                    for (int U = 0; U < Map.ListPlayer[SpawnPlayer].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[SpawnPlayer].ListSquad[U].ID == _LeaderToSpawnID)
                            return;
                    }
                }

                if (LeaderToSpawn != null && RelativeSpawnSquad != null)
                {
                    float SpawnPositionX = RelativeSpawnSquad.X + _RelativeSpawnPosition.X;
                    float SpawnPositionY = RelativeSpawnSquad.Y + _RelativeSpawnPosition.Y;
                    Microsoft.Xna.Framework.Vector3 FinalPosition;

                    Map.GetEmptyPosition(new Microsoft.Xna.Framework.Vector3(SpawnPositionX, SpawnPositionY, 0), out FinalPosition);
                    
                    Squad NewSquad = new Squad("", LeaderToSpawn.SpawnUnit, WingmanAToSpawn.SpawnUnit, WingmanBToSpawn.SpawnUnit);

                    NewSquad.IsEventSquad = IsEventSquad;
                    NewSquad.IsPlayerControlled = IsPlayerControlled;
                    AnimationPosition = new Microsoft.Xna.Framework.Vector2(NewSquad.X * Map.TileSize.X, NewSquad.Y * Map.TileSize.Y);
                    UnitSpawned = true;

                    Map.SpawnSquad(SpawnPlayer, NewSquad, _LeaderToSpawnID, FinalPosition);
                }
            }

            #region Properties

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 LeaderToSpawnID
            {
                get
                {
                    return _LeaderToSpawnID;
                }
                set
                {
                    _LeaderToSpawnID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 WingmanAToSpawnID
            {
                get
                {
                    return _WingmanAToSpawnID;
                }
                set
                {
                    _WingmanAToSpawnID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 WingmanBToSpawnID
            {
                get
                {
                    return _WingmanBToSpawnID;
                }
                set
                {
                    _WingmanBToSpawnID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public UInt32 RelativeSpawnUnitID
            {
                get
                {
                    return _RelativeSpawnUnitID;
                }
                set
                {
                    _RelativeSpawnUnitID = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public Point RelativeSpawnPosition
            {
                get
                {
                    return _RelativeSpawnPosition;
                }
                set
                {
                    _RelativeSpawnPosition = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public int SpawnPlayer
            {
                get
                {
                    return _SpawnPlayer;
                }
                set
                {
                    _SpawnPlayer = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Decide if this Unit is important and shouldn't be destroyed.")]
            public bool IsEventSquad
            {
                get
                {
                    return _IsEventSquad;
                }
                set
                {
                    _IsEventSquad = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Decide if this Unit can be controlled by a player.")]
            public bool IsPlayerControlled
            {
                get
                {
                    return _IsPlayerControlled;
                }
                set
                {
                    _IsPlayerControlled = value;
                }
            }

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Frequency at which the Timer will updates."),
            DefaultValueAttribute(0)]
            public TimerTypes TimerType
            {
                get
                {
                    return _TimerType;
                }
                set
                {
                    _TimerType = value;
                }
            }

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Number at which the Unit will spawn."),
            DefaultValueAttribute(1)]
            public int EndingValue
            {
                get
                {
                    return _EndingValue;
                }
                set
                {
                    _EndingValue = value;
                }
            }

            #endregion
        }
    }
}
