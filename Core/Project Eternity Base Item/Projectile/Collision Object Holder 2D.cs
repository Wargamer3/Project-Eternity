namespace ProjectEternity.Core
{
    public interface ICollisionObjectHolder2D<T> where T : class, ICollisionObjectHolder2D<T>
    {
        CollisionObject2D<T> Collision { get; }
    }
}
