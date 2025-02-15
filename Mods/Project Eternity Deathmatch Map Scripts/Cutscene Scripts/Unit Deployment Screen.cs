using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptUnitDeploymentScreen : DeathmatchMapScript
        {
            private int _NumberOfUnitsToSpawn;
            private AnimatedSprite AnimationSprite;
            private FMODSound ActiveSound;
            private bool IsSpawning;

            public ScriptUnitDeploymentScreen()
                : this(null)
            {
            }

            public ScriptUnitDeploymentScreen(DeathmatchMap Map)
                : base(Map, 150, 50, "Unit Deployment Screen", new string[] { "Open Deployment Screen" }, new string[] { "Deployment completed" })
            {
                IsSpawning = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                Map.UnitDeploymentScreen.Open(_NumberOfUnitsToSpawn);
                IsActive = true;
                IsDrawn = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (IsSpawning)
                {
                    HandleSpawning(gameTime);
                }
                else
                {
                    switch (Map.UnitDeploymentScreen.State)
                    {
                        case UnitDeploymentScreen.States.UnitSelection:
                            Map.UnitDeploymentScreen.Update(gameTime);
                            break;

                        case UnitDeploymentScreen.States.SubMenu:
                            if (InputHelper.InputConfirmPressed())
                            {
                                switch (Map.UnitDeploymentScreen.SubMenuIndex)
                                {
                                    //Deploy Units
                                    case 0:
                                        IsSpawning = true;
                                        break;
                                    //Change Units
                                    case 1:
                                        Map.UnitDeploymentScreen.UndoSelection();
                                        Map.UnitDeploymentScreen.State = UnitDeploymentScreen.States.UnitSelection;
                                        break;
                                    //Change Formation
                                    case 2:
                                        break;
                                    //Intermission Screen
                                    case 3:
                                        break;

                                    default:
                                        Map.UnitDeploymentScreen.Update(gameTime);
                                        break;
                                }
                            }
                            else
                            {
                                Map.UnitDeploymentScreen.Update(gameTime);
                            }
                            break;

                        case UnitDeploymentScreen.States.StatusMenu:
                            Map.UnitDeploymentScreen.Update(gameTime);
                            break;
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                Map.UnitDeploymentScreen.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                _NumberOfUnitsToSpawn = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_NumberOfUnitsToSpawn);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptUnitDeploymentScreen(Map);
            }

            private void HandleSpawning(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (Map.UnitDeploymentScreen.ListSelectedUnit.Count > 0)
                {
                    List<EventPoint> PlayerSpawnPoint = Map.LayerManager[0].ListCampaignSpawns;

                    if (AnimationSprite == null)
                    {
                        Unit CurrentUnit = Map.UnitDeploymentScreen.ListSelectedUnit[0];
                        Map.UnitDeploymentScreen.ListSelectedUnit.RemoveAt(0);

                        string SpawnTexturePath = "Spawn_strip4";

                        foreach (EventPoint Spawn in PlayerSpawnPoint)
                        {
                            if (Spawn.LeaderName == null)
                            {
                                Spawn.LeaderName = CurrentUnit.RelativePath;
                                AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + SpawnTexturePath,
                                    new Vector2((Spawn.Position.X - Map.Camera2DPosition.X) * Map.TileSize.X, (Spawn.Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y),
                                    0.5f);

                                Vector3 FinalPosition;

                                Map.GetEmptyPosition(Spawn.Position, out FinalPosition);

                                Squad NewSquad = new Squad("", CurrentUnit);

                                Map.SpawnSquad(0, NewSquad, 0, new Vector2(FinalPosition.X, FinalPosition.Y), (int)FinalPosition.Z);

                                Map.MovementAnimation.Add(NewSquad, Spawn.Position, FinalPosition);
                            }
                        }
                    }
                    else
                    {
                        AnimationSprite.Update(gameTime);
                        if (AnimationSprite.AnimationEnded)
                        {
                            AnimationSprite = null;
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
                        }
                    }

                    //If a Unit is moving, wait until it finished moving before spawning the next unit
                    if (Map.MovementAnimation.Count > 0)
                    {
                        Map.MovementAnimation.MoveSquad(gameTime, Map);

                        if (Map.MovementAnimation.Count == 0)
                        {
                            AnimationSprite = null;
                        }
                    }
                }
                else
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            private void HandleChangeFormation(Microsoft.Xna.Framework.GameTime gameTime)
            {
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public int NumberOfUnitsToSpawn
            {
                get
                {
                    return _NumberOfUnitsToSpawn;
                }
                set
                {
                    _NumberOfUnitsToSpawn = value;
                }
            }

            #endregion
        }
    }
}
