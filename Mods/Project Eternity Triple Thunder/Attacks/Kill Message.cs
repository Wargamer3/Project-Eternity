using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class KillMessage
    {
        public string Message;
        public Texture2D sprMessagePrefix;
        public float LifetimeRemaining;
        public float MaxLifetime;

        public KillMessage(string Message, float MaxLifetime, Texture2D sprMessagePrefix)
        {
            this.Message = Message;
            this.MaxLifetime = MaxLifetime;
            this.LifetimeRemaining = MaxLifetime;
            this.sprMessagePrefix = sprMessagePrefix;
        }
    }
}
