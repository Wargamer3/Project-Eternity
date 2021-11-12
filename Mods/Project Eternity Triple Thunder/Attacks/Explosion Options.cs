using System.IO;
using ProjectEternity.GameScreens.AnimationScreen;
using FMOD;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public struct ExplosionOptions
    {
        public float ExplosionRadius;
        public float ExplosionWindPowerAtCenter;
        public float ExplosionWindPowerAtEdge;
        public float ExplosionWindPowerToSelfMultiplier;
        public float ExplosionDamageAtCenter;
        public float ExplosionDamageAtEdge;
        public float ExplosionDamageToSelfMultiplier;
        public string sndExplosionPath;
        public FMODSound sndExplosion;

        public SimpleAnimation ExplosionAnimation;

        public ExplosionOptions(ExplosionOptions Clone)
        {
            ExplosionRadius = Clone.ExplosionRadius;
            ExplosionWindPowerAtCenter = Clone.ExplosionWindPowerAtCenter;
            ExplosionWindPowerAtEdge = Clone.ExplosionWindPowerAtEdge;
            ExplosionWindPowerToSelfMultiplier = Clone.ExplosionWindPowerToSelfMultiplier;
            ExplosionDamageAtCenter = Clone.ExplosionDamageAtCenter;
            ExplosionDamageAtEdge = Clone.ExplosionDamageAtEdge;
            ExplosionDamageToSelfMultiplier = Clone.ExplosionDamageToSelfMultiplier;
            sndExplosionPath = Clone.sndExplosionPath;
            sndExplosion = Clone.sndExplosion;

            ExplosionAnimation = Clone.ExplosionAnimation.Copy();
        }

        public ExplosionOptions(BinaryReader BR)
        {
            ExplosionRadius = BR.ReadSingle();
            ExplosionWindPowerAtCenter = BR.ReadSingle();
            ExplosionWindPowerAtEdge = BR.ReadSingle();
            ExplosionWindPowerToSelfMultiplier = BR.ReadSingle();
            ExplosionDamageAtCenter = BR.ReadSingle();
            ExplosionDamageAtEdge = BR.ReadSingle();
            ExplosionDamageToSelfMultiplier = BR.ReadSingle();
            sndExplosionPath = BR.ReadString();

            if (!string.IsNullOrEmpty(sndExplosionPath))
            {
                sndExplosion = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + sndExplosionPath);
            }
            else
            {
                sndExplosion = null;
            }

            ExplosionAnimation = new SimpleAnimation(BR, true);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ExplosionRadius);
            BW.Write(ExplosionWindPowerAtCenter);
            BW.Write(ExplosionWindPowerAtEdge);
            BW.Write(ExplosionWindPowerToSelfMultiplier);
            BW.Write(ExplosionDamageAtCenter);
            BW.Write(ExplosionDamageAtEdge);
            BW.Write(ExplosionDamageToSelfMultiplier);
            BW.Write(sndExplosionPath ?? string.Empty);

            ExplosionAnimation.Save(BW);
        }
    }
}
