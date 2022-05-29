using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.LightningSystem
{
    public struct Range
    {
        public Range(float Min, float Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
        public float Min;
        public float Max;
    }

    public class LightningDescriptor
    {
        public Range SubdivisionFraction = new Range(0.45f, 0.55f);
        public Range JitterForwardDeviation = new Range(-1.0f, 1.0f);
        public Range JitterLeftDeviation = new Range(-1.0f, 1.0f);//How many pixels the part can derive
        public float JitterDeviationRadius = 3.0f;
        public float JitterDecayRate = 0.6f;
        public float ForkLengthPercentage = 0.5f;
        public float ForkDecayRate = 0.5f;
        public Range ForkForwardDeviation = new Range(0.0f, 1.0f);
        public Range ForkLeftDeviation = new Range(-1.0f, 1.0f);
        public Color InteriorColor = Color.White;
        public Color ExteriorColor = Color.Blue;
        public float BaseWidth = 0.6f;
        public bool IsWidthDecreasing = true;
        public bool IsGlowEnabled = true;
        public float GlowIntensity = 0.5f;
        public float AnimationFramerate = -1.0f;
        public readonly List<LightingSection> ListTopology = new List<LightingSection>();

        public static LightningDescriptor LightningTree(LightningBolt Owner)
        {
            LightningDescriptor ld = new LightningDescriptor();
            ld.BaseWidth *= 20;
            //ld.SubdivisionFraction = new Range(0.45f * 20, 0.55f * 20);
            ld.JitterForwardDeviation = new Range(-20, 20);
            ld.JitterLeftDeviation = new Range(-20, 20);
            ld.JitterDeviationRadius = 3.0f;
            ld.JitterDecayRate = 0.6f;
            ld.ForkLengthPercentage = 0.5f;
            ld.ForkDecayRate = 0.5f;
            //ld.ForkForwardDeviation = new Range(-20, 20);
            //ld.ForkLeftDeviation = new Range(-20, 20);
            LightingSection CurrentSection = new LightingSectionFork(Owner, 0);
            ld.ListTopology.Add(CurrentSection);
            CurrentSection = CurrentSection.NextSection = new LightingSectionFork(Owner, 1);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 2);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 3);
            CurrentSection = CurrentSection.NextSection = new LightingSectionFork(Owner,  4);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 5);
            CurrentSection = CurrentSection.NextSection = new LightingSectionFork(Owner, 6);

            return ld;
        }

        public static LightningDescriptor CrimsonLine(LightningBolt Owner)
        {
            LightningDescriptor ld = new LightningDescriptor();
            ld.ForkDecayRate = 1.0f;
            ld.ForkLengthPercentage = 0.2f;
            ld.BaseWidth = 0.3f;
            ld.ExteriorColor = Color.Crimson;
            ld.ForkForwardDeviation = new Range(-1, 1);
            ld.ForkLeftDeviation = new Range(-1, 1);
            ld.ForkLengthPercentage = 0.2f;
            ld.GlowIntensity = 1.5f;
            ld.InteriorColor = Color.White;
            ld.IsGlowEnabled = true;
            ld.IsWidthDecreasing = false;
            ld.JitterDecayRate = 0.5f;
            ld.SubdivisionFraction = new Range(0.45f, 0.55f);

            LightingSection CurrentSection = new LightingSectionJitter(Owner, 0);
            ld.ListTopology.Add(CurrentSection);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 1);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 2);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 3);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 4);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 5);

            return ld;
        }

        public static LightningDescriptor RedSparkLine(LightningBolt Owner)
        {
            LightningDescriptor ld = new LightningDescriptor();
            ld.BaseWidth = 0.3f;
            ld.ExteriorColor = Color.Red;
            ld.ForkDecayRate = 0.7f;
            ld.ForkForwardDeviation = new Range(-1, 1);
            ld.ForkLeftDeviation = new Range(-1, 1);
            ld.ForkLengthPercentage = 0.2f;
            ld.GlowIntensity = 2.5f;
            ld.InteriorColor = Color.Green;
            ld.IsGlowEnabled = true;
            ld.IsWidthDecreasing = false;
            ld.JitterDeviationRadius = 0.4f;
            ld.JitterDecayRate = 1.0f;
            ld.SubdivisionFraction = new Range(0.2f, 0.8f);
            ld.AnimationFramerate = 20.0f;

            LightingSection CurrentSection = new LightingSectionJitter(Owner, 0);
            ld.ListTopology.Add(CurrentSection);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 1);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 2);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 3);
            CurrentSection = CurrentSection.NextSection = new LightingSectionFork(Owner, 4);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 5);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 6);

            return ld;
        }

        public static LightningDescriptor YellowBeam(LightningBolt Owner)
        {
            LightningDescriptor ld = new LightningDescriptor();

            LightingSection CurrentSection = new LightingSectionJitter(Owner, 0);
            ld.ListTopology.Add(CurrentSection);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 1);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 2);
            CurrentSection = CurrentSection.NextSection = new LightingSectionJitter(Owner, 3);
            CurrentSection = CurrentSection.NextSection = new LightingSectionFork(Owner, 4);

            ld.ForkForwardDeviation = new Range(-1, 1);
            ld.ForkLengthPercentage = 0.6f;
            ld.ExteriorColor = Color.Orange;
            ld.BaseWidth = 1.0f;
            ld.JitterDeviationRadius = 0.5f;
            ld.JitterDecayRate = 1.0f;
            ld.IsGlowEnabled = true;
            ld.GlowIntensity = 3.0f;

            return ld;
        }
    }
}
