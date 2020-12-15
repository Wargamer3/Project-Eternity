using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class EnemySpawner
    {
        private double Timer;
        private double TimeBeforeSpawn;
        protected int EnemyHP;

        public EnemySpawner(double TimeBeforeSpawn, int EnemyHP)
        {
            this.TimeBeforeSpawn = TimeBeforeSpawn;
            this.EnemyHP = EnemyHP;
        }

        public void Update(GameTime gameTime)
        {
            Timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (Timer <= 0)
            {
                SpawnEnemy();
                Timer = TimeBeforeSpawn;
            }
        }

        public abstract void SpawnEnemy();
    }

    public class Plane1Spawner : EnemySpawner
    {
        public Plane1Spawner(double TimeBeforeSpawn, int EnemyHP)
            : base(TimeBeforeSpawn, EnemyHP)
        {
        }

        public override void SpawnEnemy()
        {
            Plane1 NewPlane = new Plane1(EnemyHP, 50);
            Enemy.AddEnemy(NewPlane);
        }
    }

    public class Plane2Spawner : EnemySpawner
    {
        public Plane2Spawner(double TimeBeforeSpawn, int EnemyHP)
            : base(TimeBeforeSpawn, EnemyHP)
        {
        }

        public override void SpawnEnemy()
        {
            Plane2 NewPlane = new Plane2(EnemyHP, 50);
            Enemy.AddEnemy(NewPlane);
        }
    }

    public class Plane3Spawner : EnemySpawner
    {
        public Plane3Spawner(double TimeBeforeSpawn, int EnemyHP)
            : base(TimeBeforeSpawn, EnemyHP)
        {
        }

        public override void SpawnEnemy()
        {
            Plane3 NewPlane = new Plane3(EnemyHP, 50);
            Enemy.AddEnemy(NewPlane);
        }
    }

    public class Plane4Spawner : EnemySpawner
    {
        public Plane4Spawner(double TimeBeforeSpawn, int EnemyHP)
            : base(TimeBeforeSpawn, EnemyHP)
        {
        }

        public override void SpawnEnemy()
        {
            Plane4 NewPlane = new Plane4(EnemyHP, 50);
            Enemy.AddEnemy(NewPlane);
        }
    }
}
