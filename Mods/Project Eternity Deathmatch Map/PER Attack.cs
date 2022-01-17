using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PERAttack
    {
        public Attack ActiveAttack;
        public Squad Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public Vector3 Position;
        public Vector3 Speed;
        public int Lifetime;
        public Attack3D Map3DComponent;

        public PERAttack(Attack ActiveAttack, Squad Owner, int PlayerIndex, ContentManager Content, Vector3 Position, Vector3 Speed, int Lifetime)
        {
            this.ActiveAttack = ActiveAttack;
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            this.Position = Position;
            this.Speed = Speed;
            this.Lifetime = Lifetime;

            if (ActiveAttack.PERAttributes.ProjectileAnimation.IsAnimated)
            {
                Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.ActiveSprite, ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.FrameCount);
            }
            else
            {
                Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.StaticSprite, 1);
            }

            Map3DComponent.SetPosition(
                Position.X * 32 + 16 + 0.5f,
                Position.Z * 32,
                Position.Y * 32 + 16 + 0.5f);
        }
    }
}
