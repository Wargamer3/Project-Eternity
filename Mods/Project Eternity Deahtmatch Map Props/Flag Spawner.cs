using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
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

    public class FlagSpawner : InteractiveProp
    {
        private Texture2D sprFlag;

        private readonly DeathmatchMap Map;

        private UnitMap3D Flag3D;
        private Flag TeamFlag;
        private string _SpritePath;
        private byte _Team;

        public bool IsUsed;

        public FlagSpawner(DeathmatchMap Map)
            : base("Flag", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            _SpritePath = string.Empty;
            IsUsed = false;
        }


        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _SpritePath = BR.ReadString();
            _Team = BR.ReadByte();

            if (!string.IsNullOrEmpty(_SpritePath))
            {
                sprFlag = Map.Content.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
                Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), sprFlag, 1);
                Flag3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), sprFlag, 1);
                TeamFlag = new Flag(Map, this, sprFlag, Flag3D);
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_SpritePath);
            BW.Write(_Team);
        }

        public void GragFlag(Squad ActiveSquad)
        {
            //Brought back the enemy flag
            if (Map.ListPlayer[Map.ActivePlayerIndex].Team == _Team && ActiveSquad.ItemHeld != null && ActiveSquad.ItemHeld.ItemType == "Flag")
            {
                Flag ActiveFlag = (Flag)ActiveSquad.ItemHeld;

                if (ActiveFlag.Owner.Team != _Team)
                {
                    foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
                    {
                        foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                        {
                            if (ActiveProp.PropName == "Flag")
                            {
                                FlagSpawner EnemyFlagSpawner = (FlagSpawner)ActiveProp;

                                if (EnemyFlagSpawner._Team != _Team)
                                {
                                    EnemyFlagSpawner.ReturnFlag();
                                    ActiveSquad.DropItem();
                                    ActiveSquad.CurrentLeader.RefillSP(10);
                                    ActiveSquad.CurrentLeader.RefillEN(5);
                                    //Get point
                                    ((CaptureTheFlagGameRule)Map.GameRule).GetPoint(Map.ListPlayer[Map.ActivePlayerIndex].Team);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else if (Map.ListPlayer[Map.ActivePlayerIndex].Team != _Team)
            {
                ActiveSquad.PickupItem(TeamFlag);
                IsUsed = true;
            }
        }

        public void ReturnFlag()
        {
            IsUsed = false;
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
                GragFlag(SelectedUnit);
            }
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (!IsUsed && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {
                GragFlag(StoppedUnit);
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
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (sprFlag == null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
            }
            else
            {
                if (!IsUsed)
                {
                    g.Draw(sprFlag, new Vector2(PosX, PosY), Color.White);
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
            FlagSpawner NewProp = new FlagSpawner(Map);

            if (sprFlag != null)
            {
                NewProp.Team = Team;
                NewProp.sprFlag = sprFlag;
                NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprFlag, 1);
                NewProp.Flag3D = new UnitMap3D(GameScreen.GraphicsDevice, Unit3D.UnitEffect3D, sprFlag, 1);
            }

            return NewProp;
        }

        #region Properties

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
                sprFlag = GameScreen.ContentFallback.Load<Texture2D>("Animations/Sprites/" + _SpritePath);
            }
        }

        [CategoryAttribute("Spawner"),
        DescriptionAttribute("The Team."),
        DefaultValueAttribute(0)]
        public byte Team
        {
            get
            {
                return _Team;
            }
            set
            {
                _Team = value;
            }
        }

        #endregion
    }
}
