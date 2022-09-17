using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class WorldObject : ICollisionObjectHolder2D<WorldObject>
    {
        CollisionObject2D<WorldObject> CollisionBox;
        public CollisionObject2D<WorldObject> Collision => CollisionBox;

        public WorldObject(Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            CollisionBox = new CollisionObject2D<WorldObject>(ArrayVertex, MaxWidth, MaxHeight);
        }
    }
}
