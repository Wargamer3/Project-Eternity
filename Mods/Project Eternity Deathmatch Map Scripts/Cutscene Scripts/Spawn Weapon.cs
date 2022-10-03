using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.Core.Scripts.Selectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnWeapon : DeathmatchMapScript
        {
            private string _WeaponName;
            private string _SpritePath;
            private int _RespawnTime;
            private byte _Ammo;
            private Vector2 _Position;

            public ScriptSpawnWeapon()
                : this(null)
            {
            }

            public ScriptSpawnWeapon(DeathmatchMap Map)
                : base(Map, 100, 50, "Spawn Weapon", new string[] { "Spawn" }, new string[0])
            {
                _WeaponName = string.Empty;
                _SpritePath = string.Empty;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                WeaponPickup NewWeapon = new WeaponPickup(Map, WeaponName, SpritePath, RespawnTime, _Ammo, _Position);
                NewWeapon.Load(Map.Content);
                Map.LayerManager[0].ListProp.Add(NewWeapon);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _WeaponName = BR.ReadString();
                _SpritePath = BR.ReadString();
                _RespawnTime = BR.ReadInt32();
                _Ammo = BR.ReadByte();
                _Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_WeaponName);
                BW.Write(_SpritePath);
                BW.Write(_RespawnTime);
                BW.Write(_Ammo);
                BW.Write(_Position.X);
                BW.Write(_Position.Y);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnWeapon(Map);
            }

            #region Properties

            [Editor(typeof(AttackSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner"),
            DescriptionAttribute("The Weapon path."),
            DefaultValueAttribute(0)]
            public string WeaponName
            {
                get
                {
                    return _WeaponName;
                }
                set
                {
                    _WeaponName = value;
                }
            }

            [Editor(typeof(AnimationSpritesSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner"),
            DescriptionAttribute("The sprite path."),
            DefaultValueAttribute(0)]
            public string SpritePath
            {
                get
                {
                    return _SpritePath;
                }
                set
                {
                    _SpritePath = value;
                }
            }

            [CategoryAttribute("Spawner"),
            DescriptionAttribute("How many turns before it respawn."),
            DefaultValueAttribute(0)]
            public int RespawnTime
            {
                get
                {
                    return _RespawnTime;
                }
                set
                {
                    _RespawnTime = value;
                }
            }

            [CategoryAttribute("Spawner"),
            DescriptionAttribute("How much ammo to give."),
            DefaultValueAttribute(0)]
            public byte Ammo
            {
                get
                {
                    return _Ammo;
                }
                set
                {
                    _Ammo = value;
                }
            }

            [CategoryAttribute("Spawner"),
            DescriptionAttribute("Where to spawn the weapon."),
            DefaultValueAttribute(0)]
            public Vector2 Position
            {
                get
                {
                    return _Position;
                }
                set
                {
                    _Position = value;
                }
            }

            #endregion

            public class WeaponPickup : InteractiveProp
            {
                private Texture2D sprWeapon;

                private readonly DeathmatchMap Map;

                private string WeaponName;
                private string SpritePath;
                private int RespawnTime;
                private byte Ammo;
                private bool IsUsed;
                private int TurnUsed;
                private int TurnRemaining;

                public WeaponPickup(DeathmatchMap Map, string WeaponName, string SpritePath, int RespawnTime, byte Ammo, Vector2 Position)
                    : base("Weapon Pickup", PropCategories.Interactive, new bool[,] { { true } }, false)
                {
                    this.Map = Map;

                    this.WeaponName = WeaponName;
                    this.SpritePath = SpritePath;
                    this.RespawnTime = RespawnTime;
                    this.Ammo = Ammo;

                    this.Position.X = Position.X;
                    this.Position.Y = Position.Y;

                    IsUsed = false;
                }

                public override void Load(ContentManager Content)
                {
                    if (!string.IsNullOrEmpty(SpritePath))
                    {
                        sprWeapon = Map.Content.Load<Texture2D>("Animations/Sprites/" + SpritePath);
                        Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), sprWeapon, 1);
                    }
                }

                public override void DoLoad(BinaryReader BR)
                {
                    throw new NotImplementedException();
                }

                public override void DoSave(BinaryWriter BW)
                {
                    throw new NotImplementedException();
                }

                public void PickupWeapon(Squad SquadToUse)
                {
                    SquadToUse.CurrentLeader.AddTemporaryAttack(WeaponName, SpritePath, sprWeapon, Unit3D.UnitEffect3D, Ammo, Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);

                    if (RespawnTime < 0)
                    {
                        Map.LayerManager[0].ListProp.Remove(this);
                    }
                    else
                    {
                        IsUsed = true;
                        TurnUsed = Map.ActivePlayerIndex;
                        TurnRemaining = RespawnTime;
                    }
                }

                public override void Update(GameTime gameTime)
                {
                }

                public override void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit)
                {
                }

                public override void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn)
                {
                }

                public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
                {
                    if (!IsUsed && PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
                    {
                        PickupWeapon(SelectedUnit);
                    }
                }

                public override void OnUnitStop(Squad StoppedUnit)
                {
                    if (!IsUsed && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
                    {
                        PickupWeapon(StoppedUnit);
                    }
                }

                public override void OnBattleEnd(Squad Attacker, Squad Defender)
                {
                }

                public override void OnTurnEnd(int PlayerIndex)
                {
                    if (IsUsed && TurnUsed == PlayerIndex && --TurnRemaining <= 0)
                    {
                        IsUsed = false;
                    }
                }

                public override void Draw(CustomSpriteBatch g)
                {
                    float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
                    float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

                    if (sprWeapon == null)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
                    }
                    else
                    {
                        if (!IsUsed)
                        {
                            g.Draw(sprWeapon, new Vector2(PosX, PosY), Color.White);
                        }
                    }
                }

                public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
                {
                    if (!IsUsed && Unit3D != null)
                    {
                        Unit3D.SetViewMatrix(View);
                        Unit3D.Draw(GraphicsDevice);
                    }
                }

                protected override InteractiveProp Copy()
                {
                    WeaponPickup NewProp = new WeaponPickup(Map, WeaponName, SpritePath, RespawnTime, Ammo, new Vector2(Position.X, Position.Y));

                    if (sprWeapon != null)
                    {
                        NewProp.sprWeapon = sprWeapon;
                        NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprWeapon, 1);
                    }

                    return NewProp;
                }
            }
        }
    }
}
