using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PhysicsManager
    {
        private readonly LifeSimCubeSimulation CubeSimulation = new LifeSimCubeSimulation();

        public bool DemoCubeActive => CubeSimulation.IsActive;

        public void ClearMapMeshes()
        {
            CubeSimulation.ClearStaticTriangles();
        }

        public void AddMapMesh(Microsoft.Xna.Framework.Vector3[] Vertices, int[] Indices)
        {
            CubeSimulation.AddStaticTriangles(Vertices, Indices);
        }

        public Microsoft.Xna.Framework.Vector3 CubePosition => CubeSimulation.Position;

        public void Initialize()
        {
            CubeSimulation.Start();
        }

        public void Update()
        {
            CubeSimulation.Update();
        }

        public void Draw(Microsoft.Xna.Framework.Matrix ViewMatrix, Microsoft.Xna.Framework.Matrix ProjectionMatrix)
        {
            CubeSimulation.Draw(GameScreen.GraphicsDevice, ViewMatrix, ProjectionMatrix);
        }
    }
}
