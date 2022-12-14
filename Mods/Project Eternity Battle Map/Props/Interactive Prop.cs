using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class InteractiveProp
    {
        public enum PropCategories : byte { Interactive = 0, Physical = 1, Visual = 2 }

        public readonly string PropName;//HP, EN, Ammo, weapon, pillar.
        public readonly PropCategories PropCategory;//Used only for the map editor.
        public Vector3 Position;
        public bool[,] ArrayMapSize;
        public bool CanBlockPath;
        public UnitMap3D Unit3D;
        public AnimatedModel Unit3DModel;

        public Matrix Projection;
        protected InteractiveProp(string PropName, PropCategories PropCategory, bool[,] ArrayMapSize, bool CanBlockPath)
        {
            this.PropName = PropName;
            this.PropCategory = PropCategory;
            this.ArrayMapSize = ArrayMapSize;
            this.CanBlockPath = CanBlockPath;

            if (GameScreen.GraphicsDevice != null)
            {
                float aspectRatio = GameScreen.GraphicsDevice.Viewport.Width / (float)GameScreen.GraphicsDevice.Viewport.Height;
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                   aspectRatio,
                                                                   1, 10000);
            }
        }

        public abstract void Load(ContentManager Content);

        public InteractiveProp LoadCopy(BinaryReader BR, int LayerIndex)
        {
            InteractiveProp NewProp = Copy();

            NewProp.Position = new Vector3(BR.ReadSingle(), BR.ReadSingle(), LayerIndex);

            NewProp.DoLoad(BR);

            return NewProp;
        }

        public abstract void DoLoad(BinaryReader BR);

        public void Save(BinaryWriter BW)
        {
            BW.Write(PropName);

            BW.Write(Position.X);
            BW.Write(Position.Y);

            DoSave(BW);
        }

        public abstract void DoSave(BinaryWriter BW);

        public abstract void Update(GameTime gameTime);

        public void FinishMoving(Squad MovingSquad, List<Vector3> ListMVHoverPoints)
        {
            foreach (Vector3 MovedOverPoint in ListMVHoverPoints)
            {
                OnMovedOverBeforeStop(MovingSquad, MovedOverPoint, MovingSquad.Position);
            }

            OnUnitStop(MovingSquad);
        }

        public abstract void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit);

        public abstract void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn);

        public abstract void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn);

        public abstract void OnUnitStop(Squad StoppedUnit);

        public abstract void OnBattleEnd(Squad Attacker, Squad Defender);

        public abstract void OnTurnEnd(int PlayerIndex);

        public abstract void Draw(CustomSpriteBatch g);

        public abstract void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g);

        public InteractiveProp Copy(Vector3 Position, int LayerIndex)
        {
            InteractiveProp NewProp = Copy();

            NewProp.Position = Position;
            NewProp.Position.Z = LayerIndex;

            return NewProp;
        }

        protected abstract InteractiveProp Copy();

        public override string ToString()
        {
            return PropName;
        }

        public static Dictionary<string, InteractiveProp> LoadProps(params object[] Args)
        {
            Dictionary<string, InteractiveProp> DicInteractiveProp = new Dictionary<string, InteractiveProp>();
            string[] Files = Directory.GetFiles("Props/Battle Map", "*.dll", SearchOption.AllDirectories);
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<InteractiveProp> ListInteractiveProp = ReflectionHelper.GetObjectsFromBaseTypes<InteractiveProp>(typeof(InteractiveProp), ActiveAssembly.GetTypes(), Args);

                foreach (InteractiveProp Instance in ListInteractiveProp)
                {
                    DicInteractiveProp.Add(Instance.PropName, Instance);
                }
            }

            return DicInteractiveProp;
        }

        public static Dictionary<string, InteractiveProp> LoadFromAssemblyFiles(string[] ArrayFilePath, params object[] Args)
        {
            Dictionary<string, InteractiveProp> DicInteractiveProp = new Dictionary<string, InteractiveProp>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, InteractiveProp> ActiveUnit in LoadFromAssembly(ActiveAssembly, Args))
                {
                    DicInteractiveProp.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            return DicInteractiveProp;
        }

        public static Dictionary<string, InteractiveProp> LoadFromAssembly(Assembly ActiveAssembly, params object[] Args)
        {
            Dictionary<string, InteractiveProp> DicInteractiveProp = new Dictionary<string, InteractiveProp>();
            List<InteractiveProp> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<InteractiveProp>(typeof(InteractiveProp), ActiveAssembly.GetTypes(), Args);

            foreach (InteractiveProp Instance in ListSkillEffect)
            {
                DicInteractiveProp.Add(Instance.PropName, Instance);
            }

            return DicInteractiveProp;
        }

    }
}
