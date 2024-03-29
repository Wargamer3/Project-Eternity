﻿using System.IO;

namespace ProjectEternity.Core.Attacks
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

        public ExplosionOptions(ExplosionOptions Clone)
        {
            ExplosionRadius = Clone.ExplosionRadius;
            ExplosionWindPowerAtCenter = Clone.ExplosionWindPowerAtCenter;
            ExplosionWindPowerAtEdge = Clone.ExplosionWindPowerAtEdge;
            ExplosionWindPowerToSelfMultiplier = Clone.ExplosionWindPowerToSelfMultiplier;
            ExplosionDamageAtCenter = Clone.ExplosionDamageAtCenter;
            ExplosionDamageAtEdge = Clone.ExplosionDamageAtEdge;
            ExplosionDamageToSelfMultiplier = Clone.ExplosionDamageToSelfMultiplier;
        }

        public ExplosionOptions(BinaryReader BR)
        {
            ExplosionRadius = BR.ReadSingle();
            if (ExplosionRadius > 0)
            {
                ExplosionWindPowerAtCenter = BR.ReadSingle();
                ExplosionWindPowerAtEdge = BR.ReadSingle();
                ExplosionWindPowerToSelfMultiplier = BR.ReadSingle();
                ExplosionDamageAtCenter = BR.ReadSingle();
                ExplosionDamageAtEdge = BR.ReadSingle();
                ExplosionDamageToSelfMultiplier = BR.ReadSingle();
            }
            else
            {
                ExplosionWindPowerAtCenter = 0;
                ExplosionWindPowerAtEdge = 0;
                ExplosionWindPowerToSelfMultiplier = 0;
                ExplosionDamageAtCenter = 0;
                ExplosionDamageAtEdge = 0;
                ExplosionDamageToSelfMultiplier = 0;
            }
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
        }
    }
}