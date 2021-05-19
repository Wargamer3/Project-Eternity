using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ProjectileInfo
    {
        public readonly string Name;

        public float ProjectileSpeed;
        public bool AffectedByGravity;
        public bool RotatationAllowed;
        public SimpleAnimation ProjectileAnimation;
        public byte TrailType;
        public byte TrailEffectType;
        public SimpleAnimation TrailAnimation;

        public ProjectileInfo()
        {

        }

        public ProjectileInfo(BinaryReader BR)
        {
            ProjectileSpeed = BR.ReadSingle();
            AffectedByGravity = BR.ReadBoolean();
            RotatationAllowed = BR.ReadBoolean();

            ProjectileAnimation = new SimpleAnimation(BR, false);

            TrailType = BR.ReadByte();
            if (TrailType == 2)
            {
                TrailEffectType = BR.ReadByte();
                TrailAnimation = new SimpleAnimation(BR, false);
            }
        }

        public void Load(ContentManager Content)
        {
            if (ProjectileAnimation != null && ProjectileAnimation.Path != string.Empty)
            {
                ProjectileAnimation.Load(Content, "Animations/Sprites/");
            }
            if (TrailAnimation != null && TrailAnimation.Path != string.Empty)
            {
                TrailAnimation.Load(Content, "Animations/Sprites/");
            }
        }
    }
}