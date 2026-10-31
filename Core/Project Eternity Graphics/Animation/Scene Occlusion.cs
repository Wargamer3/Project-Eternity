using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Graphics
{
    // Screen occlusion hook still read by the skinned shader. The life sim leaves it off.
    public static class SceneOcclusion
    {
        public static Texture2D Map;
        public static Matrix ViewProjection = Matrix.Identity;
        public static bool On;
    }
}
