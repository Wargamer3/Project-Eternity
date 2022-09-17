namespace ProjectEternity.Core
{
    public interface ICollisionObjectHolder3D<T> where T : class, ICollisionObjectHolder3D<T>
    {
        CollisionObject3D<T> Collision { get; }
    }
}
