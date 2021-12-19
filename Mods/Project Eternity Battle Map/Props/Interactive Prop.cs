using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class InteractiveProp
    {
        public enum PropCategories : byte { Interactive = 0, Physical = 1, Visual = 2 }

        public readonly string PropName;//HP, EN, Ammo, weapon, pillar.
        public readonly PropCategories PropCategory;//Used only for the map editor.
        public Vector3 Position;
        public int LayerIndex;
        public bool[,] ArrayMapSize;
        public bool CanBlockPath;

        protected InteractiveProp(string PropName, PropCategories PropCategory,  bool[,] ArrayMapSize, bool CanBlockPath)
        {
            this.PropName = PropName;
            this.PropCategory = PropCategory;
            this.ArrayMapSize = ArrayMapSize;
            this.CanBlockPath = CanBlockPath;
        }

        public abstract void Load(ContentManager Content);

        public InteractiveProp LoadCopy(BinaryReader BR)
        {
            InteractiveProp NewProp = Copy();

            NewProp.Position = new Vector3(BR.ReadSingle(), BR.ReadSingle(), 0);
            NewProp.LayerIndex = BR.ReadInt32();

            NewProp.DoLoad(BR);

            return NewProp;
        }

        public abstract void DoLoad(BinaryReader BR);

        public void Save(BinaryWriter BW)
        {
            BW.Write(PropName);

            BW.Write(Position.X);
            BW.Write(Position.Y);
            BW.Write(LayerIndex);

            DoSave(BW);
        }

        public abstract void DoSave(BinaryWriter BW);

        public abstract void Update(GameTime gameTime);

        public abstract List<ActionPanel> OnUnitSelected(Squad SelectedUnit);

        public abstract List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn);

        public abstract void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn);

        public abstract void OnUnitStop(Squad StoppedUnit);

        public abstract void OnBattleEnd(Squad Attacker, Squad Defender);

        public abstract void OnTurnEnd(int PlayerIndex);

        public abstract void Draw(CustomSpriteBatch g);

        public InteractiveProp Copy(Vector3 Position, int LayerIndex)
        {
            InteractiveProp NewProp = Copy();

            NewProp.Position = Position;
            NewProp.LayerIndex = LayerIndex;

            return NewProp;
        }

        protected abstract InteractiveProp Copy();

        public override string ToString()
        {
            return PropName;
        }

        public static Dictionary<string, InteractiveProp> LoadProps(params object[] Args)
        {
            Dictionary<string, InteractiveProp> DicMapCondition = new Dictionary<string, InteractiveProp>();
            string[] Files = Directory.GetFiles("Props/Battle Map", "*.dll", SearchOption.AllDirectories);
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<InteractiveProp> ListInteractiveProp = ReflectionHelper.GetObjectsFromBaseTypes<InteractiveProp>(typeof(InteractiveProp), ActiveAssembly.GetTypes(), Args);

                foreach (InteractiveProp Instance in ListInteractiveProp)
                {
                    DicMapCondition.Add(Instance.PropName, Instance);
                }
            }

            return DicMapCondition;
        }
    }
}
