using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class GroundExplosion
    {
        public static AnimatedSprite GroundExplosion1;
        public static AnimatedSprite GroundExplosion2;

        public static List<GroundExplosion> ListGroundExplosion = new List<GroundExplosion>();
        public static void AddGroundExplosion(GroundExplosion NewGroundExplosion)
        {
            for (int P = 0; P < GroundExplosion.ListGroundExplosion.Count; P++)
            {
                if (!GroundExplosion.ListGroundExplosion[P].IsAlive)
                {
                    GroundExplosion.ListGroundExplosion[P] = NewGroundExplosion;
                    return;
                }
            }

            GroundExplosion.ListGroundExplosion.Add(NewGroundExplosion);
        }

        public Vector2 Position;
        public bool IsAlive;
        AnimatedSprite ActiveSprite;

        protected GroundExplosion(Vector2 Position, AnimatedSprite ActiveSprite)
		{
			IsAlive = true;
            this.Position = Position;
            this.ActiveSprite = ActiveSprite;
        }
        public GroundExplosion(Vector2 Position)
        {
            IsAlive = true;
            this.Position = Position;

            if (SuperTank2.Randomizer.Next(2) == 1)
                ActiveSprite = GroundExplosion1;
            else
                ActiveSprite = GroundExplosion2;
        }
        public void Draw(CustomSpriteBatch g)
        {
            ActiveSprite.Draw(g, 0, Position, 0, Color.White);
        }
    }
}
