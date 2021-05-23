using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class WorldObject : ICollisionObject<WorldObject>
    {
        CollisionObject<WorldObject> CollisionBox;
        public CollisionObject<WorldObject> Collision => CollisionBox;

        public WorldObject(Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            CollisionBox = new CollisionObject<WorldObject>(ArrayVertex, MaxWidth, MaxHeight);
        }
    }
}
