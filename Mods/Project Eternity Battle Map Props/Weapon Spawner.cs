using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class AnimationSpritesSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsSprites);
                if (Items != null)
                {
                    value = Items[0].Substring(0, Items[0].Length - 4).Substring(27);
                }
            }
            return value;
        }
    }

    public class WeaponSpawner : InteractiveProp
    {
        private Texture2D sprWeapon;

        private readonly BattleMap Map;

        private string _WeaponName;
        private string _SpritePath;
        private int _RespawnTime;
        private byte _Ammo;
        private bool IsUsed;
        private int TurnUsed;
        private int TurnRemaining;

        public WeaponSpawner(BattleMap Map)
            : base("Weapon Spawner", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _WeaponName = string.Empty;
            _SpritePath = string.Empty;
            _RespawnTime = 1;
            _Ammo = 1;

            IsUsed = false;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _WeaponName = BR.ReadString();
            _SpritePath = BR.ReadString();
            _RespawnTime = BR.ReadInt32();
            _Ammo = BR.ReadByte();

            if (!string.IsNullOrEmpty(_SpritePath))
            {
                sprWeapon = Map.Content.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), sprWeapon, 1);
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_WeaponName);
            BW.Write(_SpritePath);
            BW.Write(_RespawnTime);
            BW.Write(_Ammo);
        }

        public void PickupWeapon(Squad SquadToUse)
        {
            SquadToUse.CurrentLeader.AddTemporaryAttack(_WeaponName, _SpritePath, sprWeapon, Unit3D.UnitEffect3D, _Ammo, Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);
            IsUsed = true;
            TurnUsed = Map.ActivePlayerIndex;
            TurnRemaining = _RespawnTime;
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
            WeaponSpawner NewProp = new WeaponSpawner(Map);

            if (sprWeapon != null)
            {
                NewProp.sprWeapon = sprWeapon;
                NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprWeapon, 1);
            }

            return NewProp;
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
                sprWeapon = GameScreen.ContentFallback.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
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

        #endregion
    }
}
