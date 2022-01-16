using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class WeaponSelector : UITypeEditor
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
                List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttacks);
                if (Items != null)
                {
                    value = Path.GetFileNameWithoutExtension(Items[0]);
                }
            }
            return value;
        }
    }

    public class WeaponSpawner : InteractiveProp
    {
        private Texture2D sprCrate;

        private readonly BattleMap Map;

        private string _WeaponName;
        private int _RespawnTime;
        private int _Ammo;
        private bool IsUsed;

        public WeaponSpawner(BattleMap Map)
            : base("Weapon Spawner", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _WeaponName = string.Empty;
            _RespawnTime = 1;
            _Ammo = 1;

            IsUsed = false;
        }

        public override void Load(ContentManager Content)
        {
            sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
            Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), sprCrate, 1);
        }

        public override void DoLoad(BinaryReader BR)
        {
            _WeaponName = BR.ReadString();
            _RespawnTime = BR.ReadInt32();
            _Ammo = BR.ReadInt32();
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_WeaponName);
            BW.Write(_RespawnTime);
            BW.Write(_Ammo);
        }

        public void PickupWeapon(Squad SquadToUse)
        {
            SquadToUse.CurrentLeader.UnitStat.ListAttackTemporary.Add(null);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override List<ActionPanel> OnUnitSelected(Squad SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
            if (PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                IsUsed = true;
            }
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (!IsUsed)
            {
                float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
                float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

                g.Draw(sprCrate, new Vector2(PosX, PosY), Color.White);
            }
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice)
        {
            if (!IsUsed)
            {
                Unit3D.Draw(GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            WeaponSpawner NewProp = new WeaponSpawner(Map);

            NewProp.sprCrate = sprCrate;
            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Squad shader 3D"), sprCrate, 1);

            return NewProp;
        }

        #region Properties

        [Editor(typeof(WeaponSelector), typeof(UITypeEditor)),
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
        public int Ammo
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
