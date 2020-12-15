using FMOD;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SFX
    {
        public FMODSound SFXSound;
        public string SFXPath;
        public bool Loop;
        public float Volume;
        public int DeathFrame;
        public int Length;

        public SFX()
        {
            SFXSound = null;
            SFXPath = "";
            Loop = false;
            Volume = 1f;
            Length = -1;
        }

        public SFX(string SFXPath, bool Loop)
        {
            SFXSound = null;
            this.SFXPath = SFXPath;
            this.Loop = Loop;
            Volume = 1f;
            Length = -1;
        }

        public override string ToString()
        {
            return SFXPath;
        }
    }
}
