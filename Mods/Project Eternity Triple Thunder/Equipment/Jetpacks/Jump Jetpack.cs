using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class JumpJetpack : ManualJetpack
    {
        public JumpJetpack(ISFXGenerator JetpackSFXGenerator, RobotAnimation Owner)
            : base(JetpackSFXGenerator, Owner)
        {
        }

        public override void OnJetpackUse(GameTime gameTime)
        {
            if (Owner.IsOnGround)
            {
                Owner.Speed -= new Vector2(0, 3f);
            }
            else
            {
                base.OnJetpackUse(gameTime);
            }
        }
    }
}
