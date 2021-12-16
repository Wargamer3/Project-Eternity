using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SphericalMap3D : CubeMap3D
    {
        protected override bool Spherical { get { return true; } }

        public SphericalMap3D(DeathmatchMap Map, int LayerIndex, GraphicsDevice g)
            : base(Map, LayerIndex, g)
        {
        }
    }
}
