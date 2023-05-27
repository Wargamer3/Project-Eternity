using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public sealed partial class ConquestCutsceneScriptHolder
    {
        public class ScriptSpawnUnitConquest : ConquestMapScript
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
            private AnimatedSprite AnimationSprite;

            #endregion

            #region SFX

            private string _SFXPath;
            private FMODSound ActiveSound;

            #endregion

            private string _UnitToSpawn;
            private UInt32 _UnitToSpawnID;
            private Microsoft.Xna.Framework.Vector3 _SpawnPosition;
            private int _SpawnPlayer;
            private bool UnitSpawned;
            private bool IsTimerEnded;

            private string _AIPath;

            public ScriptSpawnUnitConquest()
                : this(null)
            {
            }

            public ScriptSpawnUnitConquest(ConquestMap Map)
                : base(Map, 140, 70, "Spawn Conquest Unit", new string[] { "Spawn" }, new string[] { "Timer Ended", "Animation Ended", "SFX Ended" })
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;
                IsEnded = false;
                IsTimerEnded = false;

                _UnitToSpawn = string.Empty;
                _UnitToSpawnID = 0;
                _SpawnPosition = new Microsoft.Xna.Framework.Vector3();
                _SpawnPlayer = 0;

                _AnimationPath = "";
                _AnimationSpeed = 0.5f;

                _SFXPath = "";

                _AIPath = "";
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

                        AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath, new Microsoft.Xna.Framework.Vector2((_SpawnPosition.X - Map.Camera2DPosition.X) * Map.TileSize.X, (_SpawnPosition.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y), _AnimationSpeed);
                    }
                    else
                        ExecuteEvent(this, 1);

                    if (File.Exists("Content/SFX/" + _SFXPath + ".mp3"))
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + _SFXPath + ".mp3");
                        ActiveSound.Play();
                    }
                    else
                        ExecuteEvent(this, 2);
                }
                if (AnimationSprite != null)
                {
                    AnimationSprite.Update(gameTime);
                    if (AnimationSprite.AnimationEnded)
                    {
                        ExecuteEvent(this, 1);
                    }
                }
                if (ActiveSound != null)
                {
                    if (!ActiveSound.IsPlaying())
                    {
                        if (ActiveSound != null)
                        {
                            SoundSystem.ReleaseSound(ActiveSound);
                            ActiveSound = null;
                        }
                        ExecuteEvent(this, 2);
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
                if (AnimationSprite != null)
                    AnimationSprite.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                _UnitToSpawn = BR.ReadString();
                _UnitToSpawnID = BR.ReadUInt32();
                SpawnPosition = new Microsoft.Xna.Framework.Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());

                SpawnPlayer = BR.ReadInt32();

                TimerType = (ScriptSpawnUnitConquest.TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
                AnimationPath = BR.ReadString();
                AnimationSpeed = BR.ReadSingle();
                SFXPath = BR.ReadString();

                _AIPath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitToSpawn);
                BW.Write(_UnitToSpawnID);
                BW.Write(SpawnPosition.X);
                BW.Write(SpawnPosition.Y);
                BW.Write(SpawnPosition.Z);

                BW.Write(SpawnPlayer);

                BW.Write((int)TimerType);
                BW.Write(EndingValue);
                BW.Write(AnimationPath);
                BW.Write(AnimationSpeed);
                BW.Write(SFXPath);

                BW.Write(_AIPath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnUnitConquest(Map);
            }

            private void SpawnSquad()
            {
                if (_UnitToSpawnID == 0)
                {
                    _UnitToSpawnID = Map.GetNextUnusedUnitID();
                }
                else
                {
                    if (Map.ListPlayer.Count > SpawnPlayer)
                    {
                        //Don't spawn the unit if there's already on with this ID.
                        for (int U = 0; U < Map.ListPlayer[SpawnPlayer].ListUnit.Count; U++)
                        {
                            if (Map.ListPlayer[SpawnPlayer].ListUnit[U].ID == _UnitToSpawnID)
                                return;
                        }
                    }
                }
                Microsoft.Xna.Framework.Vector3 FinalSpawnPosition;

                Map.GetEmptyPosition(_SpawnPosition, out FinalSpawnPosition);

                _SpawnPosition = FinalSpawnPosition;

                UnitConquest NewUnit = new UnitConquest(_UnitToSpawn.Remove(0, 9), Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect);
                NewUnit.ID = _UnitToSpawnID;

                if (!string.IsNullOrEmpty(AIPath))
                {
                    NewUnit.SquadAI = new ConquestScripAIContainer(new ConquestAIInfo(Map, NewUnit));
                    NewUnit.SquadAI.Load(AIPath);
                }

                Map.SpawnUnit(SpawnPlayer, NewUnit, _SpawnPosition);
            }

            #region Properties

            [Editor(typeof(ConquestUnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string UnitToSpawn
            {
                get
                {
                    return _UnitToSpawn;
                }
                set
                {
                    _UnitToSpawn = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public Microsoft.Xna.Framework.Vector3 SpawnPosition
            {
                get
                {
                    return _SpawnPosition;
                }
                set
                {
                    _SpawnPosition = value;
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

            [Editor(typeof(Selectors.BitmapAnimationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawn Animation"),
            DescriptionAttribute("The Animation path."),
            DefaultValueAttribute(0)]
            public string AnimationPath
            {
                get
                {
                    return _AnimationPath;
                }
                set
                {
                    _AnimationPath = value;
                }
            }

            [CategoryAttribute("Spawn Animation"),
            DescriptionAttribute("The Animation speed.")]
            public float AnimationSpeed
            {
                get
                {
                    return _AnimationSpeed;
                }
                set
                {
                    _AnimationSpeed = value;
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
                    return _SFXPath;
                }
                set
                {
                    _SFXPath = value;
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
                    return _AIPath;
                }
                set
                {
                    _AIPath = value;
                }
            }

            #endregion
        }
    }
}
