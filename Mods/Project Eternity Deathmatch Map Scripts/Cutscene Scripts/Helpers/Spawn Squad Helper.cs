﻿using System;
using System.IO;
using System.Drawing;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnSquadHelper
        {
            #region Timer

            public enum TimerTypes { Miliseconds, Seconds, Minutes };

            public int EndingValue;
            private TimeSpan TimerValue;
            public TimerTypes TimerType;

            #endregion

            #region Animation

            public string AnimationPath;
            public float AnimationSpeed;
            private AnimatedSprite AnimationSprite;

            #endregion

            #region SFX

            private string _SFXPath;
            public string SFXPath { get { return _SFXPath; }
                set
                { 
                    _SFXPath = value;
                    if (File.Exists("Content/SFX/" + SFXPath + ".mp3"))
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + SFXPath + ".mp3");
                    }
                }
            }
            private FMODSound ActiveSound;

            #endregion

            public uint LeaderToSpawnID;
            public uint WingmanAToSpawnID;
            public uint WingmanBToSpawnID;
            public Point SpawnPosition;
            public int SpawnLayer;
            public int SpawnPlayer;
            public bool IsEventSquad;
            public bool IsPlayerControlled;
            private bool UnitSpawned;
            private bool AnimationEnded;
            private bool SoundEnded;
            private bool IsTimerEnded;

            public bool IsEnded;
            private DeathmatchMap Map;
            private DeathmatchMapScript Owner;

            public ScriptUnit LeaderToSpawn;
            public ScriptUnit WingmanAToSpawn;
            public ScriptUnit WingmanBToSpawn;
            public ScriptUnit WingmanCToSpawn;
            public string AIPath;
            public string DefenseBattleBehavior;
            public string PartDropPath;
            public string[] ListTag;

            public ScriptSpawnSquadHelper(DeathmatchMap Map, DeathmatchMapScript Owner)
            {
                this.Map = Map;
                this.Owner = Owner;
                EndingValue = 1;
                TimerType = TimerTypes.Seconds;
                IsEnded = false;
                AnimationEnded = false;
                SoundEnded = false;
                IsTimerEnded = false;

                LeaderToSpawnID = 0;
                WingmanAToSpawnID = 0;
                WingmanBToSpawnID = 0;
                SpawnPosition = new Point();
                SpawnLayer = 0;
                SpawnPlayer = 1;
                IsEventSquad = false;
                IsPlayerControlled = true;

                AnimationPath = string.Empty;
                AnimationSpeed = 15f;

                SFXPath = string.Empty;
                AIPath = string.Empty;
                DefenseBattleBehavior = string.Empty;
                PartDropPath = string.Empty;
                ListTag = new string[0];
            }
            
            public void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!UnitSpawned)
                {
                    UnitSpawned = true;
                    SpawnSquad();

                    if (File.Exists("Content/Animations/Bitmap Animations/" + AnimationPath + ".xnb") && Map.Content != null)
                    {
                        Owner.IsDrawn = true;

                        AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + AnimationPath,
                            new Microsoft.Xna.Framework.Vector2(
                                (SpawnPosition.X - Map.Camera2DPosition.X) * Map.TileSize.X + Map.TileSize.X * 0.5f,
                                (SpawnPosition.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + Map.TileSize.Y * 0.5f), AnimationSpeed);
                    }
                    else
                    {
                        Owner.ExecuteEvent(Owner, 1);
                    }

                    if (ActiveSound != null)
                    {
                        ActiveSound.Play();
                    }
                    else if (File.Exists("Content/SFX/" + SFXPath + ".mp3") && GameScreen.FMODSystem != null)
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + SFXPath + ".mp3");
                        ActiveSound.Play();
                    }
                    else
                    {
                        Owner.ExecuteEvent(Owner, 2);
                    }
                }

                if (AnimationSprite != null && !AnimationEnded)
                {
                    AnimationSprite.Update(gameTime);
                    if (AnimationSprite.AnimationEnded)
                    {
                        AnimationEnded = true;
                        Owner.ExecuteEvent(Owner, 1);
                    }
                }
                else
                {
                    AnimationEnded = true;
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
                        SoundEnded = true;
                        Owner.ExecuteEvent(Owner, 2);
                    }
                }
                else
                {
                    SoundEnded = true;
                }

                if (!IsTimerEnded)
                {
                    TimerValue += gameTime.ElapsedGameTime;

                    switch (TimerType)
                    {
                        case TimerTypes.Miliseconds:
                            if (TimerValue.Milliseconds >= EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Seconds:
                            if (TimerValue.Seconds >= EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Minutes:
                            if (TimerValue.Minutes >= EndingValue)
                                IsTimerEnded = true;
                            break;
                    }
                    if (IsTimerEnded)
                    {
                        Owner.ExecuteEvent(Owner, 0);
                    }
                }

                if (AnimationEnded && SoundEnded && IsTimerEnded)
                {
                    IsEnded = true;
                }
            }

            public void Draw(CustomSpriteBatch g)
            {
                if (AnimationSprite != null && !AnimationSprite.AnimationEnded)
                    AnimationSprite.Draw(g);
            }

            public void Load(BinaryReader BR)
            {
                LeaderToSpawnID = BR.ReadUInt32();
                WingmanAToSpawnID = BR.ReadUInt32();
                WingmanBToSpawnID = BR.ReadUInt32();
                SpawnPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
                SpawnLayer = BR.ReadInt32();

                SpawnPlayer = BR.ReadInt32();
                IsEventSquad = BR.ReadBoolean();
                IsPlayerControlled = BR.ReadBoolean();

                TimerType = (TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
                AnimationPath = BR.ReadString();
                AnimationSpeed = BR.ReadSingle();
                SFXPath = BR.ReadString();
                AIPath = BR.ReadString();
                DefenseBattleBehavior = BR.ReadString();
            }

            public void Save(BinaryWriter BW)
            {
                BW.Write(LeaderToSpawnID);
                BW.Write(WingmanAToSpawnID);
                BW.Write(WingmanBToSpawnID);
                BW.Write(SpawnPosition.X);
                BW.Write(SpawnPosition.Y);
                BW.Write(SpawnLayer);

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
            }
            
            private void SpawnSquad()
            {
                int PlayerIndex = SpawnPlayer - 1;

                if (Map.ListPlayer.Count > PlayerIndex)
                {
                    //Don't spawn the unit if there's already on with this ID.
                    for (int U = 0; U < Map.ListPlayer[PlayerIndex].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[PlayerIndex].ListSquad[U].ID == LeaderToSpawnID)
                            return;
                    }
                }

                if (LeaderToSpawn != null)
                {
                    Microsoft.Xna.Framework.Vector3 SpawnPositionReal = new Microsoft.Xna.Framework.Vector3(SpawnPosition.X * Map.TileSize.X + Map.TileSize.X / 2, SpawnPosition.Y * Map.TileSize.Y + Map.TileSize.Y / 2, SpawnLayer * Map.LayerHeight);

                    Microsoft.Xna.Framework.Vector3 FinalPosition;

                    if (Map.IsInsideMap(SpawnPositionReal))
                    {
                        FinalPosition = SpawnPositionReal;
                    }
                    else
                    {
                        Map.GetEmptyPosition(SpawnPositionReal, out FinalPosition);
                    }

                    LeaderToSpawn.Init();
                    if (WingmanAToSpawn != null)
                    {
                        WingmanAToSpawn.Init();
                    }
                    if (WingmanBToSpawn != null)
                    {
                        WingmanBToSpawn.Init();
                    }
                    if (WingmanCToSpawn != null)
                    {
                        WingmanCToSpawn.Init();
                    }

                    Squad NewSquad = new Squad("", LeaderToSpawn.SpawnUnit, WingmanAToSpawn == null ? null : WingmanAToSpawn.SpawnUnit, WingmanBToSpawn == null ? null : WingmanBToSpawn.SpawnUnit, WingmanCToSpawn == null ? null : WingmanCToSpawn.SpawnUnit);
                    NewSquad.IsEventSquad = IsEventSquad;
                    NewSquad.IsPlayerControlled = IsPlayerControlled;
                    NewSquad.SquadDefenseBattleBehavior = DefenseBattleBehavior;

                    if (!string.IsNullOrEmpty(AIPath))
                    {
                        NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Map, NewSquad));
                        NewSquad.SquadAI.Load(AIPath);
                    }

                    Map.SpawnSquad(PlayerIndex, NewSquad, LeaderToSpawnID, new Microsoft.Xna.Framework.Vector2(FinalPosition.X, FinalPosition.Y), (int)FinalPosition.Z);

                    if (!string.IsNullOrEmpty(PartDropPath))
                    {
                        NewSquad.ListParthDrop.Add(PartDropPath);
                    }
                }
            }

            public void SpawnUnitServer()
            {
                UnitSpawned = true;
                SpawnSquad();
                Owner.ExecuteEvent(Owner, 0);
                Owner.ExecuteEvent(Owner, 1);
                Owner.ExecuteEvent(Owner, 2);
            }
        }
    }
}
