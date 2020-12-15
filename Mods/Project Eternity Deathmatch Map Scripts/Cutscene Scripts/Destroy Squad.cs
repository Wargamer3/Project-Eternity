using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptDestroySquad : DeathmatchMapScript
        {
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

            private uint _SquadToDestroyID;
            private bool AnimationStarted;

            public ScriptDestroySquad()
                : this(null)
            {
                _SquadToDestroyID = 0;
                AnimationPosition = new Microsoft.Xna.Framework.Vector2(-1, -1);
            }

            public ScriptDestroySquad(DeathmatchMap Map)
                : base(Map, 100, 50, "Destroy Squad", new string[] { "Destroy" }, new string[] { "Unit Destroyed" })
            {
                _SquadToDestroyID = 0;
                AnimationPosition = new Microsoft.Xna.Framework.Vector2(-1, -1);
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!AnimationStarted)
                {
                    AnimationStarted = true;
                    for (int P = 0; P < Map.ListPlayer.Count && AnimationPosition.X == -1; P++)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && AnimationPosition.X == -1; U++)
                        {
                            if (Map.ListPlayer[P].ListSquad[U].ID != _SquadToDestroyID)
                                continue;

                            Map.ListPlayer[P].ListSquad[U].CurrentLeader.KillUnit();
                            Map.ListPlayer[P].ListSquad[U].UpdateSquad();
                            AnimationPosition = new Microsoft.Xna.Framework.Vector2(Map.ListPlayer[P].ListSquad[U].X * Map.TileSize.X, Map.ListPlayer[P].ListSquad[U].Y * Map.TileSize.Y);
                            break;
                        }
                    }
                    if (File.Exists("Content/Maps/Animations/" + _AnimationPath + ".xnb"))
                    {
                        IsDrawn = true;

                        AnimationSprite = new AnimatedSprite(Map.Content, "Animations/Bitmap Animations/" + _AnimationPath, AnimationPosition, _AnimationSpeed);
                    }
                    else
                    {
                        IsEnded = true;
                        ExecuteEvent(this, 0);
                    }

                    if (File.Exists("Content/Maps/SFX/" + _SFXPath + ".mp3"))
                    {
                        ActiveSound = new FMODSound(GameScreen.FMODSystem, "Content/Maps/SFX/" + _SFXPath + ".mp3");
                        ActiveSound.Play();
                    }
                }
                else
                {
                    if (AnimationSprite != null)
                    {
                        AnimationSprite.Update(gameTime);
                        if (AnimationSprite.AnimationEnded)
                        {
                            IsEnded = true;
                            ExecuteEvent(this, 0);
                        }
                    }
                    if (ActiveSound != null)
                    {
                        if (!ActiveSound.IsPlaying())
                        {
                            SoundSystem.ReleaseSound(ActiveSound);
                            ActiveSound = null;
                        }
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                AnimationSprite.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                SquadToDestroyID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(SquadToDestroyID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptDestroySquad(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 SquadToDestroyID
            {
                get
                {
                    return _SquadToDestroyID;
                }
                set
                {
                    _SquadToDestroyID = value;
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

            #endregion
        }
    }
}
