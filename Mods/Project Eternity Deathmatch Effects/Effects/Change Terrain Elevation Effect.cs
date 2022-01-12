using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class ChangeTerrainElevationEffect : DeathmatchEffect
    {
        public static string Name = "Change Terrain Elevation";

        private int _Depth;
        private int _Radius;

        public ChangeTerrainElevationEffect()
            : base(Name, false)
        {
        }

        public ChangeTerrainElevationEffect(DeathmatchParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Depth = BR.ReadInt32();
            _Radius = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Depth);
            BW.Write(_Radius);
        }

        protected override string DoExecuteEffect()
        {
            MapLayer ActiveLayer = Params.LocalContext.Map.LayerManager.ListLayer[0];

            foreach (Vector3 ActivePosition in Params.LocalContext.ArrayAttackPosition)
            {
                for (int X = -_Radius; X <= _Radius; ++X)
                {
                    for (int Y = -_Radius; Y <= _Radius; ++Y)
                    {
                        int FinalX = (int)ActivePosition.X + X;
                        int FinalY = (int)ActivePosition.Y + Y;

                        if (Math.Abs(X) + Math.Abs(Y) > Radius || FinalX < 0 || FinalX >= Params.LocalContext.Map.MapSize.X || FinalY < 0 || FinalY >= Params.LocalContext.Map.MapSize.Y)
                            continue;

                        ActiveLayer.ArrayTerrain[FinalX, FinalY].Position.Z -= Depth;
                    }
                }
            }

            Params.LocalContext.Map.LayerManager.LayerHolderDrawable.Reset();
            return "Depth: " + _Depth + " - Radius: " + _Radius;

        }

        protected override void ReactivateEffect()
        {
            //Don't change terrain on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            ChangeTerrainElevationEffect NewEffect = new ChangeTerrainElevationEffect(Params);

            NewEffect._Depth = _Depth;
            NewEffect._Radius = _Radius;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeTerrainElevationEffect NewEffect = (ChangeTerrainElevationEffect)Copy;

            _Depth = NewEffect._Depth;
            _Radius = NewEffect._Radius;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public int Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public int Radius
        {
            get { return _Radius; }
            set { _Radius = value; }
        }

        #endregion
    }
}
